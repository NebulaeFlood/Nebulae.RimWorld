using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 表示用于验证要设置给依赖属性的值的方法
    /// </summary>
    /// <param name="value">要验证的值</param>
    /// <returns>如果值对于依赖属性有效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
    public delegate bool ValidateValueCallback(object value);

    /// <summary>
    /// 依赖属性
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
        private static readonly HashSet<DependencyProperty> _registeredProperties = new HashSet<DependencyProperty>();

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

        private readonly int _hashCode;
        private readonly bool _isAttached;
        private readonly string _name;
        private readonly Type _ownerType;
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
        public string Name => _name;

        /// <summary>
        /// 拥有依赖属性的对象的类型
        /// </summary>
        public Type OwnerType => _ownerType;

        /// <summary>
        /// 依赖属性的值的类型
        /// </summary>
        public Type ValueType => _valueType;

        #endregion


        /// <summary>
        /// 初始化 <see cref="DependencyProperty"/> 的新实例
        /// </summary>
        /// <param name="name">依赖属性名称</param>
        /// <param name="valueType">依赖属性的值的类型</param>
        /// <param name="ownerType">拥有依赖属性的对象的类型</param>
        /// <param name="defaultMetadata">依赖属性的默认元数据</param>
        /// <param name="validateValueCallback">属性验证回调函数</param>
        /// <param name="isAttached">指示该依赖属性是否为附加属性</param>
        private DependencyProperty(
            string name,
            Type valueType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback,
            bool isAttached = false)
        {
            _isAttached = isAttached;

            _isMetadataOverridden = false;

            _name = name;
            _valueType = valueType;
            _ownerType = ownerType;
            _defaultMetadata = defaultMetadata;
            _validateValueCallback = validateValueCallback;

            _hashCode = ownerType.GetHashCode() ^ name.GetHashCode();
        }


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

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
            return obj is DependencyProperty dependencyProperty
                && (ReferenceEquals(this, dependencyProperty)
                    || (_ownerType == dependencyProperty._ownerType
                        && _name == dependencyProperty._name));
        }

        /// <summary>
        /// 判断依赖属性是否与指定依赖属性相等
        /// </summary>
        /// <param name="other">要判断的依赖属性</param>
        /// <returns>如果相等，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(DependencyProperty other)
        {
            return ReferenceEquals(this, other)
                || (_ownerType == other._ownerType
                    && _name == other._name);
        }

        /// <summary>
        /// 获取依赖属性的哈希码
        /// </summary>
        /// <returns>依赖属性的哈希码</returns>
        public override int GetHashCode() => _hashCode;

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
                throw new InvalidOperationException($"Unable to override the _metadata of the attached property {_ownerType}.{_name}.");
            }

            if (!ValidateValue(metadata.DefaultValue))
            {
                ThrowInvalidValueException(metadata);
            }

            if (!_ownerType.IsAssignableFrom(ownerType))
            {
                throw new InvalidOperationException($"The type {ownerType} must inherit {_ownerType} to override the _metadata of {_ownerType}.{_name}.");
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
        public override string ToString() => _name;

        /// <summary>
        /// 验证要设置给依赖属性的值
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <returns>如果验证成功，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool ValidateValue(object value)
        {
            if (value is null)
            {
                if (_valueType.IsValueType
                    && !(_valueType.IsGenericType && _valueType.GetGenericTypeDefinition() == _nullableValueType))
                {
                    return false;
                }
            }
            else if (!_valueType.IsInstanceOfType(value) && !(value is IConvertible))
            {
                return false;
            }
            else if (_validateValueCallback != null
                && !_validateValueCallback(value))
            {
                return false;
            }
            return true;
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

            DependencyProperty property = new DependencyProperty(
                name,
                valueType,
                ownerType,
                defaultMetadata,
                validateValueCallback,
                isAttached);

            if (!property.ValidateValue(defaultMetadata.DefaultValue))
            {
                property.ThrowInvalidValueException(defaultMetadata.DefaultValue);
            }

            if (!_registeredProperties.Add(property))
            {
                throw new InvalidOperationException($"The property {ownerType}.{name} has been registered.");
            }

            defaultMetadata.Property = property;
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
                || _ownerType == ownerType.OriginalType
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

        internal void ThrowInvalidCoercedValueException(object value)
        {
            if (value is null)
            {
                if (_valueType.IsValueType
                    && !(_valueType.IsGenericType && _valueType.GetGenericTypeDefinition() == _nullableValueType))
                {
                    throw new InvalidOperationException($"The coreced value is null, but {_ownerType}.{_name} can not be null.");
                }
            }
            else if (value.IsUnsetValue())
            {
                throw new InvalidOperationException($"The coreced value \"DependencyProperty.UnsetValue\" is invalid for any properties.");
            }
            else if (!_valueType.IsInstanceOfType(value) && !(value is IConvertible))
            {
                throw new InvalidCastException($"Can not convert the coreced value \"{value}\" to {_valueType} for {_ownerType}.{_name}.");
            }
            else if (_validateValueCallback != null
                && !_validateValueCallback(value))
            {
                throw new ArgumentException($"The coreced value \"{value}\" is not valid for {_ownerType}.{_name}.");
            }
        }

        internal void ThrowInvalidValueException(object value)
        {
            if (value is null)
            {
                if (_valueType.IsValueType
                    && !(_valueType.IsGenericType && _valueType.GetGenericTypeDefinition() == _nullableValueType))
                {
                    throw new InvalidOperationException($"{_ownerType}.{_name} can not set to be null.");
                }
            }
            else if (value.IsUnsetValue())
            {
                throw new InvalidOperationException($"The value \"DependencyProperty.UnsetValue\" is invalid for any properties.");
            }
            else if (!_valueType.IsInstanceOfType(value) && !(value is IConvertible))
            {
                throw new InvalidCastException($"Can not convert \"{value}\" to {_valueType} for {_ownerType}.{_name}.");
            }
            else if (_validateValueCallback != null
                && !_validateValueCallback(value))
            {
                throw new ArgumentException($"The value \"{value}\" is not valid for {_ownerType}.{_name}.");
            }
        }

        #endregion


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
    }
}
