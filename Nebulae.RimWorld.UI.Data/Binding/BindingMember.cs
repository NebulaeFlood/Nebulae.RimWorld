using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 被绑定成员的信息
    /// </summary>
    public struct BindingMember : IEquatable<BindingMember>
    {
        //------------------------------------------------------
        //
        //  Pirvate Fields
        //
        //------------------------------------------------------

        #region Pirvate Fields

        private readonly int _hashCode;

        private bool _isValid;

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
        internal object _target;

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
        public object BindingTarget => IsStatic ? null : _target;

        /// <summary>
        /// 该信息是否有效
        /// </summary>
        public bool IsValid => _isValid;

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
                MemberName = property.Name;
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
            }
            else if (member is FieldInfo field)
            {
                IsReadable = true;
                IsStatic = field.IsStatic;
                MemberName = field.Name;
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
            }
            else
            {
                throw new ArgumentException("Binding member should be a property or a field.", nameof(member));
            }

            if (IsStatic)
            {
                _hashCode = MemberName.GetHashCode() ^ MemberType.GetHashCode();
            }
            else
            {
                _hashCode = target.GetHashCode() ^ MemberName.GetHashCode() ^ MemberType.GetHashCode();
                _target = target;
            }

            _isValid = true;
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

            MemberName = property.Name;
            MemberType = property.ValueType;

            _memberGetter = (obj) => ((DependencyObject)obj).GetValue(property);
            _memberSetter = (obj, val) => ((DependencyObject)obj).SetValueIntelligently(property, val);

            _hashCode = target.GetHashCode() ^ MemberName.GetHashCode() ^ MemberType.GetHashCode();
            _isValid = true;
            _target = target;
        }


        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is BindingMember other
                && (MemberType is null && other.MemberType is null
                    || (ReferenceEquals(BindingTarget, other.BindingTarget)
                        && MemberName == other.MemberName
                        && MemberType == other.MemberType));
        }

        /// <inheritdoc/>
        public bool Equals(BindingMember other)
        {
            return MemberType is null && other.MemberType is null
                || (ReferenceEquals(BindingTarget, other.BindingTarget)
                    && MemberName == other.MemberName
                    && MemberType == other.MemberType);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _hashCode;
        }

        /// <summary>
        /// 获取绑定成员的值
        /// </summary>
        /// <returns>绑定成员的值。</returns>
        public object GetValue()
        {
            return IsStatic ? _memberGetter(null) : _memberGetter(_target);
        }

        /// <summary>
        /// 无效化该绑定成员
        /// </summary>
        public void Invalid()
        {
            _isValid = false;
            _target = null;
        }

        /// <summary>
        /// 设置绑定成员的值
        /// </summary>
        /// <param name="value">要设置的值</param>
        public void SetValue(object value)
        {
            if (IsStatic)
            {
                _memberSetter(null, value);
            }
            else
            {
                _memberSetter(_target, value);
            }
        }
    }
}
