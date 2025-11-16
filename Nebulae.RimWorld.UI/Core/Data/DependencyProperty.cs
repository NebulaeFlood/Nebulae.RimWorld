using Nebulae.RimWorld.UI.Core.Data.Expressions;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

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
                _states |= StateFlags.IsAttachedProperty;
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

        /// <summary>
        /// 尝试在指定类型搜索依赖属性标识
        /// </summary>
        /// <param name="name">依赖属性的名称</param>
        /// <param name="ownerType">拥有依赖属性的类型</param>
        /// <param name="property">依赖属性标识</param>
        /// <returns>若 <paramref name="ownerType"/> 或其基类注册了名为 <paramref name="name"/> 的依赖属性，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool TrySearch(string name, Type ownerType, out DependencyProperty property)
        {
            if (string.IsNullOrWhiteSpace(name) || ownerType is null || !typeof(DependencyObject).IsAssignableFrom(ownerType))
            {
                property = null;
                return false;
            }

            var dType = DependencyObjectType.FromSystemTypeUnsafe(ownerType);

            while (dType != null)
            {
                if (TryGet(name, dType.Type, out property))
                {
                    return true;
                }

                dType = dType.BaseType;
            }

            property = null;
            return false;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 获取保存依赖属性标识符的字段
        /// </summary>
        /// <returns>用于保存依赖属性标识符的字段的 <see cref="FieldInfo"/>。</returns>
        public FieldInfo GetIdentifier()
        {
            return IdentifierMaps.GetOrAdd(this, GetIdentifierCore);
        }

        /// <summary>
        /// 获取依赖属性的元数据
        /// </summary>
        /// <param name="owner">拥有此依赖属性的元数据的类型的依赖对象实例</param>
        /// <returns>该依赖属性对应 <paramref name="owner"/> 的类型的元数据。</returns>
        public PropertyMetadata GetMetadata(DependencyObject owner)
        {
            return this[owner.DependencyObjectType];
        }

        /// <summary>
        /// 获取依赖属性的元数据
        /// </summary>
        /// <param name="ownerType">拥有此依赖属性的元数据的类型</param>
        /// <returns>该依赖属性对应 <paramref name="ownerType"/> 的元数据。</returns>
        public PropertyMetadata GetMetadata(Type ownerType)
        {
            return this[DependencyObjectType.FromSystemType(ownerType)];
        }

        /// <summary>
        /// 获取依赖属性的元数据
        /// </summary>
        /// <param name="ownerType">拥有此依赖属性的元数据的 <see cref="DependencyObjectType"/></param>
        /// <returns>该依赖属性对应 <paramref name="ownerType"/> 的元数据。</returns>
        public PropertyMetadata GetMetadata(DependencyObjectType ownerType)
        {
            if (ownerType is null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            return this[ownerType];
        }

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

            try
            {
                if ((_states & StateFlags.IsAttachedProperty) != 0)
                {
                    throw new InvalidOperationException($"'{this}' is an attached property.");
                }

                if (metadata.IsSealed)
                {
                    throw new ArgumentException($"The metadata '{metadata.GetType().AsLog()}' with value '{metadata.DefaultValue.AsLog()}' already applied to another dependency property.");
                }

                object defaultValue = metadata.DefaultValue;

                if (defaultValue is Expression)
                {
                    throw new ArgumentException("Default value of a dependency property cannot be an Expression.");
                }

                try
                {
                    ValidateValueCore(ValueType, defaultValue, ValidateValueCallback);
                }
                catch (Exception)
                {
                    if (defaultValue is null)
                    {
                        throw new ArgumentException("The default value of a dependency property cannot be null.");
                    }
                    else
                    {
                        throw new ArgumentException($"The default value '{defaultValue.AsLog()}' of type '{defaultValue.GetType().AsLog()}' is not valid.");
                    }
                }

                if (!ownerType.IsSubclassOf(OwnerType))
                {
                    throw new ArgumentException($"The owner type '{ownerType.AsLog()}' is not a subclass of '{OwnerType.AsLog()}'.");
                }

                var dType = DependencyObjectType.FromSystemTypeUnsafe(ownerType);
                var baseMetadata = this[dType];

                if (!baseMetadata.GetType().IsAssignableFrom(metadata.GetType()))
                {
                    throw new ArgumentException($"The metadata '{metadata.GetType().AsLog()}' is not compatible with the base metadata '{baseMetadata.GetType().AsLog()}'.");
                }

                metadata.ApplyTo(this, baseMetadata);

                _states |= StateFlags.IsPotentiallyInherited;

                _metadataMaps.Clear();
                _metadataSets[dType] = metadata;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot override the metadata of dependency property '{this}'.", e);
            }
        }

        /// <summary>
        /// 验证要设置给依赖属性的值
        /// </summary>
        /// <param name="value">要验证的值</param>
        public void ValidateValue(object value)
        {
            ValidateValueCore(ValueType, value, ValidateValueCallback);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Constructor
        //
        //------------------------------------------------------

        #region Private Static Method

        private static FieldInfo GetIdentifierCore(DependencyProperty property)
        {
            var field = property.OwnerType.GetField(property.Name + "Property", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (field is null)
            {
                throw new InvalidOperationException($"Cannot find the identifier field of dependency property '{property}', make sure the identifier field is named as '{property.Name}Property'.");
            }

            return field;
        }

        private static DependencyProperty RegisterCommon(string name, Type valueType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback, bool isAttachedProperty)
        {
            try
            {
                if (defaultMetadata.IsSealed)
                {
                    throw new ArgumentException("Metadata has been sealed by another dependency property.");
                }

                if (ownerType.IsGenericTypeDefinition)
                {
                    throw new ArgumentException("Owner type of a dependency property cannot be a generic type definition.");
                }

                if (!typeof(DependencyObject).IsAssignableFrom(ownerType))
                {
                    throw new ArgumentException($"Owner type of a dependency property must be a subclass of '{typeof(DependencyObject).AsLog()}'.");
                }

                if (Exist(name, ownerType))
                {
                    throw new ArgumentException($"Dependency property '{ownerType.AsLog()}.{name}' has already been registered.");
                }

                object defaultValue = defaultMetadata.DefaultValue;

                if (defaultValue is Expression)
                {
                    throw new ArgumentException("Default value of a dependency property cannot be an Expression.");
                }

                try
                {
                    ValidateValueCore(valueType, defaultValue, validateValueCallback);
                }
                catch (Exception e)
                {
                    if (defaultValue is null)
                    {
                        throw new ArgumentException("The default value of a dependency property cannot be null.", e);
                    }
                    else
                    {
                        throw new ArgumentException($"The default value '{defaultValue.AsLog()}' of type '{defaultValue.GetType().AsLog()}' is not valid.", e);
                    }
                }

                return new DependencyProperty(name, ownerType, valueType, defaultMetadata, validateValueCallback, isAttachedProperty);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot register new dependency property '{ownerType.AsLog()}.{name}'.", e);
            }
        }

        private static void ValidateValueCore(Type valueType, object value, ValidateValueCallback validateValueCallback)
        {
            if (UnsetValue == value)
            {
                throw new InvalidOperationException($"'{UnsetValue}' is not a valid dependency property value.");
            }

            if (value == null)
            {
                if (valueType.IsValueType)
                {
                    throw new InvalidOperationException($"Value of type '{value.GetType()}' cannot be null.");
                }
            }

            if (!valueType.IsInstanceOfType(value))
            {
                throw new InvalidCastException($"Value '{value.AsLog()}' of type '{value.GetType()}' is not compatible with type '{valueType.AsLog()}'.");
            }

            if (validateValueCallback != null && !validateValueCallback.Invoke(value))
            {
                throw new ArgumentException($"Value '{value.AsLog()}' cannot pass the validation callback from '{validateValueCallback.AsLog()}'.");
            }
        }

        #endregion


        private PropertyMetadata GetMetadataCore(DependencyObjectType dType)
        {
            // 此处保证 dType 不为 null
            do
            {
                if (_metadataSets.TryGetValue(dType, out var metadata))
                {
                    return metadata;
                }

                dType = dType.BaseType;
            }
            while (dType != null);

            return DefaultMetadata;
        }


        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        internal bool IsAttachedProperty => (_states & StateFlags.IsAttachedProperty) != 0;

        internal bool IsDefaultMetadata
        {
            get
            {
                return (_states & StateFlags.IsAttachedProperty) != 0
                    || (_states & StateFlags.IsPotentiallyInherited) == 0;
            }
        }

        internal bool IsPotentiallyInherited => (_states & StateFlags.IsPotentiallyInherited) != 0;

        internal PropertyMetadata this[DependencyObjectType index]
        {
            get
            {
                if (index.Type == OwnerType || IsDefaultMetadata)
                {
                    return DefaultMetadata;
                }

                if (_metadataSets.TryGetValue(index, out PropertyMetadata metadata))
                {
                    return metadata;
                }

                metadata = GetMetadataCore(index);
                _metadataSets[index] = metadata;

                return metadata;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static readonly ConcurrentDictionary<DependencyProperty, FieldInfo> IdentifierMaps = new ConcurrentDictionary<DependencyProperty, FieldInfo>();
        private static readonly Func<DependencyProperty, FieldInfo> IdentifierFinder = GetIdentifierCore;

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Dictionary<DependencyObjectType, PropertyMetadata> _metadataMaps = new Dictionary<DependencyObjectType, PropertyMetadata>();
        private readonly Dictionary<DependencyObjectType, PropertyMetadata> _metadataSets = new Dictionary<DependencyObjectType, PropertyMetadata>();

        private StateFlags _states;

        #endregion


        [Flags]
        private enum StateFlags : byte
        {
            IsAttachedProperty = 0b01,
            IsPotentiallyInherited = 0b10
        }


        [DebuggerDisplay(UnsetObjectString)]
        private sealed class UnsetObject
        {
            private const string UnsetObjectString = "DependencyProperty.UnsetObject";


            public override int GetHashCode() => UnsetObjectString.GetHashCode();

            public override string ToString() => UnsetObjectString;
        }
    }
}
