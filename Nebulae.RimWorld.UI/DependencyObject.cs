using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 依赖对象
    /// </summary>
    public abstract class DependencyObject
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Type _currentType;
        private readonly Dictionary<DependencyProperty, EffectiveValueEntry> _effectiveAttachedValues;
        private readonly Dictionary<DependencyProperty, EffectiveValueEntry> _effectiveValues;

        #endregion


        /// <summary>
        /// 为 <see cref="DependencyObject"/> 派生类实现基本初始化
        /// </summary>
        protected DependencyObject()
        {
            _currentType = GetType();
            _effectiveAttachedValues = new Dictionary<DependencyProperty, EffectiveValueEntry>();
            _effectiveValues = new Dictionary<DependencyProperty, EffectiveValueEntry>();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 停止依赖属性的临时设置状态，并将其还原到调用 <see cref="ModifyValue(DependencyProperty, object)"/> 方法前的状态
        /// </summary>
        /// <param name="property">要停止临时设置状态的依赖属性</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="property"/> 为 <see langword="null"/> 时发生。</exception>
        public void EndModify(DependencyProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (TryGetValueEntry(property, out EffectiveValueEntry valueEntry))
            {
                if (valueEntry.Value != GetDefaultValue(property))
                {
                    SetValueEntry(property, valueEntry.Value);
                }
                else
                {
                    if (!property._isAttached)
                    {
                        _effectiveValues.Remove(property);
                    }
                    else
                    {
                        _effectiveAttachedValues.Remove(property);
                    }
                }
            }
        }

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
            if (TryGetValueEntry(property, out EffectiveValueEntry valueEntry))
            {
                return valueEntry.EffectiveValue;
            }
            return GetDefaultValue(property);
        }

        /// <summary>
        /// 临时设置依赖属性的值
        /// </summary>
        /// <param name="property">要设置值的依赖属性</param>
        /// <param name="value">要设置的值</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="property"/> 为 <see langword="null"/> 时发生。</exception>
        /// <remarks>
        /// 使用此方法设置的值，在调用 <see cref="EndModify(DependencyProperty)"/> 
        /// 或 <see cref="SetValue(DependencyProperty, object)"/> 后，会被还原到使用该方法前。
        /// </remarks>
        public void ModifyValue(DependencyProperty property, object value)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if(!property.ValidateValue(value))
            {
                value = CoerceValue(property, value);
            }
            ModifyValueEntry(property, value);
        }

        /// <summary>
        /// 设置依赖属性的值并停止临时设置状态
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

            if (!property.ValidateValue(value))
            {
                value = CoerceValue(property, value);
            }
            SetValueEntry(property, CoerceValue(property, value));
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
            PropertyMetadata metadata = property.GetMetadata(_currentType);

            object coercedValue = metadata.CoerceValue(this, value);
            if(!property.ValidateValue(coercedValue))
            {
                property.ThrowInvalidCoercedValueException(coercedValue);
            }    
            return coercedValue;
        }

        private void ModifyValueEntry(DependencyProperty property, object value)
        {
            object oldValue;
            if (TryGetValueEntry(property, out EffectiveValueEntry valueEntry))
            {
                oldValue = valueEntry.EffectiveValue;
                if (oldValue == value) { return; }

                EffectiveValueEntry newEntry = new EffectiveValueEntry(valueEntry.Value, value);

                if (!property._isAttached)
                {
                    _effectiveValues[property] = newEntry;
                }
                else
                {
                    _effectiveAttachedValues[property] = newEntry;
                }

                PropertyMetadata metadata = property.GetMetadata(_currentType);
                metadata.NotifyPropertyChanged(this,
                    new DependencyPropertyChangedEventArgs(property, metadata, valueEntry, newEntry));
            }
            else
            {
                oldValue = GetDefaultValue(property);
                if (oldValue == value) { return; }

                EffectiveValueEntry newEntry = new EffectiveValueEntry(valueEntry.Value, value);
                if (!property._isAttached)
                {
                    _effectiveValues.Add(property, newEntry);
                }
                else
                {
                    _effectiveAttachedValues.Add(property, newEntry);
                }

                PropertyMetadata metadata = property.GetMetadata(_currentType);
                metadata.NotifyPropertyChanged(this,
                    new DependencyPropertyChangedEventArgs(property, metadata, new EffectiveValueEntry(oldValue), newEntry));
            }
        }

        private object GetDefaultValue(DependencyProperty property)
        {
            return property.GetMetadata(_currentType)._defaultValue;
        }

        private void SetValueEntry(DependencyProperty property, object value)
        {
            object oldValue;
            if (TryGetValueEntry(property, out EffectiveValueEntry valueEntry))
            {
                oldValue = valueEntry.EffectiveValue;
                if (oldValue == value) { return; }

                EffectiveValueEntry newEntry = new EffectiveValueEntry(value);
                if (!property._isAttached)
                {
                    _effectiveValues[property] = newEntry;
                }
                else
                {
                    _effectiveAttachedValues[property] = newEntry;
                }

                PropertyMetadata metadata = property.GetMetadata(_currentType);
                metadata.NotifyPropertyChanged(this,
                    new DependencyPropertyChangedEventArgs(property, metadata, valueEntry, newEntry));
            }
            else
            {
                oldValue = GetDefaultValue(property);
                if (oldValue == value) { return; }

                EffectiveValueEntry newEntry = new EffectiveValueEntry(value);

                if (!property._isAttached)
                {
                    _effectiveValues.Add(property, new EffectiveValueEntry(value));
                }
                else
                {
                    _effectiveAttachedValues.Add(property, new EffectiveValueEntry(value));
                }

                PropertyMetadata metadata = property.GetMetadata(_currentType);
                metadata.NotifyPropertyChanged(this,
                    new DependencyPropertyChangedEventArgs(property, metadata, new EffectiveValueEntry(oldValue), newEntry));
            }
        }

        private bool TryGetValueEntry(DependencyProperty property, out EffectiveValueEntry valueEntry)
        {
            return property._isAttached
                ? _effectiveAttachedValues.TryGetValue(property, out valueEntry)
                : _effectiveValues.TryGetValue(property, out valueEntry);
        }


        #endregion
    }
}
