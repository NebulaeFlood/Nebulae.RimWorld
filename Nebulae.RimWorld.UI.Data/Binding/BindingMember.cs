using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 被绑定成员的信息
    /// </summary>
    public readonly struct BindingMember : IEquatable<BindingMember>
    {
        //------------------------------------------------------
        //
        //  Pirvate Fields
        //
        //------------------------------------------------------

        #region Pirvate Fields

        /// <summary>
        /// 成员值获取器
        /// </summary>
        private readonly Func<object, object> _memberGetter;

        /// <summary>
        /// 成员值设置器
        /// </summary>
        private readonly Action<object, object> _memberSetter;

        /// <summary>
        /// 绑定对象
        /// </summary>
        private readonly WeakReference _target;

        #endregion


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 是否可获取成员的值
        /// </summary>
        public readonly bool IsReadable;

        /// <summary>
        /// 是否为静态成员
        /// </summary>
        public readonly bool IsStatic;

        /// <summary>
        /// 是否可设置成员的值
        /// </summary>
        public readonly bool IsWritable;

        /// <summary>
        /// 成员名称
        /// </summary>
        public readonly string MemberName;

        /// <summary>
        /// 成员类型
        /// </summary>
        public readonly Type MemberType;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 目标对象
        /// </summary>
        public object AssociatedObject => IsStatic ? null : _target.Target;

        /// <summary>
        /// 目标对象是否存活
        /// </summary>
        public bool IsAlive => IsStatic || _target.IsAlive;

        /// <summary>
        /// 获取或设置绑定成员的值
        /// </summary>
        /// <remarks>使用前注意判断绑定成员是否可读或可写。</remarks>
        public object Value
        {
            get => IsStatic ? _memberGetter(null) : _memberGetter(_target.Target);
            set
            {
                if (IsStatic)
                {
                    _memberSetter(null, value);
                }
                else
                {
                    _memberSetter(_target.Target, value);
                }
            }
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="BindingMember"/> 的新实例
        /// </summary>
        /// <param name="target">绑定的对象</param>
        /// <param name="member">绑定的成员，可以为 <see cref="FieldInfo"/> 或 <see cref="PropertyInfo"/></param>
        public BindingMember(object target, MemberInfo member) : this()
        {
            if (member is PropertyInfo property)
            {
                // SetMethod can not be null when GetMethod is null.
                IsStatic = property.GetMethod?.IsStatic ?? property.SetMethod.IsStatic;
                MemberType = property.PropertyType;

                ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
                MemberExpression propertyExp = Expression.Property(
                    IsStatic ? null : Expression.Convert(targetExp, property.DeclaringType),
                    property);

                if (property.GetMethod != null)
                {
                    IsReadable = true;

                    _memberGetter = Expression.Lambda<Func<object, object>>(
                        Expression.Convert(propertyExp, typeof(object)),
                        targetExp).Compile();
                }

                if (property.SetMethod != null)
                {
                    IsWritable = true;

                    ParameterExpression valueExp = Expression.Parameter(typeof(object), "value");

                    _memberSetter = Expression.Lambda<Action<object, object>>(
                        Expression.Assign(propertyExp, Expression.Convert(valueExp, MemberType)),
                        targetExp,
                        valueExp).Compile();
                }

                _target = IsStatic
                    ? null
                    : new WeakReference(target);
            }
            else if (member is FieldInfo field)
            {
                IsReadable = true;
                IsStatic = field.IsStatic;
                MemberType = field.FieldType;

                ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
                MemberExpression fieldExp = Expression.Field(
                    IsStatic ? null : Expression.Convert(targetExp, field.DeclaringType),
                    field);

                _memberGetter = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(fieldExp, typeof(object)),
                    targetExp).Compile();

                if (!field.IsInitOnly)
                {
                    IsWritable = true;

                    ParameterExpression valueExp = Expression.Parameter(typeof(object), "value");

                    _memberSetter = Expression.Lambda<Action<object, object>>(
                        Expression.Assign(fieldExp, Expression.Convert(valueExp, MemberType)),
                        targetExp,
                        valueExp).Compile();
                }

                _target = IsStatic
                    ? null
                    : new WeakReference(target);
            }
            else
            {
                throw new ArgumentException("Binding member should be a property or a field.", nameof(member));
            }
        }


        /// <summary>
        /// 初始化 <see cref="BindingMember"/> 的新实例
        /// </summary>
        /// <param name="target">绑定的对象</param>
        /// <param name="property">绑定的成员</param>
        public BindingMember(object target, DependencyProperty property) : this()
        {
            IsReadable = true;
            IsStatic = false;
            IsWritable = true;

            _memberGetter = (obj) => ((DependencyObject)obj).GetValue(property);
            _memberSetter = (obj, val) => ((DependencyObject)obj).SetValueIntelligently(property, val);

            _target = new WeakReference(target);

            MemberName = property.Name;
            MemberType = property.ValueType;
        }


        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is BindingMember other
                && AssociatedObject == other.AssociatedObject
                && MemberName == other.MemberName
                && MemberType == other.MemberType;
        }

        /// <inheritdoc/>
        public bool Equals(BindingMember other)
        {
            return AssociatedObject == other.AssociatedObject
                && MemberName == other.MemberName
                && MemberType == other.MemberType;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return IsStatic
                ? MemberType.GetHashCode() ^ MemberName.GetHashCode()
                : AssociatedObject.GetHashCode() ^ MemberType.GetHashCode() ^ MemberName.GetHashCode();
        }
    }
}
