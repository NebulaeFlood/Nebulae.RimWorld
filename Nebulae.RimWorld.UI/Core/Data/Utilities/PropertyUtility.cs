using System;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Core.Data.Utilities
{
    /// <summary>
    /// 静态属性访问器
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    /// <returns>对应的属性的值。</returns>
    public delegate T PropertyAccessor<out T>();

    /// <summary>
    /// 属性访问器
    /// </summary>
    /// <typeparam name="TClass">拥有属性的类型</typeparam>
    /// <typeparam name="TValue">属性类型</typeparam>
    /// <param name="obj">拥有属性的实例</param>
    /// <returns>对应的属性的值。</returns>
    public delegate TValue PropertyAccessor<in TClass, out TValue>(TClass obj);

    /// <summary>
    /// 静态属性修改器
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    /// <param name="value">要赋给属性的值</param>
    public delegate void PropertyModifier<in T>(T value);

    /// <summary>
    /// 属性修改器
    /// </summary>
    /// <typeparam name="TClass">拥有属性的类型</typeparam>
    /// <typeparam name="TValue">属性类型</typeparam>
    /// <param name="obj">拥有属性的实例</param>
    /// <param name="value">要赋给属性的值</param>
    public delegate void PropertyModifier<in TClass, in TValue>(TClass obj, TValue value);

    /// <summary>
    /// 属性工具类
    /// </summary>
    public static class PropertyUtility
    {
        /// <summary>
        /// 创建一个获取属性值的委托
        /// </summary>
        /// <typeparam name="TClass">拥有属性的类型</typeparam>
        /// <typeparam name="TValue">属性值的类型</typeparam>
        /// <param name="name">属性名称</param>
        /// <param name="flags">搜索属性的方式</param>
        /// <returns>获取属性值的委托。</returns>
        /// <exception cref="MissingMemberException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的属性时发生。</exception>
        /// <exception cref="InvalidOperationException">当属性没有 <see langword="get"/> 访问器或属性为静态属性时发生。</exception>
        /// <exception cref="InvalidCastException">当属性类型不是 <typeparamref name="TValue"/> 时发生。</exception>
        public static PropertyAccessor<TClass, TValue> CreatePropertyAccessor<TClass, TValue>(
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type type = typeof(TClass);

            if (type.GetProperty(name, flags) is PropertyInfo property)
            {
                if (property.GetMethod is MethodInfo method)
                {
                    Type propertyType = property.PropertyType;
                    Type targetType = typeof(TValue);

                    if (propertyType != targetType)
                    {
                        throw new InvalidCastException($"Property type: {propertyType} must match with target type: {targetType}.");
                    }

                    if (method.IsStatic)
                    {
                        throw new ArgumentException($"{type}.{name} is static, call {typeof(PropertyUtility)}.CreateStaticPropertyAccessor instead.");
                    }

                    return (PropertyAccessor<TClass, TValue>)method.CreateDelegate(typeof(PropertyAccessor<TClass, TValue>));
                }
                else
                {
                    throw new MissingMethodException($"Property {type.Name}.{name} has no getter.");
                }
            }
            else
            {
                throw new MissingMemberException(type.FullName, name);
            }
        }

        /// <summary>
        /// 创建一个获取静态属性值的委托
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="type">拥有属性的类型</param>
        /// <param name="name">属性名称</param>
        /// <param name="flags">搜索属性的方式</param>
        /// <returns>获取属性值的委托。</returns>
        /// <exception cref="MissingMemberException">当无法在 <paramref name="type"/> 中找到名为 <paramref name="name"/> 的属性时发生。</exception>
        /// <exception cref="InvalidOperationException">当属性没有 <see langword="get"/> 访问器或属性不是静态属性时发生。</exception>
        /// <exception cref="InvalidCastException">当属性类型不是 <typeparamref name="T"/> 时发生。</exception>
        public static PropertyAccessor<T> CreateStaticPropertyAccessor<T>(
            this Type type,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetProperty(name, flags) is PropertyInfo property)
            {
                if (property.GetMethod is MethodInfo method)
                {
                    Type propertyType = property.PropertyType;
                    Type targetType = typeof(T);

                    if (propertyType != targetType)
                    {
                        throw new InvalidCastException($"Property type: {propertyType} must match with target type: {targetType}.");
                    }

                    if (!method.IsStatic)
                    {
                        throw new ArgumentException($"{type}.{name} is not static, call {typeof(PropertyUtility)}.CreatePropertyAccessor instead.");
                    }

                    return (PropertyAccessor<T>)method.CreateDelegate(typeof(PropertyAccessor<T>));
                }
                else
                {
                    throw new MissingMethodException($"Property {type.Name}.{name} has no getter.");
                }
            }
            else
            {
                throw new MissingMemberException(type.FullName, name);
            }
        }

        /// <summary>
        /// 创建一个修改属性值的委托
        /// </summary>
        /// <typeparam name="TClass">拥有属性的类型</typeparam>
        /// <typeparam name="TValue">属性值的类型</typeparam>
        /// <param name="name">属性名称</param>
        /// <param name="flags">搜索属性的方式</param>
        /// <returns>修改属性值的委托。</returns>
        /// <exception cref="MissingMemberException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的属性时发生。</exception>
        /// <exception cref="InvalidOperationException">当属性没有 set 访问器或属性为静态属性时发生。</exception>
        /// <exception cref="InvalidCastException">当 <typeparamref name="TValue"/> 不是属性的类型时发生。</exception>
        public static PropertyModifier<TClass, TValue> CreatePropertyModifier<TClass, TValue>(
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type type = typeof(TClass);

            if (type.GetProperty(name, flags) is PropertyInfo property)
            {
                if (property.SetMethod is MethodInfo method)
                {
                    Type propertyType = property.PropertyType;
                    Type sourceType = typeof(TValue);

                    if (propertyType != sourceType)
                    {
                        throw new InvalidCastException($"Property type: {propertyType} must match with source type: {sourceType}.");
                    }

                    if (method.IsStatic)
                    {
                        throw new ArgumentException($"{type}.{name} is static, call {typeof(PropertyUtility)}.CreateStaticPropertyModifier instead.");
                    }

                    return (PropertyModifier<TClass, TValue>)method.CreateDelegate(typeof(PropertyModifier<TClass, TValue>));
                }
                else
                {
                    throw new MissingMethodException($"Property {type.Name}.{name} has no setter.");
                }
            }
            else
            {
                throw new MissingMemberException(type.FullName, name);
            }
        }

        /// <summary>
        /// 创建一个修改静态属性值的委托
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="type">拥有属性的类型</param>
        /// <param name="name">属性名称</param>
        /// <param name="flags">搜索属性的方式</param>
        /// <returns>修改属性值的委托。</returns>
        /// <exception cref="MissingMemberException">当无法在 <paramref name="type"/> 中找到名为 <paramref name="name"/> 的属性时发生。</exception>
        /// <exception cref="InvalidOperationException">当属性没有 set 访问器或属性不是静态属性时发生。</exception>
        /// <exception cref="InvalidCastException">当 <typeparamref name="T"/> 不是属性的类型时发生。</exception>
        public static PropertyModifier<T> CreateStaticPropertyModifier<T>(
            this Type type,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetProperty(name, flags) is PropertyInfo property)
            {
                if (property.SetMethod is MethodInfo method)
                {
                    Type propertyType = property.PropertyType;
                    Type sourceType = typeof(T);

                    if (propertyType != sourceType)
                    {
                        throw new InvalidCastException($"Property type: {propertyType} must match with source type: {sourceType}.");
                    }

                    if (!method.IsStatic)
                    {
                        throw new ArgumentException($"{type}.{name} is not static, call {typeof(PropertyUtility)}.CreatePropertyModifier instead.");
                    }

                    return (PropertyModifier<T>)method.CreateDelegate(typeof(PropertyModifier<T>));
                }
                else
                {
                    throw new MissingMethodException($"Property {type.Name}.{name} has no setter.");
                }
            }
            else
            {
                throw new MissingMemberException(type.FullName, name);
            }
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <typeparam name="T">字段值将转化成的类型</typeparam>
        /// <param name="type">拥有属性的类型</param>
        /// <param name="name">属性名称</param>
        /// <param name="obj">目标实例</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的属性值。</returns>
        /// <exception cref="MissingFieldException">当无法在 <paramref name="obj"/> 中找到名为 <paramref name="name"/> 的属性时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将属性值转化为 <typeparamref name="T"/> 类型时发生。</exception>
        public static T GetPropertyValue<T>(
            this Type type,
            string name,
            object obj,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetProperty(name, flags) is PropertyInfo property)
            {
                return property.GetValue<T>(obj);
            }
            else
            {
                throw new MissingMemberException(type.FullName, name);
            }
        }

        //------------------------------------------------------
        //
        //  Get Value
        //
        //------------------------------------------------------

        #region Get Value

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="TClass">拥有属性的类型</typeparam>
        /// <typeparam name="TValue">返回值的类型</typeparam>
        /// <param name="obj">目标实例</param>
        /// <param name="name">属性名称</param>
        /// <param name="flags">绑定标志</param>
        /// <returns>转化为 <typeparamref name="TValue"/> 类型的属性值。</returns>
        /// <exception cref="MissingMemberException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的属性时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将属性值转化为 <typeparamref name="TValue"/> 时发生。</exception>
        public static TValue GetPropertyValue<TClass, TValue>(
            TClass obj,
            string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type type = typeof(TClass);

            if (type.GetProperty(name, flags) is PropertyInfo property)
            {
                return property.GetValue<TValue>(obj);
            }
            else
            {
                throw new MissingMemberException(type.FullName, name);
            }
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <typeparam name="T">属性值将转化成的类型</typeparam>
        /// <param name="property">属性信息</param>
        /// <param name="obj">目标实例</param>
        /// <returns>转化为 <typeparamref name="T"/> 类型的属性值。</returns>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将属性值转化为 <typeparamref name="T"/> 类型时发生。</exception>
        public static T GetValue<T>(this PropertyInfo property, object obj)
        {
            Type propertyType = property.PropertyType;
            Type targetType = typeof(T);

            if (!MemberUtility.CanConvert(property.PropertyType, targetType, out var convertOperation))
            {
                throw new InvalidCastException($"Cannot cast type: {propertyType} to type: {targetType}.");
            }

            if (convertOperation)
            {
                return (T)Convert.ChangeType(property.GetValue(obj), targetType);
            }

            return (T)property.GetValue(obj);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Set Value
        //
        //------------------------------------------------------

        #region Set Value

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <typeparam name="T">赋给属性的值的类型</typeparam>
        /// <param name="type">拥有属性的类型</param>
        /// <param name="name">属性名称</param>
        /// <param name="obj">拥有属性的实例</param>
        /// <param name="value">要赋给属性的值</param>
        /// <param name="flags">搜索属性的方式</param>
        /// <exception cref="MissingMemberException">当无法在 <paramref name="type"/> 中找到名为 <paramref name="name"/> 的属性时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将 <paramref name="value"/> 转化为属性的类型时发生。</exception>
        public static void SetPropertyValue<T>(
            this Type type,
            string name,
            object obj,
            T value,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (type.GetProperty(name, flags) is PropertyInfo property)
            {
                SetValue(property, obj, value);
            }
            else
            {
                throw new MissingMemberException(type.FullName, name);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="TClass">拥有属性的类型</typeparam>
        /// <typeparam name="TValue">赋给属性的值的类型</typeparam>
        /// <param name="obj">目标实例</param>
        /// <param name="name">属性名称</param>
        /// <param name="value">要设置的值</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <exception cref="MissingMemberException">当无法在 <typeparamref name="TClass"/> 中找到名为 <paramref name="name"/> 的属性时发生。</exception>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将 <paramref name="value"/> 转化为属性的类型时发生。</exception>
        public static void SetPropertyValue<TClass, TValue>(
            this TClass obj,
            string name,
            TValue value,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type type = typeof(TClass);

            if (type.GetProperty(name, flags) is PropertyInfo property)
            {
                SetValue(property, obj, value);
            }
            else
            {
                throw new MissingMemberException(type.FullName, name);
            }
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <typeparam name="T">赋给属性的值的类型</typeparam>
        /// <param name="property">属性信息</param>
        /// <param name="obj">拥有属性的实例</param>
        /// <param name="value">要赋给属性的值</param>
        /// <exception cref="InvalidCastException">当 <see cref="Convert"/> 无法将 <paramref name="value"/> 转化为属性的类型时发生。</exception>
        public static void SetValue<T>(this PropertyInfo property, object obj, T value)
        {
            Type propertyType = property.PropertyType;
            Type sourceType = typeof(T);

            if (!MemberUtility.CanConvert(sourceType, propertyType, out var convertOperation))
            {
                throw new InvalidCastException($"Cannot cast type: {sourceType} to type: {propertyType}.");
            }

            if (convertOperation)
            {
                property.SetValue(obj, Convert.ChangeType(value, propertyType));
            }
            else
            {
                property.SetValue(obj, value);
            }
        }

        #endregion
    }
}