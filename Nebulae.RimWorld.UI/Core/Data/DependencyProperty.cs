using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖属性的标识
    /// </summary>
    [DebuggerStepThrough]
    public sealed class DependencyProperty : Singleton<DependencyProperty>
    {
        /// <summary>
        /// 依赖属性的无效值
        /// </summary>
        public static readonly object UnsetValue = new UnsetObject();


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 依赖属性的默认元数据
        /// </summary>
        public PropertyMetadata DefaultMetadata => _defaultMetadata;

        /// <summary>
        /// 依赖属性的值的验证回调
        /// </summary>
        public ValidateValueCallback ValidateValueCallback => _validateValueCallback;

        /// <summary>
        /// 依赖属性的值的类型
        /// </summary>
        public Type ValueType => _valueType;

        #endregion


        private DependencyProperty(string name, Type ownerType, Type valueType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback, bool isAttached = false)
            : base(name, ownerType)
        {
            _defaultMetadata = defaultMetadata;
            _isAttached = isAttached;
            _valueType = valueType;
            _validateValueCallback = validateValueCallback;
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
        /// <exception cref="ArgumentException">当 <paramref name="name"/> 为空时发生。</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="valueType"/> 或 <paramref name="ownerType"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当目标依赖属性已被注册时发生。</exception>
        public static DependencyProperty Register(string name, Type valueType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Property Name cannot be null or whitespace.", nameof(name));
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
                throw new ArgumentNullException(nameof(defaultMetadata));
            }

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
        /// <exception cref="ArgumentException">当 <paramref name="name"/> 为空时发生。</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="valueType"/> 或 <paramref name="ownerType"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当目标附加属性已被注册时发生。</exception>
        public static DependencyProperty RegisterAttached(string name, Type valueType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Property Name cannot be null or whitespace.", nameof(name));
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
                throw new ArgumentNullException(nameof(defaultMetadata));
            }

            return RegisterCommon(name, valueType, ownerType, defaultMetadata, validateValueCallback, true);
        }


        /// <summary>
        /// 搜索依赖属性标识
        /// </summary>
        /// <param name="name">依赖属性的名称</param>
        /// <param name="ownerType">拥有依赖属性的类型</param>
        /// <returns>对应的依赖属性标识。</returns>
        /// <exception cref="MissingMemberException">当无法在 <paramref name="ownerType"/> 中找到名为 <paramref name="name"/> 的依赖属性时发生。</exception>
        public static DependencyProperty Search(string name, Type ownerType)
        {
            while (DependencyObjectType.RootType.IsAssignableFrom(ownerType))
            {
                if (TryGetSingleton(name, ownerType, out var property))
                {
                    return property;
                }

                ownerType = ownerType.BaseType;
            }

            throw new MissingMemberException($"Cannot find any property named '{name}' in '{ownerType}'.");
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

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
                throw new InvalidOperationException($"Unable to override the _metadata of the attached property {OwnerType}.{Name}.");
            }

            if (metadata.isSealed)
            {
                throw new InvalidOperationException("Metadata has been sealed by another dependency property.");
            }

            if (!ValidateValue(metadata.defaultValue, out var exception))
            {
                throw new InvalidOperationException($"Deafult value '{metadata.defaultValue.AsLog()}' of type '{metadata.defaultValue.GetType()}' is not valid for '{OwnerType}.{Name}'.", exception);
            }

            if (!OwnerType.IsAssignableFrom(ownerType))
            {
                throw new InvalidOperationException($"The type {ownerType} must inherit {OwnerType} to override the _metadata of {OwnerType}.{Name}.");
            }

            _isMetadataOverridden = true;

            var dType = DependencyObjectType.From(ownerType);
            metadata.Merge(GetMetadata(dType));

            metadata.isSealed = true;
            _metadatas[dType] = metadata;

            _metadataMap.Clear();
        }

        /// <summary>
        /// 返回依赖属性的名称
        /// </summary>
        /// <returns>依赖属性的名称。</returns>
        public override string ToString() => Name;

        /// <summary>
        /// 验证要设置给依赖属性的值
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="exception">验证失败时的错误信息</param>
        /// <returns>如果验证成功，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool ValidateValue(object value, out Exception exception)
        {
            return ValidateValueCore(_valueType, value, _validateValueCallback, out exception);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal PropertyMetadata GetMetadata(DependencyObjectType dType)
        {
            if (_isAttached || !_isMetadataOverridden || OwnerType == dType.Type)
            {
                return _defaultMetadata;
            }

            if (_metadataMap.TryGetValue(dType, out var metadata))
            {
                return metadata;
            }

            metadata = GetMetadataInHierarchy(dType);
            _metadataMap[dType] = metadata;

            return metadata;
        }

        internal PropertyMetadata GetMetadataInHierarchy(DependencyObjectType dType)
        {
            while (dType != null)
            {
                if (_metadatas.TryGetValue(dType, out var metadata))
                {
                    return metadata;
                }

                dType = dType.Base;
            }

            return _defaultMetadata;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Method
        //
        //------------------------------------------------------

        #region Private Static Method

        private static DependencyProperty RegisterCommon(string name, Type valueType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback = null, bool isAttached = false)
        {
            if (defaultMetadata.isSealed)
            {
                throw new InvalidOperationException("Metadata has been sealed by another dependency property.");
            }

            if (Exist(name, ownerType))
            {
                throw new InvalidOperationException($"Property '{ownerType}.{name}' has already been registered.");
            }

            if (!ValidateValueCore(valueType, defaultMetadata.defaultValue, validateValueCallback, out var exception))
            {
                throw new InvalidOperationException($"Faild to register property '{ownerType}.{name}'", exception);
            }

            return new DependencyProperty(name, ownerType, valueType, defaultMetadata, validateValueCallback, isAttached);
        }

        private static bool ValidateValueCore(Type valueType, object value, ValidateValueCallback validate, out Exception exception)
        {
            if (ReferenceEquals(UnsetValue, value))
            {
                exception = new InvalidOperationException("'UnsetValue' is not a valid property value.");
                return false;
            }
            else if (value is null)
            {
                if (valueType.IsValueType && (!valueType.IsGenericType || valueType.GetGenericTypeDefinition() != NullableValueType))
                {
                    exception = new InvalidOperationException($"Value cannot be null for value type '{valueType}'.");
                    return false;
                }
            }
            else if (!valueType.IsInstanceOfType(value))
            {
                exception = new InvalidCastException($"Value '{value.AsLog()}' of type '{value.GetType()}' is not compatible with type '{valueType}'.");
                return false;
            }
            else if (validate != null && !validate(value))
            {
                exception = new ArgumentException($"Value '{value.AsLog()}' is not a valid value.");
                return false;
            }

            exception = null;
            return true;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly Type NullableValueType = typeof(Nullable<>);

        private readonly PropertyMetadata _defaultMetadata;
        private readonly Dictionary<DependencyObjectType, PropertyMetadata> _metadatas = new Dictionary<DependencyObjectType, PropertyMetadata>();
        private readonly Dictionary<DependencyObjectType, PropertyMetadata> _metadataMap = new Dictionary<DependencyObjectType, PropertyMetadata>();
        private readonly ValidateValueCallback _validateValueCallback;

        private readonly bool _isAttached;

        private readonly Type _valueType;

        private bool _isMetadataOverridden;

        #endregion



        [DebuggerDisplay("DependencyProperty.UnsetObject")]
        private class UnsetObject
        {
            public override int GetHashCode() => "DependencyProperty.UnsetObject".GetHashCode();

            public override string ToString() => "DependencyProperty.UnsetObject";
        }
    }
}
