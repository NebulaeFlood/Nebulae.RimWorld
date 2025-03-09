using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Utilities
{
    /// <summary>
    /// 静态字段访问器
    /// </summary>
    /// <typeparam name="T">字段类型</typeparam>
    /// <returns>对应的字段的值。</returns>
    public delegate T FieldAccessor<out T>();

    /// <summary>
    /// 字段访问器
    /// </summary>
    /// <typeparam name="TClass">拥有字段的类型</typeparam>
    /// <typeparam name="TValue">字段类型</typeparam>
    /// <param name="obj">目标实例</param>
    /// <returns>对应的字段的值。</returns>
    public delegate TValue FieldAccessor<in TClass, out TValue>(TClass obj);

    /// <summary>
    /// 静态字段修改器
    /// </summary>
    /// <typeparam name="T">字段类型</typeparam>
    /// <param name="value">要设置的值</param>
    public delegate void FieldModifier<in T>(T value);

    /// <summary>
    /// 字段修改器
    /// </summary>
    /// <typeparam name="TClass">拥有字段的类型</typeparam>
    /// <typeparam name="TValue">字段类型</typeparam>
    /// <param name="obj">目标实例</param>
    /// <param name="value">要设置的值</param>
    public delegate void FieldModifier<in TClass, in TValue>(TClass obj, TValue value);


    /// <summary>
    /// 字段工具类
    /// </summary>
    public static class FieldUtility
    {
        //------------------------------------------------------
        //
        //  Create Accesor
        //
        //------------------------------------------------------

        #region Create Accesor

        /// <summary>
        /// 创建一个获取字段值的委托
        /// </summary>
        /// <typeparam name="TClass">拥有字段的类型</typeparam>
        /// <typeparam name="TValue">获取的字段值将转化成的类型</typeparam>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>获取字段值的委托。</returns>
        /// <exception cref="MissingFieldException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidOperationException">字段为静态字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段的类型转化为 <typeparamref name="TValue"/> 类型时发生。</exception>
        public static FieldAccessor<TClass, TValue> CreateFieldAccessor<TClass, TValue>(
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type type = typeof(TClass);

            if (type.GetField(name, flags) is FieldInfo field)
            {
                if (field.IsStatic)
                {
                    throw new InvalidOperationException($"{type}.{name} is static, call {typeof(FieldUtility)}.CreateStaticFieldAccessor instead.");
                }

                Type fieldType = field.FieldType;
                Type targetType = typeof(TValue);

                if (!MemberUtility.CanConvert(fieldType, targetType, out var convertOperation))
                {
                    throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
                }

                ParameterExpression targetExp = Expression.Parameter(type, "target");
                MemberExpression fieldExp = Expression.Field(targetExp, field);

                if (convertOperation)
                {
                    return Expression.Lambda<FieldAccessor<TClass, TValue>>(
                        Expression.Convert(fieldExp, targetType),
                        targetExp).Compile();
                }

                return Expression.Lambda<FieldAccessor<TClass, TValue>>(
                    fieldExp,
                    targetExp).Compile();
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        /// <summary>
        /// 创建一个获取静态字段值的委托
        /// </summary>
        /// <typeparam name="T">获取的字段值将转化成的类型</typeparam>
        /// <param name="type">拥有字段的类型</param>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>获取字段值的委托。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="type"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidOperationException">字段不是静态字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将 <typeparamref name="T"/> 类型转化为字段的类型时发生。</exception>
        public static FieldAccessor<T> CreateStaticFieldAccessor<T>(
            this Type type,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetField(name, flags) is FieldInfo field)
            {
                if (!field.IsStatic)
                {
                    throw new InvalidOperationException($"{type}.{name} is not static, call {typeof(FieldUtility)}.CreateFieldAccessor instead.");
                }

                Type fieldType = field.FieldType;
                Type targetType = typeof(T);

                if (!MemberUtility.CanConvert(fieldType, targetType, out var convertOperation))
                {
                    throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
                }
                MemberExpression fieldExp = Expression.Field(null, field);

                if (convertOperation)
                {
                    return Expression.Lambda<FieldAccessor<T>>(
                        Expression.Convert(fieldExp, targetType))
                        .Compile();
                }

                return Expression.Lambda<FieldAccessor<T>>(fieldExp).Compile();
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Create Modifier
        //
        //------------------------------------------------------

        #region Create Modifier

        /// <summary>
        /// 创建一个修改字段值的委托
        /// </summary>
        /// <typeparam name="TClass">拥有字段的类型</typeparam>
        /// <typeparam name="TValue">将给字段赋的值的类型</typeparam>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索成员的方式</param>
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

                if (!MemberUtility.CanConvert(valueType, fieldType, out var convertOperation))
                {
                    throw new InvalidCastException($"Cannot cast type: {valueType} to type: {fieldType}.");
                }

                ParameterExpression targetExp = Expression.Parameter(type, "target");
                ParameterExpression valueExp = Expression.Parameter(valueType, "value");
                MemberExpression fieldExp = Expression.Field(
                    field.IsStatic ? null : targetExp,
                    field);

                if (convertOperation)
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

        #endregion


        //------------------------------------------------------
        //
        //  Get Value
        //
        //------------------------------------------------------

        #region Get Value

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">字段值将转化成的类型</typeparam>
        /// <param name="type">拥有字段的类型</param>
        /// <param name="name">字段名称</param>
        /// <param name="obj">目标实例</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段值转化为 <typeparamref name="T"/> 类型时发生。</exception>
        public static T GetFieldValue<T>(
            this Type type,
            string name,
            object obj,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetField(name, flags) is FieldInfo field)
            {
                return GetValue<T>(field, obj);
            }
            else
            {
                throw new MissingFieldException(type.Name, name);
            }
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="TClass">拥有字段的类型</typeparam>
        /// <typeparam name="TValue">字段值将转化成的类型</typeparam>
        /// <param name="obj">目标实例</param>
        /// <param name="name">字段名称</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>转化为 <typeparamref name="TValue"/> 类型的字段值。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段值转化为 <typeparamref name="TValue"/> 类型时发生。</exception>
        public static TValue GetFieldValue<TClass, TValue>(
            TClass obj,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type classType = typeof(TClass);

            if (classType.GetField(name, flags) is FieldInfo field)
            {
                return GetValue<TValue>(field, obj);
            }
            else
            {
                throw new MissingFieldException(classType.Name, name);
            }
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">字段值将转化成的类型</typeparam>
        /// <param name="field">字段信息</param>
        /// <param name="obj">目标实例</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值。</returns>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将字段值转化为 <typeparamref name="T"/> 类型时发生。</exception>
        public static T GetValue<T>(this FieldInfo field, object obj)
        {
            Type fieldType = field.FieldType;
            Type targetType = typeof(T);

            if (!MemberUtility.CanConvert(fieldType, targetType, out var convertOperation))
            {
                throw new InvalidCastException($"Cannot cast type: {fieldType} to type: {targetType}.");
            }

            if (convertOperation)
            {
                return (T)Convert.ChangeType(field.GetValue(obj), targetType);
            }

            return (T)field.GetValue(obj);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Set Value
        //
        //------------------------------------------------------

        #region Set Value

        /// <summary>
        /// 设置字段的值
        /// </summary>
        /// <typeparam name="T">赋给字段的值的类型</typeparam>
        /// <param name="type">拥有字段的类型</param>
        /// <param name="name">字段名称</param>
        /// <param name="obj">目标实例</param>
        /// <param name="value">要设置的值</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的字段值。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        public static void SetFieldValue<T>(
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
                throw new MissingFieldException(type.FullName, name);
            }
        }

        /// <summary>
        /// 设置字段的值
        /// </summary>
        /// <typeparam name="TClass">拥有字段的类型</typeparam>
        /// <typeparam name="TValue">赋给字段的值的类型</typeparam>
        /// <param name="obj">目标实例</param>
        /// <param name="name">字段名称</param>
        /// <param name="value">要设置的值</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <exception cref="MissingFieldException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的字段时发生。</exception>
        public static void SetFieldValue<TClass, TValue>(
            TClass obj,
            string name,
            TValue value,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type classType = typeof(TClass);

            if (classType.GetField(name, flags) is FieldInfo field)
            {
                SetValue(field, obj, value);
            }
            else
            {
                throw new MissingFieldException(classType.FullName, name);
            }
        }

        /// <summary>
        /// 设置字段的值
        /// </summary>
        /// <typeparam name="T">赋给字段的值的类型</typeparam>
        /// <param name="field">字段信息</param>
        /// <param name="obj">目标实例</param>
        /// <param name="value">要设置的值</param>
        public static void SetValue<T>(this FieldInfo field, object obj, T value)
        {
            Type fieldType = field.FieldType;
            Type sourceType = typeof(T);

            if (!MemberUtility.CanConvert(sourceType, fieldType, out var convertOperation))
            {
                throw new InvalidCastException($"Cannot cast type: {sourceType} to type: {fieldType}.");
            }

            if (convertOperation)
            {
                field.SetValue(obj, Convert.ChangeType(value, fieldType));
            }
            else
            {
                field.SetValue(obj, value);
            }
        }

        #endregion
    }
}
