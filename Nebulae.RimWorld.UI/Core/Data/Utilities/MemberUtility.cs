using System;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Core.Data.Utilities
{
    /// <summary>
    /// 类成员帮助类
    /// </summary>
    public static class MemberUtility
    {
        /// <summary>
        /// 默认的成员搜索方式
        /// </summary>
        public const BindingFlags DefaultFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


        /// <summary>
        /// 判断 <see cref="Convert"/> 类型能否将 <paramref name="sourceType"/> 类型转化为 <paramref name="targetType"/> 类型
        /// </summary>
        /// <param name="sourceType">源类型</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="convertOperation"><paramref name="sourceType"/> 到 <paramref name="targetType"/> 是否需要额外的转化操作。</param>
        /// <returns>当 <see cref="Convert"/> 类型能够进行转化时，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool CanConvert(Type sourceType, Type targetType, out bool convertOperation)
        {
            if (targetType.IsAssignableFrom(sourceType))
            {
                convertOperation = false;
                return true;
            }

            if (SystemConvertUtility.CanConvert(sourceType, targetType))
            {
                convertOperation = true;
                return true;
            }

            convertOperation = false;
            return false;
        }

        /// <summary>
        /// 使用 <see cref="Convert"/> 将值转化为指定类型
        /// </summary>
        /// <typeparam name="TSource">值的原类型</typeparam>
        /// <typeparam name="TTarget">转化的类型</typeparam>
        /// <param name="value">要转化的值</param>
        /// <returns>转化后的值。</returns>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将 <paramref name="value"/> 转化为 <typeparamref name="TTarget"/> 时发生。</exception>
        public static TTarget ConvertTo<TSource, TTarget>(TSource value)
        {
            if (value is TTarget target)
            {
                return target;
            }

            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            if (SystemConvertUtility.CanConvert(sourceType, targetType))
            {
                return (TTarget)Convert.ChangeType(value, targetType);
            }

            throw new InvalidCastException($"Cannot cast type: {sourceType} to type: {targetType}.");
        }

        /// <summary>
        /// 从方法创建指定类型的委托
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <param name="method">目标方法</param>
        /// <returns>由 <paramref name="method"/> 创建的委托。</returns>
        public static T CreateDelegate<T>(this MethodInfo method) where T : Delegate
        {
            return (T)method.CreateDelegate(typeof(T));
        }

        /// <summary>
        /// 从方法创建指定类型的委托
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <param name="method">目标方法</param>
        /// <param name="target">目标对象</param>
        /// <returns>由 <paramref name="method"/> 创建的委托。</returns>
        public static T CreateDelegate<T>(this MethodInfo method, object target) where T : Delegate
        {
            return (T)method.CreateDelegate(typeof(T), target);
        }

        /// <summary>
        /// 尝试使用 <see cref="Convert"/> 将值转化为指定类型
        /// </summary>
        /// <typeparam name="TSource">值的原类型</typeparam>
        /// <typeparam name="TTarget">转化的类型</typeparam>
        /// <param name="value">要转化的值</param>
        /// <param name="result">转化结果</param>
        /// <returns>当 <see cref="Convert"/> 类型能够进行转化时，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool TryConvertTo<TSource, TTarget>(TSource value, out TTarget result)
        {
            if (value is TTarget target)
            {
                result = target;
                return true;
            }

            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            if (SystemConvertUtility.CanConvert(sourceType, targetType))
            {
                result = (TTarget)Convert.ChangeType(value, targetType);
                return true;
            }

            result = default;
            return false;
        }


        /// <summary>
        /// 获取字段或属性的值
        /// </summary>
        /// <typeparam name="T">获取的值将转换成的类型</typeparam>
        /// <param name="type">拥有字段或属性的类型</param>
        /// <param name="name">字段或属性名称</param>
        /// <param name="obj">目标对象</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值或属性值。</returns>
        /// <exception cref="MissingMemberException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段或属性时发生。</exception>
        public static T GetValue<T>(this Type type, string name, object obj, BindingFlags flags = DefaultFlags)
        {
            var members = type.GetMember(name, MemberTypes.Field | MemberTypes.Property, flags);

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] is FieldInfo field)
                {
                    return field.GetValue<T>(obj);
                }
                else if (members[i] is PropertyInfo property)
                {
                    return PropertyUtility.GetValue<T>(property, obj);
                }
            }

            throw new MissingMemberException(type.FullName, name);
        }

        /// <summary>
        /// 获取字段或属性的值
        /// </summary>
        /// <typeparam name="T">获取的值将转换成的类型</typeparam>
        /// <param name="member">可能为字段或属性的成员</param>
        /// <param name="obj">目标对象</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值或属性值。</returns>
        /// <exception cref="ArgumentException">当 <paramref name="member"/> 不是字段或属性时发生。</exception>
        public static T GetValue<T>(this MemberInfo member, object obj)
        {
            if (member is FieldInfo field)
            {
                return field.GetValue<T>(obj);
            }
            else if (member is PropertyInfo property)
            {
                return PropertyUtility.GetValue<T>(property, obj);
            }
            else
            {
                throw new ArgumentException("Memer must be a field or a property.");
            }
        }

        /// <summary>
        /// 获取字段或属性的值
        /// </summary>
        /// <typeparam name="TClass">拥有字段或属性的类型</typeparam>
        /// <typeparam name="TValue">获取的值将转换成的类型</typeparam>
        /// <param name="obj">目标对象</param>
        /// <param name="name">字段或属性名称</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>转化为 <typeparamref name="TValue"/> 类型的字段值或属性值。</returns>
        /// <exception cref="MissingMemberException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段或属性时发生。</exception>
        public static TValue GetValue<TClass, TValue>(TClass obj, string name, BindingFlags flags = DefaultFlags)
        {
            var type = typeof(TClass);
            var members = type.GetMember(name, MemberTypes.Field | MemberTypes.Property, flags);

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] is FieldInfo field)
                {
                    return field.GetValue<TValue>(obj);
                }
                else if (members[i] is PropertyInfo property)
                {
                    return PropertyUtility.GetValue<TValue>(property, obj);
                }
            }

            throw new MissingMemberException(type.FullName, name);
        }
    }
}
