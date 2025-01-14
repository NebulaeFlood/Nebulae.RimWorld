using Nebulae.RimWorld.UI.Data.Binding;
using System;
using System.Collections.Generic;

namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 依赖对象
    /// </summary>
    public abstract class DependencyObject
    {
        #region DependencyPropertyChanged

        private readonly BindingDependencyPropertyChangedEvent _dependencyPropertyChanged =
            new BindingDependencyPropertyChangedEvent();

        /// <summary>
        /// 提供给 <see cref="BindingBase"/> 的当依赖属性值发生变化时触发的事件
        /// </summary>
        internal event PropertyChangedCallback DependencyPropertyChanged
        {
            add => _dependencyPropertyChanged.Add(value, value.Invoke);
            remove => _dependencyPropertyChanged.Remove(value);
        }

        #endregion

        /// <summary>
        /// 依赖对象当前的类型
        /// </summary>
        public readonly Type Type;

        internal readonly DependencyObjectType DependencyType;

        private readonly Dictionary<DependencyProperty, EffectiveValueEntry> _effectiveValues = new Dictionary<DependencyProperty, EffectiveValueEntry>();


        /// <summary>
        /// 为 <see cref="DependencyObject"/> 派生类实现基本初始化
        /// </summary>
        protected DependencyObject()
        {
            Type = GetType();
            DependencyType = DependencyObjectType.FromType(Type);
        }


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

                if (CompareAndUpdate(oldEntry.TemporaryValue, oldEntry.Value, ForceValueStatus.Common, out var newEntry))
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
        /// <exception cref="ArgumentNullException">当 <paramref name="property"/> 为 <see langword="null"/> 时发生。</exception>
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

            if (!property.ValidateValue(value))
            {
                value = CoerceValue(property, value);
            }
            SetValueCommon(property, value, ForceValueStatus.Common);
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

            if (!property.ValidateValue(value))
            {
                value = CoerceValue(property, value);
            }
            SetValueCommon(property, value, ForceValueStatus.Temporary);
        }

        #endregion


        /// <summary>
        /// 当前对象的依赖属性的值发生变化时执行的方法
        /// </summary>
        /// <param name="args">有关属性更改的数据</param>
        protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
        }


        internal void SetValueIntelligently(DependencyProperty property, object newValue)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (ReferenceEquals(DependencyProperty.UnsetValue, newValue))
            {
                return;
            }

            if (!property.ValidateValue(newValue))
            {
                newValue = CoerceValue(property, newValue);
            }

            DependencyPropertyChangedEventArgs args;
            PropertyMetadata metadata;
            EffectiveValueEntry newEntry;

            if (_effectiveValues.TryGetValue(property, out EffectiveValueEntry oldEntry))
            {
                if (CompareAndUpdate(
                        oldEntry,
                        newValue,
                        ForceValueStatus.Keep,
                        out newEntry))
                {
                    return;
                }

                _effectiveValues[property] = newEntry;

                metadata = property.GetMetadata(DependencyType);
                args = new DependencyPropertyChangedEventArgs(
                    property,
                    metadata,
                    newEntry);

                metadata.PropertyChangedCallback?.Invoke(this, args);
                OnPropertyChanged(args);
                _dependencyPropertyChanged.Invoke(this, args);
            }
            else
            {
                metadata = property.GetMetadata(DependencyType);

                if (CompareAndUpdate(
                        metadata.DefaultValue,
                        newValue,
                        ForceValueStatus.Keep,
                        out newEntry))
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


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private object CoerceValue(DependencyProperty property, object value)
        {
            PropertyMetadata metadata = property.GetMetadata(DependencyType);

            object coercedValue = metadata.CoerceValueCallback?.Invoke(this, value) ?? value;

            if (!property.ValidateValue(coercedValue))
            {
                property.ThrowInvalidCoercedValueException(coercedValue);
            }

            return coercedValue;
        }

        private object GetValueCommon(DependencyProperty property)
        {
            if (_effectiveValues.TryGetValue(property, out EffectiveValueEntry valueEntry))
            {
                return valueEntry.IsTemporary
                    ? valueEntry.TemporaryValue
                    : valueEntry.Value;
            }

            return property.GetMetadata(DependencyType).DefaultValue;
        }

        private void SetValueCommon(DependencyProperty property, object newValue, ForceValueStatus valueStatus)
        {
            DependencyPropertyChangedEventArgs args;
            PropertyMetadata metadata;
            EffectiveValueEntry newEntry;

            if (_effectiveValues.TryGetValue(property, out EffectiveValueEntry oldEntry))
            {
                if (CompareAndUpdate(
                        oldEntry,
                        newValue,
                        valueStatus,
                        out newEntry))
                {
                    return;
                }

                metadata = property.GetMetadata(DependencyType);
            }
            else
            {
                metadata = property.GetMetadata(DependencyType);

                if (CompareAndUpdate(
                        metadata.DefaultValue,
                        newValue,
                        valueStatus,
                        out newEntry))
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

            args.Metadata.PropertyChangedCallback?.Invoke(this, args);

            OnPropertyChanged(args);

            _dependencyPropertyChanged.Invoke(this, args);
        }

        private static bool CompareAndUpdate(EffectiveValueEntry entry, object newValue, ForceValueStatus forceStatus, out EffectiveValueEntry newEntry)
        {
            object entryValue;

            if (forceStatus is ForceValueStatus.Common)
            {

                if (entry.IsTemporary)
                {
                    entryValue = entry.TemporaryValue;
                }
                else
                {
                    entryValue = entry.Value;
                }

                newEntry = new EffectiveValueEntry(newValue);

            }
            else if (forceStatus is ForceValueStatus.Keep)
            {

                if (entry.IsTemporary)
                {
                    entryValue = entry.TemporaryValue;
                    newEntry = new EffectiveValueEntry(entry.Value, newValue);
                }
                else
                {
                    entryValue = entry.Value;
                    newEntry = new EffectiveValueEntry(newValue);
                }

            }
            else
            {

                if (entry.IsTemporary)
                {
                    entryValue = entry.TemporaryValue;
                }
                else
                {
                    entryValue = entry.Value;
                }

                newEntry = new EffectiveValueEntry(entry.Value, newValue);
            }

            return entryValue?.Equals(newValue) ?? newValue is null;
        }

        private static bool CompareAndUpdate(object oldValue, object newValue, ForceValueStatus forceStatus, out EffectiveValueEntry newEntry)
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

        #endregion


        private enum ForceValueStatus
        {
            Common,
            Keep,
            Temporary
        }
    }
}
