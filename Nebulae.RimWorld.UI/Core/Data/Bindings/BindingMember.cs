using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

namespace Nebulae.RimWorld.UI.Core.Data.Bindings
{
    /// <summary>
    /// 被绑定成员的信息
    /// </summary>
    /// <remarks>保留对拥有指定成员的对象的强引用。</remarks>
    public readonly struct BindingMember : IEquatable<BindingMember>
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 成员的完整名称
        /// </summary>
        public readonly string FullName;

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
        public readonly string Name;

        /// <summary>
        /// 成员所属的对象
        /// </summary>
        public readonly object Target;

        /// <summary>
        /// 成员类型
        /// </summary>
        public readonly Type Type;

        #endregion


        /// <summary>
        /// 获取或设置成员的值
        /// </summary>
        public object Value
        {
            get => MemberGetter();
            set => MemberSetter(value);
        }


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="BindingMember"/> 的新实例
        /// </summary>
        /// <param name="target">绑定的对象</param>
        /// <param name="member">绑定的成员，可以为 <see cref="FieldInfo"/> 或 <see cref="PropertyInfo"/></param>
        public BindingMember(object target, MemberInfo member) : this()
        {
            var declaringType = member.DeclaringType;

            FullName = $"{declaringType.FullName}.{member.Name}";
            Name = member.Name;

            if (member is PropertyInfo property)
            {
                Type = property.PropertyType;
                IsReadable = property.CanRead;
                IsStatic = property.GetMethod.IsStatic;
                IsWritable = property.CanWrite;

                if (IsReadable)
                {
                    if (Type.IsClass)
                    {
                        MemberGetter = (Func<object>)(IsStatic
                            ? property.GetMethod.CreateDelegate(typeof(Func<object>))
                            : property.GetMethod.CreateDelegate(typeof(Func<object>), target));
                    }
                    else
                    {
                        MemberGetter = Expression.Lambda<Func<object>>(
                            Expression.Convert(
                                Expression.Property(
                                    IsStatic ? null : Expression.Constant(target, declaringType),
                                    property),
                                typeof(object))).Compile();
                    }
                }

                if (IsWritable)
                {
                    if (Type != typeof(object))
                    {
                        MemberSetter = Expression.Lambda<Action<object>>(
                            Expression.Assign(
                                Expression.Property(
                                    IsStatic ? null : Expression.Constant(target, declaringType),
                                    property),
                                Expression.Convert(ValueArgExp, Type)),
                            ValueArgExp).Compile();
                    }
                    else
                    {
                        MemberSetter = (Action<object>)(IsStatic
                            ? property.SetMethod.CreateDelegate(typeof(Action<object>))
                            : property.SetMethod.CreateDelegate(typeof(Action<object>), target));
                    }
                }
            }
            else if (member is FieldInfo field)
            {
                Type = field.FieldType;
                IsReadable = true;
                IsStatic = field.IsStatic;
                IsWritable = !field.IsInitOnly;

                if (Type.IsClass)
                {
                    MemberGetter = Expression.Lambda<Func<object>>(
                        Expression.Field(
                            IsStatic ? null : Expression.Constant(target, declaringType),
                            field)).Compile();
                }
                else
                {
                    MemberGetter = Expression.Lambda<Func<object>>(
                        Expression.Convert(
                            Expression.Field(
                                IsStatic ? null : Expression.Constant(target, declaringType),
                                field),
                            typeof(object))).Compile();
                }

                if (IsWritable)
                {
                    if (Type != typeof(object))
                    {
                        MemberSetter = Expression.Lambda<Action<object>>(
                            Expression.Assign(
                                Expression.Field(
                                    IsStatic ? null : Expression.Constant(target, declaringType),
                                    field),
                                Expression.Convert(ValueArgExp, Type)),
                            ValueArgExp).Compile();
                    }
                    else
                    {
                        MemberSetter = Expression.Lambda<Action<object>>(
                            Expression.Assign(
                                Expression.Field(
                                    IsStatic ? null : Expression.Constant(target, declaringType),
                                    field),
                                ValueArgExp),
                            ValueArgExp).Compile();
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"{typeof(BindingMember)} can only support fields and properties.");
            }

            Target = IsStatic ? null : target;

            _hashCode = target.GetHashCode() ^ Name.GetHashCode() ^ Type.GetHashCode();
        }

        /// <summary>
        /// 初始化 <see cref="BindingMember"/> 的新实例
        /// </summary>
        /// <param name="target">绑定的对象</param>
        /// <param name="property">绑定的成员</param>
        public BindingMember(DependencyObject target, DependencyProperty property) : this()
        {
            IsReadable = true;
            IsStatic = false;
            IsWritable = true;

            FullName = $"{target.Type.FullName}.{property.Name}";
            Name = property.Name;
            Type = property.ValueType;

            MemberGetter = () => target.GetValue(property);
            MemberSetter = (value) => target.SetValueByBinding(property, value);

            Target = target;

            _hashCode = target.GetHashCode() ^ Name.GetHashCode() ^ Type.GetHashCode();
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is BindingMember other
                && (Type is null && other.Type is null
                    || (ReferenceEquals(Target, other.Target)
                        && Name.Equals(other.Name)
                        && Type.Equals(other.Type)));
        }

        /// <summary>
        /// 判断当前 <see cref="BindingMember"/> 是否与指定的 <see cref="BindingMember"/> 相等
        /// </summary>
        /// <param name="other">要判断的 <see cref="BindingMember"/></param>
        /// <returns>若相等，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(BindingMember other)
        {
            return Type is null && other.Type is null
                || (ReferenceEquals(Target, other.Target)
                    && Name.Equals(other.Name)
                    && Type.Equals(other.Type));
        }

        /// <summary>
        /// 获取该 <see cref="BindingMember"/> 的哈希代码
        /// </summary>
        /// <returns>该 <see cref="BindingMember"/> 的哈希代码。</returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }

        /// <summary>
        /// 返回该 <see cref="BindingMember"/> 的字符串表示形式
        /// </summary>
        /// <returns>该 <see cref="BindingMember"/> 的字符串表示形式。</returns>
        public override string ToString() => FullName;

        #endregion


        //------------------------------------------------------
        //
        //  Pirvate Fields
        //
        //------------------------------------------------------

        #region Pirvate Fields

        private static readonly ParameterExpression ValueArgExp = Expression.Parameter(typeof(object), "value");

        private readonly int _hashCode;

        private readonly Func<object> MemberGetter;
        private readonly Action<object> MemberSetter;

        #endregion
    }
}
