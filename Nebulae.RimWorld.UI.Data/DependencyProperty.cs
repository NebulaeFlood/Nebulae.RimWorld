using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        private static readonly Type _nullableType = typeof(Nullable<>);

        /// <summary>
        /// 已注册的附加属性
        /// </summary>
        private static readonly HashSet<DependencyProperty> _registeredAttachedProperties = new HashSet<DependencyProperty>();

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
        /// 附加属性的元数据
        /// </summary>
        private readonly Dictionary<Type, PropertyMetadata> _attachedMetadata;

        /// <summary>
        /// 依赖属性的默认元数据
        /// </summary>
        private readonly PropertyMetadata _defaultMetadata;

        private readonly int _hashCode;

        /// <summary>
        /// 依赖属性的元数据是否已被重写
        /// </summary>
        private bool _isMetadataOverridden;

        /// <summary>
        /// 依赖属性的元数据
        /// </summary>
        private readonly Dictionary<Type, PropertyMetadata> _metadata;

        private readonly string _name;

        private readonly Type _ownerType;

        /// <summary>
        /// 属性验证回调函数
        /// </summary>
        private readonly ValidateValueCallback _validateValueCallback;

        private readonly Type _valueType;

        #endregion


        // 全部使用字段旨在大量读写属性时提供略微性能提升
        internal readonly bool _isAttached;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 指示该依赖属性是否为附加属性
        /// </summary>
        public bool IsAttached => _isAttached;

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
            _attachedMetadata = new Dictionary<Type, PropertyMetadata>();
            _metadata = new Dictionary<Type, PropertyMetadata>();
            _isMetadataOverridden = false;

            _name = name;
            _valueType = valueType;
            _ownerType = ownerType;
            _defaultMetadata = defaultMetadata;
            _validateValueCallback = validateValueCallback;
            _isAttached = isAttached;

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
        /// <param name="defaultValue">依赖属性的默认值</param>
        /// <param name="validateValueCallback">属性验证回调函数</param>
        /// <returns><see cref="DependencyProperty"/> 的新实例。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/>、<paramref name="valueType"/> 或 <paramref name="ownerType"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当目标依赖属性已被注册时发生。</exception>
        public static DependencyProperty Register(
            string name,
            Type valueType,
            Type ownerType,
            object defaultValue,
            ValidateValueCallback validateValueCallback = null)
        {
            return Register(name, valueType, ownerType, new PropertyMetadata(defaultValue), validateValueCallback);
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
            DependencyProperty property = new DependencyProperty(name, valueType, ownerType, defaultMetadata, validateValueCallback);
            if (!property.ValidateValue(defaultMetadata.DefaultValue))
            {
                property.ThrowInvalidValueException(defaultMetadata.DefaultValue);
            }

            if (!_registeredProperties.Add(property))
            {
                throw new InvalidOperationException($"The property {ownerType}.{name} has been registered.");
            }

            return property;
        }

        /// <summary>
        /// 注册附加属性
        /// </summary>
        /// <param name="name">附加属性名称</param>
        /// <param name="valueType">附加属性的值的类型</param>
        /// <param name="ownerType">拥有附加属性的对象的类型</param>
        /// <param name="defaultValue">附加属性的默认元数据</param>
        /// <param name="validateValueCallback">属性验证回调函数</param>
        /// <returns><see cref="DependencyProperty"/> 的新实例。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="name"/>、<paramref name="valueType"/> 或 <paramref name="ownerType"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当目标附加属性已被注册时发生。</exception>
        public static DependencyProperty RegisterAttached(
            string name,
            Type valueType,
            Type ownerType,
            object defaultValue,
            ValidateValueCallback validateValueCallback = null)
        {
            return RegisterAttached(name, valueType, ownerType, new PropertyMetadata(defaultValue), validateValueCallback);
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
            DependencyProperty property = new DependencyProperty(name, valueType, ownerType, defaultMetadata, validateValueCallback, true);
            property.ValidateValue(defaultMetadata.DefaultValue);

            if (!_registeredAttachedProperties.Add(property))
            {
                throw new InvalidOperationException($"The property {ownerType}.{name} has been registered.");
            }

            return property;
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
            if (obj is null) { return false; }
            if (obj is DependencyProperty dependencyProperty)
            {
                return ReferenceEquals(this, dependencyProperty)
                    || (_ownerType == dependencyProperty._ownerType
                        && _name == dependencyProperty._name);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// 判断依赖属性是否与指定依赖属性相等
        /// </summary>
        /// <param name="other">要判断的依赖属性</param>
        /// <returns>如果相等，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(DependencyProperty other)
        {
            if (other is null) { return false; }
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
        /// 重写依赖属性的元数据
        /// </summary>
        /// <param name="ownerType">拥有依赖属性的对象的类型</param>
        /// <param name="metadata">新的元数据</param>
        /// <returns>如果重写成功，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        /// <remarks>仅当该依赖属性不属于 <paramref name="ownerType"/> 时，才可能返回 <see langword="false"/>。</remarks>
        public bool OverrideMetadata(Type ownerType, PropertyMetadata metadata)
        {
            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            ValidateValue(metadata.DefaultValue);

            _isMetadataOverridden = true;
            if (_isAttached)
            {
                _attachedMetadata[ownerType] = metadata;
                return true;
            }
            else
            {
                if (ownerType == _ownerType || ownerType.IsSubclassOf(_ownerType))
                {
                    _metadata[ownerType] = metadata;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString() => _name;

        /// <summary>
        /// 验证要设置给依赖属性的值
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <returns>如果验证成功，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <see langword="null"/> 且 <see cref="ValueType"/> 不为可空类型时发生。</exception>
        /// <exception cref="ArgumentException">当 <paramref name="value"/> 无法转换为 <see cref="ValueType"/> 或无法通过 <see cref="ValidateValueCallback"/> 的验证时发生。</exception>
        public bool ValidateValue(object value)
        {
            if (value is null)
            {
                if (_valueType.IsValueType
                    && !(_valueType.IsGenericType && _valueType.GetGenericTypeDefinition() == _nullableType))
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

        /// <summary>
        /// 获取依赖属性的元数据
        /// </summary>
        /// <param name="ownerType">拥有依赖属性的对象的类型</param>
        /// <param name="metadata">依赖属性的元数据</param>
        /// <returns>如果成功获取，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        internal bool TryGetMetadata(Type ownerType, out PropertyMetadata metadata)
        {
            if (!_isMetadataOverridden)
            {
                if (_isAttached || _ownerType.IsAssignableFrom(ownerType))
                {
                    metadata = _defaultMetadata;
                    return true;
                }
                metadata = null;
                return false;
            }
            return _isAttached
                ? _attachedMetadata.TryGetValue(ownerType, out metadata)
                : _metadata.TryGetValue(ownerType, out metadata);
        }

        internal void ThrowInvalidCoercedValueException(object value)
        {
            if (value is null)
            {
                if (_valueType.IsValueType
                    && !(_valueType.IsGenericType && _valueType.GetGenericTypeDefinition() == _nullableType))
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
                    && !(_valueType.IsGenericType && _valueType.GetGenericTypeDefinition() == _nullableType))
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
            private string _value;
        }
    }
}
