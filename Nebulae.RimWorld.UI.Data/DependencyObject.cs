using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 依赖对象
    /// </summary>
    public abstract class DependencyObject
    {
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
                if (oldEntry.Value != oldEntry.TemporaryValue)
                {
                    EffectiveValueEntry newEntry = new EffectiveValueEntry(oldEntry.Value);
                    _effectiveValues[property] = newEntry;
                    property.GetMetadata(DependencyType).NotifyPropertyChanged(this, oldEntry, newEntry);
                }
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
            if (ReferenceEquals(DependencyProperty.UnsetValue, value))
            {
                return;
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (!property.ValidateValue(value))
            {
                value = CoerceValue(property, value);
            }
            SetValueCommon(property, value, isTemporary: false);
        }

        /// <summary>
        /// 临时地设置依赖属性的值
        /// </summary>
        /// <param name="property">要设置值的依赖属性</param>
        /// <param name="value">要设置的值</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="property"/> 为 <see langword="null"/> 时发生。</exception>
        public void SetValueTemporarily(DependencyProperty property, object value)
        {
            if (ReferenceEquals(DependencyProperty.UnsetValue, value))
            {
                return;
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (!property.ValidateValue(value))
            {
                value = CoerceValue(property, value);
            }
            SetValueCommon(property, value, isTemporary: true);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private object CoerceValue(DependencyProperty property, object value)
        {
            PropertyMetadata metadata = property.GetMetadata(DependencyType);

            object coercedValue = metadata.CoerceValue(this, value);
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

        private void SetValueCommon(DependencyProperty property, object value, bool isTemporary = false)
        {
            object oldValue;
            if (_effectiveValues.TryGetValue(property, out EffectiveValueEntry oldEntry))
            {
                oldValue = oldEntry.IsTemporary ? oldEntry.TemporaryValue : oldEntry.Value;

                if (oldValue == value) { return; }

                EffectiveValueEntry newEntry = isTemporary
                    ? new EffectiveValueEntry(oldValue, value)
                    : new EffectiveValueEntry(value);

                _effectiveValues[property] = newEntry;

                property.GetMetadata(DependencyType).NotifyPropertyChanged(this, oldEntry, newEntry);
            }
            else
            {
                PropertyMetadata metadata = property.GetMetadata(DependencyType);
                oldValue = metadata.DefaultValue;

                if (oldValue == value) { return; }

                EffectiveValueEntry newEntry = isTemporary
                    ? new EffectiveValueEntry(oldValue, value)
                    : new EffectiveValueEntry(value);

                _effectiveValues[property] = newEntry;

                property.GetMetadata(DependencyType).NotifyPropertyChanged(this, newEntry);
            }
        }

        #endregion
    }
}
