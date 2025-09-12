using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖属性标识
    /// </summary>
    public sealed class DependencyProperty : Singleton<DependencyProperty>
    {
        /// <summary>
        /// 依赖属性的无效值
        /// </summary>
        public static readonly object UnsetValue = new UnsetObject();


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 依赖属性的默认元数据
        /// </summary>
        public readonly PropertyMetadata DefaultMetadata;

        /// <summary>
        /// 依赖属性值的验证回调
        /// </summary>
        public readonly ValidateValueCallback ValidateValueCallback;

        /// <summary>
        /// 依赖属性值的类型
        /// </summary>
        public readonly Type ValueType;

        #endregion


        private DependencyProperty(string name, Type ownerType, Type valueType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback, bool isAttachedProperty = false)
            : base(name, ownerType)
        {
            DefaultMetadata = defaultMetadata;

            ValueType = valueType;
            ValidateValueCallback = validateValueCallback;

            if (isAttachedProperty)
            {
                _states |= Flags.IsAttachedProperty;
            }

            if (defaultMetadata.Inherits)
            {
                _states |= Flags.IsPotentiallyInherited;
            }

            if (valueType == typeof(string) || valueType.IsValueType)
            {
                _states |= Flags.IsStringOrStructType;
            }

            DefaultMetadata.ApplyTo(this);
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
                throw new ArgumentException("Dependency property name cannot be null or whitespace.", nameof(name));
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
                throw new ArgumentException("Dependency property name cannot be null or whitespace.", nameof(name));
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
        /// <remarks>当找不到符合条件的依赖属性时，返回 <see langword="null"/>。</remarks>
        public static DependencyProperty Search(string name, Type ownerType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Dependency property name must not be null or whitespace.", nameof(name));
            }

            if (ownerType is null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            if (!typeof(DependencyObject).IsAssignableFrom(ownerType))
            {
                throw new ArgumentException($"Owner type of a dependency property must be a subclass of '{typeof(DependencyObject).AsLog()}'.", nameof(ownerType));
            }

            var dType = DependencyObjectType.FromSystemTypeUnsafe(ownerType);

            while (dType != null)
            {
                if (TryGet(name, dType.Type, out var property))
                {
                    return property;
                }

                dType = dType.BaseType;
            }

            return null;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 重写依赖属性的元数据
        /// </summary>
        /// <param name="ownerType">拥有依赖属性的对象的类型</param>
        /// <param name="metadata">新的元数据</param>
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

            if ((_states & Flags.IsAttachedProperty) != 0)
            {
                throw new InvalidOperationException($"Unable to override the metadata since '{OwnerType.AsLog()}.{Name}' is an attached property.");
            }

            if (metadata.IsSealed)
            {
                throw new InvalidOperationException($"Unable to override the metadata of '{OwnerType.AsLog()}.{Name}' since the new metadata '{metadata.DefaultValue.AsLog()}' already applied to another dependency property.");
            }

            if (!ValidateValue(metadata.DefaultValue, out var exception))
            {
                throw new InvalidOperationException($"Unable to override the metadata of '{OwnerType.AsLog()}.{Name}' since the deafult value '{metadata.DefaultValue.AsLog()}' of type '{metadata.DefaultValue.GetType().AsLog()}' is not valid.", exception);
            }

            if (!OwnerType.IsAssignableFrom(ownerType))
            {
                throw new InvalidOperationException($"Unable to override the metadata of '{OwnerType.AsLog()}.{Name}' since the owner type '{ownerType.AsLog()} is not a subclass of '{OwnerType.AsLog()}'.");
            }

            var dType = DependencyObjectType.FromSystemTypeUnsafe(ownerType);
            var baseMetadata = this[dType];

            if (!baseMetadata.GetType().IsAssignableFrom(metadata.GetType()))
            {
                throw new InvalidOperationException($"Unable to override the metadata of '{OwnerType.AsLog()}.{Name}' since the new metadata '{metadata.DefaultValue.AsLog()}' is not compatible with the base metadata of type '{baseMetadata.GetType().AsLog()}'.");
            }

            metadata.ApplyTo(this, baseMetadata);

            if (metadata.Inherits)
            {
                _states |= Flags.IsPotentiallyInherited;
            }

            if ((_states & Flags.IsStringOrStructType) != 0)
            {
                if (!Equals(DefaultMetadata.DefaultValue, metadata.DefaultValue))
                {
                    _states |= Flags.IsDefaultValueChanged;
                }
            }
            else
            {
                if (!ReferenceEquals(DefaultMetadata.DefaultValue, metadata.DefaultValue))
                {
                    _states |= Flags.IsDefaultValueChanged;
                }
            }

            _metadataMaps.Clear();
            _metadataSets[dType] = metadata;
        }

        /// <summary>
        /// 验证要设置给依赖属性的值
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="exception">验证失败时的错误信息</param>
        /// <returns>若验证通过，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool ValidateValue(object value, out Exception exception)
        {
            return ValidateValueCore(ValueType, value, ValidateValueCallback, out exception);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Method
        //
        //------------------------------------------------------

        #region Private Static Method

        private static DependencyProperty RegisterCommon(string name, Type valueType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback, bool isAttachedProperty)
        {
            if (defaultMetadata.IsSealed)
            {
                throw new InvalidOperationException(
                    $"Faild to register dependency property '{ownerType.AsLog()}.{name}'",
                    new ArgumentException("Metadata has been sealed by another dependency property."));
            }

            if (ownerType.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException(
                    $"Faild to register dependency property '{ownerType.AsLog()}.{name}'",
                    new ArgumentException("Owner type of a dependency property cannot be a generic type definition."));
            }

            if (Exist(name, ownerType))
            {
                throw new InvalidOperationException(
                    $"Faild to register dependency property '{ownerType.AsLog()}.{name}'",
                    new ArgumentException($"Dependency property '{ownerType.AsLog()}.{name}' has already been registered."));
            }

            if (!ValidateValueCore(valueType, defaultMetadata.DefaultValue, validateValueCallback, out var exception))
            {
                throw new InvalidOperationException($"Faild to register dependency property '{ownerType.AsLog()}.{name}'.", exception);
            }

            return new DependencyProperty(name, ownerType, valueType, defaultMetadata, validateValueCallback, isAttachedProperty);
        }

        private static bool ValidateValueCore(Type valueType, object value, ValidateValueCallback validate, out Exception exception)
        {
            if (ReferenceEquals(UnsetValue, value))
            {
                exception = new InvalidOperationException("'UnsetValue' is not a valid dependency property value.");
                return false;
            }
            else if (value == null)
            {
                if (valueType.IsValueType && (!valueType.IsGenericType || valueType.GetGenericTypeDefinition() != typeof(Nullable<>)))
                {
                    exception = new InvalidOperationException($"Value cannot be null for value type '{valueType.AsLog()}'.");
                    return false;
                }
            }
            else if (!valueType.IsInstanceOfType(value))
            {
                exception = new InvalidCastException($"Value '{value.AsLog()}' of type '{value.GetType()}' is not compatible with type '{valueType.AsLog()}'.");
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


        private PropertyMetadata GetMetadata(DependencyObjectType dType)
        {
            while (dType != null)
            {
                if (_metadataSets.TryGetValue(dType, out var metadata))
                {
                    return metadata;
                }

                dType = dType.BaseType;
            }

            return DefaultMetadata;
        }


        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        internal bool IsPotentiallyInherited => (_states & Flags.IsPotentiallyInherited) != 0;

        internal PropertyMetadata this[DependencyObjectType index]
        {
            get
            {
                if (OwnerType == index.Type || (_states & Flags.IsDefaultValueChanged) is 0)
                {
                    return DefaultMetadata;
                }

                return _metadataMaps.GetOrAdd(index, GetMetadata);
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly ConcurrentDictionary<DependencyObjectType, PropertyMetadata> _metadataMaps = new ConcurrentDictionary<DependencyObjectType, PropertyMetadata>();
        private readonly Dictionary<DependencyObjectType, PropertyMetadata> _metadataSets = new Dictionary<DependencyObjectType, PropertyMetadata>();

        private Flags _states;

        #endregion


        [Flags]
        private enum Flags : byte
        {
            IsAttachedProperty = 0b0001,
            IsDefaultValueChanged = 0b0010,
            IsPotentiallyInherited = 0b0100,
            IsStringOrStructType = 0b1000
        }


        [DebuggerDisplay("DependencyProperty.UnsetObject")]
        private sealed class UnsetObject
        {
            public override int GetHashCode() => "DependencyProperty.UnsetObject".GetHashCode();

            public override string ToString() => "DependencyProperty.UnsetObject";
        }
    }
}
