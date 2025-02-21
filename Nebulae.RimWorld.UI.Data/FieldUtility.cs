using Nebulae.RimWorld.UI.Data.Binding.Converters;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 字段访问器
    /// </summary>
    /// <typeparam name="TClass">拥有字段的类型</typeparam>
    /// <typeparam name="TField">字段类型</typeparam>
    /// <param name="obj"></param>
    /// <returns>对应的字段的值。</returns>
    /// <remarks>对于静态字段，<paramref name="obj"/> 应传入 <see langword="null"/>。</remarks>
    public delegate TField FieldAccesser<in TClass, out TField>(TClass obj);

    /// <summary>
    /// 字段工具类
    /// </summary>
    public static class FieldUtility
    {
        /// <summary>
        /// 创建一个获取字段值的委托
        /// </summary>
        /// <typeparam name="T">字段类型</typeparam>
        /// <param name="type">拥有字段的类型</param>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>获取字段值的委托。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="type"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        public static FieldAccesser<object, T> CreateFieldAccesser<T>(
            this Type type,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetField(name, flags) is FieldInfo field)
            {
                ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
                MemberExpression fieldExp = Expression.Field(
                    field.IsStatic ? null : Expression.Convert(targetExp, type),
                    field);

                return Expression.Lambda<FieldAccesser<object, T>>(
                    fieldExp,
                    targetExp).Compile();
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        /// <summary>
        /// 创建一个获取字段值的委托
        /// </summary>
        /// <typeparam name="TClass">拥有字段的类型</typeparam>
        /// <typeparam name="TField">字段类型</typeparam>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>获取字段值的委托。</returns>
        /// <exception cref="MissingFieldException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        public static FieldAccesser<TClass, TField> CreateFieldAccesser<TClass, TField>(
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type type = typeof(TClass);

            if (type.GetField(name, flags) is FieldInfo field)
            {
                ParameterExpression targetExp = Expression.Parameter(type, "target");
                MemberExpression fieldExp = Expression.Field(
                    field.IsStatic ? null : targetExp,
                    field);

                return Expression.Lambda<FieldAccesser<TClass, TField>>(
                    fieldExp,
                    targetExp).Compile();
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">字段值将转化成的类型</typeparam>
        /// <param name="type">拥有字段的类型</param>
        /// <param name="name">字段名称</param>
        /// <param name="obj">拥有字段的实例</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段值转化为 <typeparamref name="T"/> 类型时发生。</exception>
        public static T GetValue<T>(
            this Type type,
            string name,
            object obj,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetField(name, flags) is FieldInfo field)
            {
                Type fieldType = field.FieldType;
                Type targetType = typeof(T);

                if (fieldType == targetType)
                {
                    return (T)field.GetValue(obj);
                }
                else if (SystemConvertUtility.CanConvert(fieldType, targetType))
                {
                    return (T)Convert.ChangeType(field.GetValue(obj), typeof(T), CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
                }
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">字段值将转化成的类型</typeparam>
        /// <param name="field">字段信息</param>
        /// <param name="obj">拥有字段的实例</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值。</returns>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段值转化为 <typeparamref name="T"/> 类型时发生。</exception>
        public static T GetValue<T>(this FieldInfo field, object obj)
        {
            Type fieldType = field.FieldType;
            Type targetType = typeof(T);

            if (fieldType == targetType)
            {
                return (T)field.GetValue(obj);
            }
            else if (SystemConvertUtility.CanConvert(fieldType, targetType))
            {
                return (T)Convert.ChangeType(field.GetValue(obj), typeof(T), CultureInfo.InvariantCulture);
            }
            else
            {
                throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
            }
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">字段值将转化成的类型</typeparam>
        /// <param name="obj">拥有字段的实例</param>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段值转化为 <typeparamref name="T"/> 类型时发生。</exception>
        public static T GetValue<T>(
            object obj,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type classType = obj.GetType();

            if (classType.GetField(name, flags) is FieldInfo field)
            {
                Type fieldType = field.FieldType;
                Type targetType = typeof(T);

                if (fieldType == targetType)
                {
                    return (T)field.GetValue(obj);
                }
                else if (SystemConvertUtility.CanConvert(fieldType, targetType))
                {
                    return (T)Convert.ChangeType(field.GetValue(obj), typeof(T), CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
                }
            }
            else
            {
                throw new MissingFieldException(classType.Name, name);
            }
        }
    }
}
