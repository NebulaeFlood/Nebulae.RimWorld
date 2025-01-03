using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 被绑定成员的信息
    /// </summary>
    /// <typeparam name="T">被绑定成员的类型</typeparam>
    public struct BindingMemberInfo<T>
    {
        /// <summary>
        /// 是否为静态成员
        /// </summary>
        public readonly bool IsStatic;

        /// <summary>
        /// 成员名称
        /// </summary>
        public readonly string MemberName;


        /// <summary>
        /// 成员值获取器
        /// </summary>
        public readonly Func<object, T> MemberGetter;

        /// <summary>
        /// 成员值设置器
        /// </summary>
        public readonly Action<object, T> MemberSetter;

        /// <summary>
        /// 静态成员值获取器
        /// </summary>
        public readonly Func<T> StaticMemberGetter;
        /// <summary>
        /// 静态成员值设置器
        /// </summary>
        public readonly Action<T> StaticMemberSetter;


        /// <summary>
        /// 初始化 <see cref="BindingMemberInfo{T}"/> 的新实例
        /// </summary>
        /// <param name="ownerType">拥有被绑定成员的类型</param>
        /// <param name="memberName">被绑定成员的名称</param>
        /// <param name="flags">搜索成员的方式</param>
        public BindingMemberInfo(Type ownerType, string memberName, BindingFlags flags) : this()
        {
            if (ownerType.GetProperty(memberName, flags) is PropertyInfo property)
            {
                IsStatic = property.GetMethod?.IsStatic ?? property.SetMethod?.IsStatic
                ?? throw new InvalidOperationException($"Find a property {ownerType}.{memberName} without get and set method.");

                try
                {
                    if (IsStatic)
                    {
                        if (property.GetMethod != null)
                        {
                            StaticMemberGetter = (Func<T>)property.GetMethod
                                .CreateDelegate(typeof(Func<T>));
                        }
                        if (property.SetMethod != null)
                        {
                            StaticMemberSetter = (Action<T>)property.SetMethod
                                .CreateDelegate(typeof(Action<T>));
                        }
                    }
                    else
                    {
                        if (property.GetMethod != null)
                        {
                            MemberGetter = (Func<object, T>)property.GetMethod
                                .CreateDelegate(typeof(Func<object, T>));
                        }
                        if (property.SetMethod != null)
                        {
                            MemberSetter = (Action<object, T>)property.SetMethod
                                .CreateDelegate(typeof(Action<object, T>));
                        }
                    }
                }
                catch (ArgumentException exception)
                {
                    throw new ArgumentException($"Caught an unexpected error, probably caused by trying to get the info of an DependencyProperty wrapper named {ownerType}.{property.Name}, try again by switch the path to the instance of DependencyProperty.", exception);
                }
            }
            else if (ownerType.GetField(memberName, flags) is FieldInfo field)
            {
                IsStatic = field.IsStatic;

                if (IsStatic)
                {
                    ParameterExpression valueExp = Expression.Parameter(field.FieldType, "value");
                    MemberExpression fieldExp = Expression.Field(null, field);
                    BinaryExpression assignExp = Expression.Assign(fieldExp, valueExp);

                    StaticMemberGetter = Expression.Lambda<Func<T>>(fieldExp).Compile();
                    StaticMemberSetter = Expression.Lambda<Action<T>>(assignExp, valueExp).Compile();
                }
                else
                {
                    ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
                    ParameterExpression valueExp = Expression.Parameter(field.FieldType, "value");
                    MemberExpression fieldExp = Expression.Field(Expression.Convert(targetExp, ownerType), field);
                    BinaryExpression assignExp = Expression.Assign(fieldExp, valueExp);

                    MemberGetter = Expression.Lambda<Func<object, T>>(fieldExp, targetExp).Compile();
                    MemberSetter = Expression.Lambda<Action<object, T>>(assignExp, targetExp, valueExp).Compile();
                }
            }
            else
            {
                throw new InvalidOperationException($"Can not find any member named {memberName} in the type {ownerType}.");
            }

            MemberName = memberName;
        }

        /// <summary>
        /// 初始化 <see cref="BindingMemberInfo{T}"/> 的新实例
        /// </summary>
        /// <param name="property">被绑定成员</param>
        /// <param name="proeprtyType">被绑定成员的类型</param>
        public BindingMemberInfo(DependencyProperty property, Type proeprtyType) : this()
        {
            IsStatic = false;

            MemberGetter = (obj) => (T)((DependencyObject)obj).GetValue(property);
            MemberSetter = (obj, val) => ((DependencyObject)obj).SetValueIntelligently(property, val);

            MemberName = property.Name;
        }


        /// <summary>
        /// 判断成员的值能否被获取
        /// </summary>
        /// <returns>如果成员的值能被获取，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool IsReadable()
        {
            return MemberGetter != null || StaticMemberGetter != null;
        }

        /// <summary>
        /// 判断成员的值能否被设置
        /// </summary>
        /// <returns>如果成员的值能被设置，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool IsWritable()
        {
            return MemberSetter != null || StaticMemberSetter != null;
        }
    }
}
