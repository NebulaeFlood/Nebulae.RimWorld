using Nebulae.RimWorld.UI.Core.Data.Expressions;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖对象
    /// </summary>
    public abstract class DependencyObject
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 该 <see cref="DependencyObject"/> 的实际类型
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// 该 <see cref="DependencyObject"/> 的 CLR 类型包装器
        /// </summary>
        public DependencyObjectType DependencyObjectType;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取 <see cref="DependencyObject"/> 的父级
        /// </summary>
        public DependencyObject Parent
        {
            get => parent;
        }

        /// <summary>
        /// 获取 <see cref="DependencyObject"/> 的逻辑子元素集合
        /// </summary>
        /// <remarks>需保证返回的集合中不包含 <see langword="null"/>。</remarks>
        public virtual IEnumerable<DependencyObject> LogicalChildren
        {
            get => Enumerable.Empty<DependencyObject>();
        }

        #endregion


        /// <summary>
        /// 为 <see cref="DependencyObject"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="type">该 <see cref="DependencyObject"/> 的实际类型</param>
        /// <remarks><paramref name="type"/> 不应为 <see langword="null"/>。</remarks>
        protected DependencyObject(Type type)
        {
            Type = type;
            DependencyObjectType = DependencyObjectType.FromSystemTypeUnsafe(type);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 清除依赖属性的有效值
        /// </summary>
        /// <param name="property">要清除有效值的依赖属性</param>
        /// <remarks>仅清除优先级为 <see cref="ValuePrecedence.Local"/> 的有效值。</remarks>
        public void ClearValue(DependencyProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            updatableProperties.Remove(property);
            ClearValueCommon(property, ValuePrecedence.Local);
        }

        /// <summary>
        /// 清除依赖属性的有效值
        /// </summary>
        /// <param name="property">要清除有效值的依赖属性</param>
        /// <param name="precedence">要清除的有效值的优先级</param>
        public void ClearValue(DependencyProperty property, ValuePrecedence precedence)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            updatableProperties.Remove(property);
            ClearValueCommon(property, precedence);
        }

        /// <summary>
        /// 获取依赖属性的有效值
        /// </summary>
        /// <param name="property">要获取有效值的依赖属性</param>
        /// <returns><paramref name="property"/> 的有效值。</returns>
        public object GetValue(DependencyProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return GetValueCommon(property);
        }

        /// <summary>
        /// 获取依赖属性的有效值
        /// </summary>
        /// <param name="property">要获取有效值的依赖属性</param>
        /// <param name="precedence">要获取的有效值的优先级</param>
        /// <returns><paramref name="property"/> 在 <paramref name="precedence"/> 优先级下的有效值。</returns>
        /// <remarks>当未设置在 <paramref name="precedence"/> 优先级下的有效值时，将返回默认值。</remarks>
        public object GetValue(DependencyProperty property, ValuePrecedence precedence)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            switch (precedence)
            {
                case ValuePrecedence.Default:
                    return property[DependencyObjectType].DefaultValue;
                case ValuePrecedence.Local:
                    return GetValueCommon(property);
                default:
                    return GetValueDirectly(property, precedence);
            }
        }

        /// <summary>
        /// 设置依赖属性的有效值
        /// </summary>
        /// <param name="property">要设置有效值的依赖属性</param>
        /// <param name="value">要设置的有效值</param>
        public void SetValue(DependencyProperty property, object value)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (value is Expression expression)
            {
                updatableProperties.Remove(property);

                SetValueExpression(property, expression);
            }
            else
            {
                property.ValidateValue(value);
                updatableProperties.Remove(property);

                SetValueCommon(property, value);
            }
        }

        /// <summary>
        /// 设置依赖属性指定优先级的有效值
        /// </summary>
        /// <param name="property">要设置有效值的依赖属性</param>
        /// <param name="value">要设置的有效值</param>
        /// <param name="precedence">设置的有效值的优先级</param>
        public void SetValue(DependencyProperty property, object value, ValuePrecedence precedence)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (value is Expression expression)
            {
                updatableProperties.Remove(property);

                SetValueExpression(property, expression);
                return;
            }

            property.ValidateValue(value);
            updatableProperties.Remove(property);

            if (precedence is ValuePrecedence.Local)
            {
                SetValueCommon(property, value);
            }
            else
            {
                SetValueDirectly(property, value, precedence);
            }
        }

        #endregion


        /// <summary>
        /// 当前对象的依赖属性的值发生变化时执行的方法
        /// </summary>
        /// <param name="args">有关属性更改的数据</param>
        protected virtual void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs args) { }


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal void AddDependent(DependencyProperty property, Expression expression)
        {
            if (_dependentMaps.TryGetValue(property, out DependentList dependentList))
            {
                dependentList.Add(expression);
                return;
            }

            dependentList = new DependentList { expression };

            _dependentMaps[property] = dependentList;

            expression.OnAttached(this, property);
        }

        internal void RemoveDependent(DependencyProperty property, Expression expression)
        {
            if (!_dependentMaps.TryGetValue(property, out DependentList dependentList))
            {
                return;
            }

            dependentList.Remove(expression);

            if (dependentList.IsEmpty)
            {
                _dependentMaps.Remove(property);
            }

            expression.OnDetached(this, property);
        }

        internal object CoerceValue(DependencyProperty property, PropertyMetadata metadata, object value, out bool isCoerced)
        {
            var coerceValueCallback = metadata.CoerceValueCallback;

            if (coerceValueCallback is null)
            {
                isCoerced = false;
                return value;
            }

            object coercedValue = coerceValueCallback.Invoke(this, value);

            if (!Equals(coercedValue, value))
            {
                try
                {
                    property.ValidateValue(coercedValue);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Coerced value '{coercedValue.AsLog()}' of type '{coercedValue.GetType()}' is not valid for '{property}'.", e);
                }

                isCoerced = true;
                return coercedValue;
            }

            isCoerced = false;
            return value;
        }

        internal object GetBindingValue(DependencyProperty property)
        {
            if (_effectiveValues.TryGetValue(property, out DependencyValue values))
            {
                return values.GetValue();
            }
            else
            {
                return property[DependencyObjectType].DefaultValue;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private void ClearValueCommon(DependencyProperty property, ValuePrecedence precedence)
        {
            if (!_effectiveValues.TryGetValue(property, out DependencyValue values))
            {
                return;
            }

            PropertyMetadata metadata = values.Metadata;
            object oldValue = values.GetValue(this, property);

            values.RemoveValue(precedence);

            if (values.IsEmpty)
            {
                if (!metadata.Inherits || parent is null)
                {
                    values.SetValue(this, property, metadata.DefaultValue, ValuePrecedence.Default);
                }
                else
                {
                    values.SetValue(this, property, parent.GetValueCommon(property), ValuePrecedence.Inherited);
                }
            }

            object newValue = values.GetValue(this, property);

            if (!Equals(oldValue, newValue))
            {
                NotifyPropertyChanged(new DependencyPropertyChangedEventArgs(
                    property,
                    values.Metadata,
                    oldValue,
                    newValue));
            }
        }

        private object GetValueCommon(DependencyProperty property)
        {
            if (_effectiveValues.TryGetValue(property, out DependencyValue values))
            {
                return values.GetValue(this, property);
            }

            if (property.IsDefaultMetadata)
            {
                return property.DefaultMetadata.DefaultValue;
            }

            PropertyMetadata metadata = property[DependencyObjectType];
            values = new DependencyValue(metadata);

            if (!metadata.Inherits || parent is null)
            {
                values.SetValue(this, property, metadata.DefaultValue, ValuePrecedence.Default);
            }
            else
            {
                values.SetValue(this, property, parent.GetValueCommon(property), ValuePrecedence.Inherited);
            }

            _effectiveValues[property] = values;
            return values.GetValue(this, property);
        }

        private object GetValueDirectly(DependencyProperty property, ValuePrecedence precedence)
        {
            if (_effectiveValues.TryGetValue(property, out DependencyValue values))
            {
                return values.GetValue(precedence);
            }
            else
            {
                return property[DependencyObjectType].DefaultValue;
            }
        }

        private DependencyValue GetValueStore(DependencyProperty property)
        {
            if (_effectiveValues.TryGetValue(property, out DependencyValue values))
            {
                return values;
            }

            PropertyMetadata metadata = property[DependencyObjectType];
            values = new DependencyValue(metadata);

            if (property.IsDefaultMetadata || !metadata.Inherits || parent is null)
            {
                values.SetValue(this, property, metadata.DefaultValue, ValuePrecedence.Default);
            }
            else
            {
                values.SetValue(this, property, parent.GetValueCommon(property), ValuePrecedence.Inherited);
            }

            _effectiveValues[property] = values;
            return values;
        }

        private void SetValueCommon(DependencyProperty property, object value)
        {
            DependencyValue values = GetValueStore(property);
            object oldValue = values.GetValue(this, property);

            if (values.Expression is null)
            {
                if (Equals(oldValue, value))
                {
                    return;
                }

                values.SetValue(this, property, value, ValuePrecedence.Local);
            }
            else
            {
                if (values.Expression.Assignable)
                {
                    values.SetValue(this, property, value, ValuePrecedence.Cache);
                }
                else
                {
                    var expression = values.Expression;

                    values.Expression = null;
                    values.RemoveValue(ValuePrecedence.Cache);
                    values.SetValue(this, property, value, ValuePrecedence.Local);

                    RemoveDependent(property, expression);
                }
            }

            object newValue = values.GetValue(this, property);

            if (!Equals(oldValue, newValue))
            {
                NotifyPropertyChanged(new DependencyPropertyChangedEventArgs(
                    property,
                    values.Metadata,
                    oldValue,
                    newValue));
            }
        }

        private void SetValueDirectly(DependencyProperty property, object value, ValuePrecedence precedence)
        {
            DependencyValue values = GetValueStore(property);
            object oldValue = values.GetValue(this, property);

            if (Equals(oldValue, value))
            {
                return;
            }

            values.SetValue(this, property, value, precedence);

            object newValue = values.GetValue(this, property);

            if (!Equals(oldValue, newValue))
            {
                NotifyPropertyChanged(new DependencyPropertyChangedEventArgs(
                    property,
                    values.Metadata,
                    oldValue,
                    newValue));
            }
        }

        private void SetValueExpression(DependencyProperty property, Expression newExpr)
        {
            DependencyValue values = GetValueStore(property);

            if (values.Expression == newExpr)
            {
                return;
            }

            object oldValue = values.GetValue(this, property);

            if (values.Expression != null)
            {
                var oldExpr = values.Expression;

                values.Expression = null;
                values.RemoveValue(ValuePrecedence.Cache);

                RemoveDependent(property, oldExpr);
            }

            if (newExpr != Expression.Empty)
            {
                values.Expression = newExpr;

                AddDependent(property, newExpr);
            }

            object newValue = values.GetValue(this, property);

            if (!Equals(oldValue, newValue))
            {
                NotifyPropertyChanged(new DependencyPropertyChangedEventArgs(
                    property,
                    values.Metadata,
                    oldValue,
                    newValue));
            }
        }

        private void NotifyPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            args.Metadata.PropertyChangedCallback?.Invoke(this, args);
            OnDependencyPropertyChanged(args);

            if (_dependentMaps.TryGetValue(args.Property, out DependentList dependentList))
            {
                updatableProperties.Add(args.Property);
                dependentList.Update(this, args.Property, args.NewValue);
            }

            if (args.Metadata.Inherits)
            {
                foreach (var child in LogicalChildren)
                {
                    if (!updatableProperties.Contains(args.Property))
                    {
                        return;
                    }

                    child.SetValueDirectly(args.Property, args.NewValue, ValuePrecedence.Inherited);
                }
            }
        }

        #endregion


        internal DependencyObject parent;
        internal readonly HashSet<DependencyProperty> updatableProperties = new HashSet<DependencyProperty>();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Dictionary<DependencyProperty, DependencyValue> _effectiveValues = new Dictionary<DependencyProperty, DependencyValue>();
        private readonly Dictionary<DependencyProperty, DependentList> _dependentMaps = new Dictionary<DependencyProperty, DependentList>();

        #endregion
    }
}