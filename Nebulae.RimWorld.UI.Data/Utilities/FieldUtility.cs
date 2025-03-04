using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Utilities
{
    /// <summary>
    /// 字段访问器
    /// </summary>
    /// <typeparam name="TClass">拥有字段的类型</typeparam>
    /// <typeparam name="TField">字段类型</typeparam>
    /// <param name="obj">拥有字段的实例</param>
    /// <returns>对应的字段的值。</returns>
    /// <remarks>对于静态字段，<paramref name="obj"/> 应传入 <see langword="null"/>。</remarks>
    public delegate TField FieldAccesser<in TClass, out TField>(TClass obj);

    /// <summary>
    /// 字段修改器
    /// </summary>
    /// <typeparam name="TClass">拥有字段的类型</typeparam>
    /// <typeparam name="TField">字段类型</typeparam>
    /// <param name="obj">拥有字段的实例</param>
    /// <param name="value">要赋给字段的值</param>
    /// <remarks>对于静态字段，<paramref name="obj"/> 应传入 <see langword="null"/>。</remarks>
    public delegate void FieldModifier<in TClass, in TField>(TClass obj, TField value);


    /// <summary>
    /// 字段工具类
    /// </summary>
    public static class FieldUtility
    {
        /// <summary>
        /// 创建一个获取字段值的委托
        /// </summary>
        /// <typeparam name="T">获取的字段值将转化成的类型</typeparam>
        /// <param name="type">拥有字段的类型</param>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>获取字段值的委托。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="type"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段的类型转化为 <typeparamref name="T"/> 类型时发生。</exception>
        public static FieldAccesser<object, T> CreateFieldAccesser<T>(
            this Type type,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetField(name, flags) is FieldInfo field)
            {
                Type fieldType = field.FieldType;
                Type targetType = typeof(T);

                if (!CanConvert(fieldType, targetType, out var needConvert))
                {
                    throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
                }

                ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
                MemberExpression fieldExp = Expression.Field(
                    field.IsStatic ? null : Expression.Convert(targetExp, type),
                    field);

                if (needConvert)
                {
                    return Expression.Lambda<FieldAccesser<object, T>>(
                        Expression.Convert(fieldExp, targetType),
                        targetExp).Compile();
                }

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
        /// <typeparam name="TValue">获取的字段值将转化成的类型</typeparam>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>获取字段值的委托。</returns>
        /// <exception cref="MissingFieldException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段的类型转化为 <typeparamref name="TValue"/> 类型时发生。</exception>
        public static FieldAccesser<TClass, TValue> CreateFieldAccesser<TClass, TValue>(
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type type = typeof(TClass);

            if (type.GetField(name, flags) is FieldInfo field)
            {
                Type fieldType = field.FieldType;
                Type targetType = typeof(TValue);

                if (!CanConvert(fieldType, targetType, out var needConvert))
                {
                    throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
                }

                ParameterExpression targetExp = Expression.Parameter(type, "target");
                MemberExpression fieldExp = Expression.Field(
                    field.IsStatic ? null : targetExp,
                    field);

                if (needConvert)
                {
                    return Expression.Lambda<FieldAccesser<TClass, TValue>>(
                        Expression.Convert(fieldExp, targetType),
                        targetExp).Compile();
                }

                return Expression.Lambda<FieldAccesser<TClass, TValue>>(
                    fieldExp,
                    targetExp).Compile();
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        /// <summary>
        /// 创建一个修改字段值的委托
        /// </summary>
        /// <typeparam name="T">将给字段赋的值的类型</typeparam>
        /// <param name="type">拥有字段的类型</param>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>修改字段值的委托。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="type"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidOperationException">当字段为 <see langword="readonly"/> 时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法 <typeparamref name="T"/> 类型转化为将字段的类型时发生。</exception>
        public static FieldModifier<object, T> CreateFieldModifier<T>(
            Type type,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetField(name, flags) is FieldInfo field)
            {
                if (field.IsInitOnly)
                {
                    throw new InvalidOperationException($"Can not create modifier for {type}.{field.Name} - it is read only.");
                }

                Type fieldType = field.FieldType;
                Type valueType = typeof(T);

                if (!CanConvert(valueType, fieldType, out var needConvert))
                {
                    throw new InvalidCastException($"Cannot cast type: {valueType} to type: {fieldType}.");
                }

                ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
                ParameterExpression valueExp = Expression.Parameter(valueType, "value");
                MemberExpression fieldExp = Expression.Field(
                    field.IsStatic ? null : Expression.Convert(targetExp, type),
                    field);

                if (needConvert)
                {
                    return Expression.Lambda<FieldModifier<object, T>>(
                        Expression.Assign(fieldExp, Expression.Convert(valueExp, fieldType)),
                        targetExp, valueExp).Compile();
                }

                return Expression.Lambda<FieldModifier<object, T>>(
                    Expression.Assign(fieldExp, valueExp),
                    targetExp, valueExp).Compile();
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        /// <summary>
        /// 创建一个修改字段值的委托
        /// </summary>
        /// <typeparam name="TClass">拥有字段的类型</typeparam>
        /// <typeparam name="TValue">将给字段赋的值的类型</typeparam>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>修改字段值的委托。</returns>
        /// <exception cref="MissingFieldException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidOperationException">当字段为 <see langword="readonly"/> 时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法 <typeparamref name="TValue"/> 类型转化为将字段的类型时发生。</exception>
        public static FieldModifier<TClass, TValue> CreateFieldModifier<TClass, TValue>(
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type type = typeof(TClass);

            if (type.GetField(name, flags) is FieldInfo field)
            {
                if (field.IsInitOnly)
                {
                    throw new InvalidOperationException($"Can not create modifier for {type}.{field.Name} - it is read only.");
                }

                Type fieldType = field.FieldType;
                Type valueType = typeof(TValue);

                if (!CanConvert(valueType, fieldType, out var needConvert))
                {
                    throw new InvalidCastException($"Cannot cast type: {valueType} to type: {fieldType}.");
                }

                ParameterExpression targetExp = Expression.Parameter(type, "target");
                ParameterExpression valueExp = Expression.Parameter(valueType, "value");
                MemberExpression fieldExp = Expression.Field(
                    field.IsStatic ? null : targetExp,
                    field);

                if (needConvert)
                {
                    return Expression.Lambda<FieldModifier<TClass, TValue>>(
                        Expression.Assign(fieldExp, Expression.Convert(valueExp, fieldType)),
                        targetExp, valueExp).Compile();
                }

                return Expression.Lambda<FieldModifier<TClass, TValue>>(
                    Expression.Assign(fieldExp, valueExp),
                    targetExp, valueExp).Compile();
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
                return field.GetValue<T>(obj);
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

            if (!CanConvert(fieldType, targetType, out var needConvert))
            {
                throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
            }

            if (needConvert)
            {
                return (T)Convert.ChangeType(field.GetValue(obj), typeof(T), CultureInfo.InvariantCulture);
            }

            return (T)field.GetValue(obj);
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
            this object obj,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type classType = obj.GetType();

            if (classType.GetField(name, flags) is FieldInfo field)
            {
                return field.GetValue<T>(obj);
            }
            else
            {
                throw new MissingFieldException(classType.Name, name);
            }
        }

        /// <summary>
        /// 设置字段的值
        /// </summary>
        /// <typeparam name="T">赋给字段的值的类型</typeparam>
        /// <param name="type">拥有字段的类型</param>
        /// <param name="name">字段名称</param>
        /// <param name="obj">拥有字段的实例</param>
        /// <param name="value">要赋给字段的值</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将 <paramref name="value"/> 转化为字段的类型时发生。</exception>
        public static void SetValue<T>(
            this Type type,
            string name,
            object obj,
            T value,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetField(name, flags) is FieldInfo field)
            {
                SetValue(field, obj, value);
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        /// <summary>
        /// 设置字段的值
        /// </summary>
        /// <typeparam name="T">赋给字段的值的类型</typeparam>
        /// <param name="field">字段信息</param>
        /// <param name="obj">拥有字段的实例</param>
        /// <param name="value">要赋给字段的值</param>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将 <paramref name="value"/> 转化为字段的类型时发生。</exception>
        public static void SetValue<T>(this FieldInfo field, object obj, T value)
        {
            Type fieldType = field.FieldType;
            Type sourceType = typeof(T);

            if (!CanConvert(sourceType, fieldType, out var needConvert))
            {
                throw new InvalidCastException($"Cannot cast type: {sourceType} to type: {fieldType}.");
            }

            if (needConvert)
            {
                field.SetValue(obj, Convert.ChangeType(value, fieldType));
            }
            else
            {
                field.SetValue(obj, value);
            }
        }

        /// <summary>
        /// 设置字段的值
        /// </summary>
        /// <typeparam name="T">赋给字段的值的类型</typeparam>
        /// <param name="obj">拥有字段的实例</param>
        /// <param name="name">字段名称</param>
        /// <param name="value">要赋给字段的值</param>
        /// <param name="flags">搜索字段的方式</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将 <paramref name="value"/> 转化为字段的类型时发生。</exception>
        public static void SetValue<T>(
            this object obj,
            string name,
            T value,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type classType = obj.GetType();

            if (classType.GetField(name, flags) is FieldInfo field)
            {
                SetValue(field, obj, value);
            }
            else
            {
                throw new MissingFieldException(classType.Name, name);
            }
        }


        private static bool CanConvert(Type sourceType, Type targetType, out bool needConvert)
        {
            if (sourceType == targetType)
            {
                needConvert = false;
                return true;
            }

            if (SystemConvertUtility.CanConvert(sourceType, targetType))
            {
                needConvert = true;
                return true;
            }

            needConvert = false;
            return false;
        }
    }
}
