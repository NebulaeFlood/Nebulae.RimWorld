using System;
using UnityEngine;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 数字类型的帮助类
    /// </summary>
    public static class NumberUtility
    {
        /// <summary>
        /// 对数字向上取整
        /// </summary>
        /// <param name="value">要取整的数</param>
        /// <returns>向上取整后的数字</returns>
        public static float Ceiling(this float value) => Mathf.Ceil(value);

        /// <summary>
        /// 限定数字到指定范围
        /// </summary>
        /// <param name="value">要限定的数</param>
        /// <param name="min">数的最小值</param>
        /// <param name="max">数的最大值</param>
        /// <returns>限定后的数字</returns>
        public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);

        /// <summary>
        /// 限定数字到指定范围
        /// </summary>
        /// <param name="value">要限定的数</param>
        /// <param name="min">数的最小值</param>
        /// <param name="max">数的最大值</param>
        /// <returns>限定后的数字</returns>
        public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);

        /// <summary>
        /// 限定数字到指定范围
        /// </summary>
        /// <typeparam name="T">数字类型</typeparam>
        /// <param name="value">要限定的数</param>
        /// <param name="min">数的最小值</param>
        /// <param name="max">数的最大值</param>
        /// <returns>限定后的数字</returns>
        public static T Clamp<T>(this T value, T min, T max) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            else if (value.CompareTo(max) > 0)
            {
                return max;
            }
            return value;
        }

        /// <summary>
        /// 对数字向下取整
        /// </summary>
        /// <param name="value">要取整的数</param>
        /// <returns>向下取整后的数字</returns>
        public static float Floor(this float value) => Mathf.Floor(value);

        /// <summary>
        /// 判断数字是否为无穷
        /// </summary>
        /// <param name="value">要判断的数</param>
        /// <returns>数字是否为无穷</returns>
        public static bool IsInfinity(this float value) => float.IsInfinity(value);

        /// <summary>
        /// 判断数字是否在指定范围内
        /// </summary>
        /// <typeparam name="T">数字类型</typeparam>
        /// <param name="value">要判断的数</param>
        /// <param name="min">范围的最小值</param>
        /// <param name="max">范围的最大值</param>
        /// <returns>数字是否在指定范围内</returns>
        public static bool IsInRange<T>(this T value, T min, T max) where T : struct, IComparable<T>
        {
            return value.CompareTo(min) >= -1 && value.CompareTo(max) <= 1;
        }

        /// <summary>
        /// 将字符串转化为浮点数
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defaultValue">转换失败时返回的默认值</param>
        /// <returns>转换后的数字</returns>
        public static float Prase(this string str, float defaultValue)
        {
            if (!float.TryParse(str, out float value))
            {
                if (string.IsNullOrEmpty(str))
                {
                    value = 0f;
                }
                else
                {
                    value = defaultValue;
                }
            }

            return value;
        }

        /// <summary>
        /// 将无穷替换为指定值
        /// </summary>
        /// <param name="value">可能被替换的数</param>
        /// <param name="specificValue">替换无穷的数</param>
        /// <returns>如果 <paramref name="value"/> 为无穷，返回 <paramref name="specificValue"/>；反之则返回 <paramref name="value"/>。</returns>
        public static float ReplaceInfinityWith(this float value, float specificValue)
        {
            return float.IsInfinity(value) ? specificValue : value;
        }
    }
}
