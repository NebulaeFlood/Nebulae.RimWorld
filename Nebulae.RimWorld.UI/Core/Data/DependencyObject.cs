using Nebulae.RimWorld.UI.Automation.Diagnostics;
using Nebulae.RimWorld.UI.Core.Data.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖对象
    /// </summary>
    [DebuggerStepThrough]
    public abstract class DependencyObject
    {
        //------------------------------------------------------
        //
        //  Construstor
        //
        //------------------------------------------------------

        #region Construstor

        /// <summary>
        /// 为 <see cref="DependencyObject"/> 派生类实现基本初始化
        /// </summary>
        protected DependencyObject()
        {
            Type = GetType();
            DependencyType = DependencyObjectType.From(Type);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 获取依赖属性的值
        /// </summary>
        /// <param name="property">要获取值的依赖属性</param>
        /// <returns>依赖属性的值</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="property"/> 为 <see langword="null"/> 时发生。</exception>
        public object GetValue(DependencyProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return GetValueCommon(property);
        }

        /// <summary>
        /// 恢复依赖属性到临时状态前的值
        /// </summary>
        /// <param name="property">要恢复的依赖属性</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="property"/> 为 <see langword="null"/> 时发生。</exception>
        public void RestoreValue(DependencyProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (_effectiveValues.TryGetValue(property, out EffectiveValueEntry oldEntry))
            {
                if (!oldEntry.IsTemporary)
                {
                    return;
                }

                if (Compare(oldEntry.TemporaryValue, oldEntry.Value, ForceValueStatus.Common, out var newEntry))
                {
                    return;
                }

                var args = new DependencyPropertyChangedEventArgs(
                    property,
                    property.GetMetadata(DependencyType),
                    oldEntry,
                    newEntry);

                SetValueStraightly(args);
            }
        }

        /// <summary>
        /// 设置依赖属性的值
        /// </summary>
        /// <param name="property">要设置值的依赖属性</param>
        /// <param name="value">要设置的值</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="property"/> 为 <see langword="null"/> 时发生。</exception>、
        /// <remarks>将移除依赖属性的临时状态。</remarks>
        public void SetValue(DependencyProperty property, object value)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (ReferenceEquals(DependencyProperty.UnsetValue, value))
            {
                return;
            }

            value = ResolveValue(property, value, out var metadata);

            SetValueCommon(property, metadata, value, ForceValueStatus.Common);
        }

        /// <summary>
        /// 临时地设置依赖属性的值
        /// </summary>
        /// <param name="property">要设置值的依赖属性</param>
        /// <param name="value">要设置的值</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="property"/> 为 <see langword="null"/> 时发生。</exception>
        public void SetValueTemporarily(DependencyProperty property, object value)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (ReferenceEquals(DependencyProperty.UnsetValue, value))
            {
                return;
            }

            value = ResolveValue(property, value, out var metadata);

            SetValueCommon(property, metadata, value, ForceValueStatus.Temporary);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal void SetValueByBinding(DependencyProperty property, object value)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (ReferenceEquals(DependencyProperty.UnsetValue, value))
            {
                return;
            }

            value = ResolveValue(property, value, out var metadata);

            DependencyPropertyChangedEventArgs args;
            EffectiveValueEntry newEntry;

            if (_effectiveValues.TryGetValue(property, out EffectiveValueEntry oldEntry))
            {
                if (Compare(oldEntry, value, ForceValueStatus.Keep, out newEntry))
                {
                    return;
                }
            }
            else if (Compare(metadata.defaultValue, value, ForceValueStatus.Keep, out newEntry))
            {
                return;
            }

            args = new DependencyPropertyChangedEventArgs(
                property,
                metadata,
                newEntry);

            SetValueStraightly(args);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Method
        //
        //------------------------------------------------------

        #region Protected Method

        /// <summary>
        /// 当前对象的依赖属性的值发生变化时执行的方法
        /// </summary>
        /// <param name="args">有关属性更改的数据</param>
        protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs args) { }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private static bool Compare(EffectiveValueEntry oldEntry, object newValue, ForceValueStatus forceStatus, out EffectiveValueEntry newEntry)
        {
            object oldValue;

            if (forceStatus is ForceValueStatus.Common)
            {
                oldValue = oldEntry.IsTemporary ? oldEntry.TemporaryValue : oldEntry.Value;
                newEntry = new EffectiveValueEntry(newValue);

            }
            else if (forceStatus is ForceValueStatus.Keep)
            {

                if (oldEntry.IsTemporary)
                {
                    oldValue = oldEntry.TemporaryValue;
                    newEntry = new EffectiveValueEntry(oldEntry.Value, newValue);
                }
                else
                {
                    oldValue = oldEntry.Value;
                    newEntry = new EffectiveValueEntry(newValue);
                }

            }
            else
            {
                oldValue = oldEntry.IsTemporary ? oldEntry.TemporaryValue : oldEntry.Value;
                newEntry = new EffectiveValueEntry(oldEntry.Value, newValue);
            }

            return oldValue?.Equals(newValue) ?? newValue is null;
        }

        private static bool Compare(object oldValue, object newValue, ForceValueStatus forceStatus, out EffectiveValueEntry newEntry)
        {
            if (forceStatus is ForceValueStatus.Temporary)
            {
                newEntry = new EffectiveValueEntry(oldValue, newValue);
            }
            else
            {
                newEntry = new EffectiveValueEntry(newValue);
            }

            return oldValue?.Equals(newValue) ?? newValue is null;
        }

        private object GetValueCommon(DependencyProperty property)
        {
            if (_effectiveValues.TryGetValue(property, out EffectiveValueEntry valueEntry))
            {
                return valueEntry.IsTemporary ? valueEntry.TemporaryValue : valueEntry.Value;
            }

            return property.GetMetadata(DependencyType).defaultValue;
        }

        private object ResolveValue(DependencyProperty property, object value, out PropertyMetadata metadata)
        {
            if (!property.ValidateValue(value, out var exception))
            {
                throw exception;
            }

            metadata = property.GetMetadata(DependencyType);

            if (metadata.coerceValueCallback is null)
            {
                return value;
            }

            object coercedValue = metadata.coerceValueCallback.Invoke(this, value);

            if (!coercedValue.Equals(value) && !property.ValidateValue(coercedValue, out exception))
            {
                throw new InvalidOperationException($"Coerced value \"{coercedValue}\" is not valid for {property.OwnerType}.{property.Name}.", exception);
            }

            return coercedValue;
        }

        private void SetValueCommon(DependencyProperty property, PropertyMetadata metadata, object newValue, ForceValueStatus valueStatus)
        {
            DependencyPropertyChangedEventArgs args;
            EffectiveValueEntry newEntry;

            if (_effectiveValues.TryGetValue(property, out EffectiveValueEntry oldEntry))
            {
                if (Compare(oldEntry, newValue, valueStatus, out newEntry))
                {
                    return;
                }
            }
            else
            {
                if (Compare(metadata.defaultValue, newValue, valueStatus, out newEntry))
                {
                    return;
                }
            }

            args = new DependencyPropertyChangedEventArgs(
                property,
                metadata,
                newEntry);

            SetValueStraightly(args);
        }

        private void SetValueStraightly(DependencyPropertyChangedEventArgs args)
        {
            _effectiveValues[args.Property] = args.NewEntry;

            args.Metadata.propertyChangedCallback?.Invoke(this, args);

            OnPropertyChanged(args);
            PropertyBindings.Update(this, args);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Fields
        //
        //------------------------------------------------------

        #region Fields

        /// <summary>
        /// 依赖对象当前的类型
        /// </summary>
        [DebugMember(int.MinValue)]
        public readonly Type Type;


        internal readonly DependencyObjectType DependencyType;
        internal readonly DependencyPropertyBindings PropertyBindings = new DependencyPropertyBindings();


        private readonly Dictionary<DependencyProperty, EffectiveValueEntry> _effectiveValues = new Dictionary<DependencyProperty, EffectiveValueEntry>();

        #endregion


        private enum ForceValueStatus
        {
            Common,
            Keep,
            Temporary
        }
    }
}
