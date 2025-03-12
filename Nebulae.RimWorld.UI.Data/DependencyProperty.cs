using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 表示用于验证要设置给依赖属性的值的方法
    /// </summary>
    /// <param name="value">要验证的值</param>
    /// <returns>如果值对于依赖属性有效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
    public delegate bool ValidateValueCallback(object value);

    /// <summary>
    /// 依赖属性的标识
    /// </summary>
    public class DependencyProperty : IEquatable<DependencyProperty>
    {
        /// <summary>
        /// 表示一个对于依赖属性不合理的值
        /// </summary>
        public static readonly object UnsetValue = new DebugValue("DependencyProperty.UnsetValue");


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static readonly Type _nullableValueType = typeof(Nullable<>);

        /// <summary>
        /// 已注册的依赖属性
        /// </summary>
        private static readonly Dictionary<Key, DependencyProperty> _registeredProperties = new Dictionary<Key, DependencyProperty>();

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        /// <summary>
        /// 依赖属性的默认元数据
        /// </summary>
        private readonly PropertyMetadata _defaultMetadata;

        /// <summary>
        /// 重写了依赖属性元数据的类型
        /// </summary>
        private SortedSet<DependencyObjectType> _overrideInfo;

        /// <summary>
        /// 依赖属性的元数据
        /// </summary>
        private readonly Dictionary<DependencyObjectType, PropertyMetadata> _metadata = new Dictionary<DependencyObjectType, PropertyMetadata>();

        /// <summary>
        /// 临时的依赖属性元数据，存储继承的元数据
        /// </summary>
        private Dictionary<DependencyObjectType, PropertyMetadata> _temporaryMetadata;

        /// <summary>
        /// 属性验证回调函数
        /// </summary>
        private readonly ValidateValueCallback _validateValueCallback;

        private readonly bool _isAttached;

        private readonly Key _globalKey;
        private readonly Type _valueType;

        private bool _isMetadataOverridden;
        private bool _isMetadataTempDirty;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 依赖属性名称
        /// </summary>
        public string Name => _globalKey.Name;

        /// <summary>
        /// 拥有依赖属性的对象的类型
        /// </summary>
        public Type OwnerType => _globalKey.OwnerType;

        /// <summary>
        /// 依赖属性的值的类型
        /// </summary>
        public Type ValueType => _valueType;

        #endregion


        /// <summary>
        /// 初始化 <see cref="DependencyProperty"/> 的新实例
        /// </summary>
        /// <param name="globalKey">依赖属性的全局键</param>
        /// <param name="valueType">依赖属性的值的类型</param>
        /// <param name="defaultMetadata">依赖属性的默认元数据</param>
        /// <param name="validateValueCallback">属性验证回调函数</param>
        /// <param name="isAttached">指示该依赖属性是否为附加属性</param>
        private DependencyProperty(
            Key globalKey,
            Type valueType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback,
            bool isAttached = false)
        {
            _globalKey = globalKey;

            _isAttached = isAttached;
            _isMetadataOverridden = false;

            _valueType = valueType;
            _defaultMetadata = defaultMetadata;
            _validateValueCallback = validateValueCallback;
        }


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

        /// <summary>
        /// 搜索依赖属性
        /// </summary>
        /// <param name="ownerType">拥有依赖属性的类型</param>
        /// <param name="name">依赖属性的名称，即 <see cref="Name"/>。</param>
        /// <returns>对应的依赖属性标识。</returns>
        /// <exception cref="MissingMemberException">当无法在 <paramref name="ownerType"/> 中找到名为 <paramref name="name"/> 的依赖属性时发生。</exception>
        public static DependencyProperty Search(Type ownerType, string name)
        {
            if (_registeredProperties.TryGetValue(new Key(name, ownerType), out var property))
            {
                return property;
            }
            else
            {
                ownerType = ownerType.BaseType;

                if (DependencyObjectType.RootType.IsAssignableFrom(ownerType))
                {
                    return Search(ownerType, name);
                }
            }

            throw new MissingMemberException($"Cannot find Dependency Property with name: {name} in {ownerType}.");
        }


        /// <summary>
        /// 注册依赖属性
        /// </summary>
        /// <param name="name">依赖属性名称</param>
        /// <param name="valueType">依赖属性的值的类型</param>
        /// <param name="ownerType">拥有依赖属性的对象的类型</param>
        /// <param name="defaultMetadata">依赖属性的默认元数据</param>
        /// <param name="validateValueCallback">属性验证回调函数</param>
        /// <returns><see cref="DependencyProperty"/> 的新实例。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/>、<paramref name="valueType"/> 或 <paramref name="ownerType"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当目标依赖属性已被注册时发生。</exception>
        public static DependencyProperty Register(
            string name,
            Type valueType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback = null)
        {
            return RegisterCommon(name, valueType, ownerType, defaultMetadata, validateValueCallback, false);
        }

        /// <summary>
        /// 注册附加属性
        /// </summary>
        /// <param name="name">附加属性名称</param>
        /// <param name="valueType">附加属性的值的类型</param>
        /// <param name="ownerType">拥有附加属性的对象的类型</param>
        /// <param name="defaultMetadata">附加属性的默认元数据</param>
        /// <param name="validateValueCallback">属性验证回调函数</param>
        /// <returns><see cref="DependencyProperty"/> 的新实例。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/>、<paramref name="valueType"/> 或 <paramref name="ownerType"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当目标附加属性已被注册时发生。</exception>
        public static DependencyProperty RegisterAttached(
            string name,
            Type valueType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback = null)
        {
            return RegisterCommon(name, valueType, ownerType, defaultMetadata, validateValueCallback, true);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断依赖属性是否与指定对象相等
        /// </summary>
        /// <param name="obj">要判断的对象</param>
        /// <returns>如果相等，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj)
                || (obj is DependencyProperty other
                    && _globalKey.Equals(other._globalKey));
        }

        /// <summary>
        /// 判断依赖属性是否与指定依赖属性相等
        /// </summary>
        /// <param name="other">要判断的依赖属性</param>
        /// <returns>如果相等，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(DependencyProperty other)
        {
            return ReferenceEquals(this, other)
                || _globalKey.Equals(other._globalKey);
        }

        /// <summary>
        /// 获取依赖属性的哈希码
        /// </summary>
        /// <returns>依赖属性的哈希码</returns>
        public override int GetHashCode() => _globalKey.HashCode;

        /// <summary>
        /// 获取依赖属性的元数据
        /// </summary>
        /// <param name="ownerType">拥有依赖属性的对象的类型</param>
        /// <returns>依赖属性的元数据。</returns>
        public PropertyMetadata GetMetadata(Type ownerType) => GetMetadata(DependencyObjectType.FromType(ownerType));

        /// <summary>
        /// 获取依赖属性与指定对象对应的元数据
        /// </summary>
        /// <param name="target">与要获取的元数据对应的对象。</param>
        /// <returns>依赖属性的元数据。</returns>
        public PropertyMetadata GetMetadata(DependencyObject target) => GetMetadata(target.DependencyType);

        /// <summary>
        /// 重写依赖属性的元数据
        /// </summary>
        /// <param name="ownerType">拥有依赖属性的对象的类型</param>
        /// <param name="metadata">新的元数据</param>
        /// <exception cref="ArgumentNullException">当任意参数为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当 <paramref name="ownerType"/> 不拥有该依赖属性时发生。</exception>
        public void OverrideMetadata(Type ownerType, PropertyMetadata metadata)
        {
            if (ownerType is null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (_isAttached)
            {
                throw new InvalidOperationException($"Unable to override the _metadata of the attached property {_globalKey.OwnerType}.{_globalKey.Name}.");
            }

            if (!ValidateValue(metadata.DefaultValue, out var e))
            {
                throw e;
            }

            if (!_globalKey.OwnerType.IsAssignableFrom(ownerType))
            {
                throw new InvalidOperationException($"The type {ownerType} must inherit {_globalKey.OwnerType} to override the _metadata of {_globalKey.OwnerType}.{_globalKey.Name}.");
            }

            DependencyObjectType dType = DependencyObjectType.FromType(ownerType);
            dType.Upgrade();
            metadata.MergeMetadata(GetMetadata(dType));

            _metadata.Add(dType, metadata);

            if (!_isMetadataOverridden)
            {
                _overrideInfo = new SortedSet<DependencyObjectType>(DependencyObjectType.Comparer);
                _temporaryMetadata = new Dictionary<DependencyObjectType, PropertyMetadata>();

                _isMetadataOverridden = true;
            }

            if (_isMetadataTempDirty)
            {
                _temporaryMetadata = new Dictionary<DependencyObjectType, PropertyMetadata>(_metadata);
                _isMetadataTempDirty = false;
            }

            _overrideInfo.Add(dType);
            _temporaryMetadata[dType] = metadata;
        }

        /// <summary>
        /// 返回依赖属性的名称
        /// </summary>
        /// <returns>依赖属性的名称。</returns>
        public override string ToString() => _globalKey.Name;

        /// <summary>
        /// 验证要设置给依赖属性的值
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="exception">验证失败时的错误信息</param>
        /// <returns>如果验证成功，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool ValidateValue(object value, out Exception exception)
        {
            return ValidateValue(value, _valueType, _globalKey, _validateValueCallback, out exception);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal static DependencyProperty RegisterCommon(
            string name,
            Type valueType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback = null,
            bool isAttached = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (valueType is null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }

            if (ownerType is null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            if (defaultMetadata is null)
            {
                defaultMetadata = new PropertyMetadata(null);
            }

            Key key = new Key(name, ownerType);

            if (_registeredProperties.ContainsKey(key))
            {
                throw new InvalidOperationException($"The property {ownerType}.{name} has been registered.");
            }

            if (!ValidateValue(defaultMetadata.DefaultValue, valueType, key, validateValueCallback, out var e))
            {
                throw e;
            }

            DependencyProperty property = new DependencyProperty(
                key,
                valueType,
                defaultMetadata,
                validateValueCallback,
                isAttached);
            defaultMetadata.Property = property;

            _registeredProperties.Add(key, property);
            return property;
        }

        /// <summary>
        /// 获取依赖属性的元数据
        /// </summary>
        /// <param name="ownerType">拥有依赖属性的对象的类型</param>
        /// <returns>依赖属性的元数据。</returns>
        internal PropertyMetadata GetMetadata(DependencyObjectType ownerType)
        {
            if (!_isMetadataOverridden
                || _globalKey.OwnerType == ownerType.OriginalType
                || _isAttached)
            {
                return _defaultMetadata;
            }

            if (!_temporaryMetadata.TryGetValue(ownerType, out PropertyMetadata metadata))
            {
                metadata = MapType(ownerType);
            }

            return metadata;
        }

        internal PropertyMetadata MapType(DependencyObjectType dType)
        {
            DependencyObjectType potentialMapResult;
            DependencyObjectType[] sortedInfo = _overrideInfo.ToArray();

            for (int i = 0; i < sortedInfo.Length; i++)
            {
                potentialMapResult = sortedInfo[i];
                if (potentialMapResult.OriginalType.IsAssignableFrom(dType.OriginalType))
                {
                    PropertyMetadata metadata = _metadata[potentialMapResult];
                    _temporaryMetadata[dType] = metadata;

                    _isMetadataTempDirty = true;
                    return metadata;
                }
            }

            return _defaultMetadata;
        }

        #endregion

        private static bool ValidateValue(
            object value,
            Type valueType,
            Key globalKey,
            ValidateValueCallback validate, out Exception exception)
        {
            Type ownerType = globalKey.OwnerType;
            string name = globalKey.Name;

            if (value is null)
            {
                if (valueType.IsValueType
                    && !(valueType.IsGenericType && valueType.GetGenericTypeDefinition() == _nullableValueType))
                {
                    exception = new InvalidOperationException($"{ownerType}.{name} can not set to be null.");
                    return false;
                }
            }
            else if (value.IsUnsetValue())
            {
                exception = new InvalidOperationException($"The value \"DependencyProperty.UnsetValue\" is invalid for any properties.");
                return false;
            }
            else if (!valueType.IsInstanceOfType(value) && !(value is IConvertible))
            {
                exception = new InvalidCastException($"Can not convert \"{value}\" to {valueType} for {ownerType}.{name}.");
                return false;
            }
            else if (validate != null
                && !validate(value))
            {
                exception = new ArgumentException($"The value \"{value}\" is not valid for {ownerType}.{name}.");
                return false;
            }

            exception = null;
            return true;
        }


        [DebuggerDisplay("{_value, nq}")]
        private class DebugValue
        {
            public DebugValue(string valueTag)
            {
                if (string.IsNullOrEmpty(valueTag))
                    throw new ArgumentNullException(nameof(valueTag));

                _value = valueTag;
            }

            public override string ToString() => _value;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly string _value;
        }

        private readonly struct Key : IEquatable<Key>
        {
            public readonly int HashCode;
            public readonly string Name;
            public readonly Type OwnerType;


            public Key(string name, Type ownerType)
            {
                HashCode = ownerType.GetHashCode() ^ name.GetHashCode();
                Name = name;
                OwnerType = ownerType;
            }


            public override bool Equals(object obj)
            {
                return obj is Key other
                    && Name == other.Name
                    && OwnerType == other.OwnerType;
            }

            public bool Equals(Key other)
            {
                return Name == other.Name
                    && OwnerType == other.OwnerType;
            }

            public override int GetHashCode()
            {
                return HashCode;
            }
        }
    }
}
