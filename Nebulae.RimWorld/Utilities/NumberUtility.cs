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
        /// 将浮点数舍入到指定小数位数
        /// </summary>
        /// <param name="value">要舍入的浮点数</param>
        /// <param name="decimals">要保留的小数位数</param>
        /// <returns>舍入到指定小数位数的浮点数。</returns>
        public static float Round(this float value, int decimals)
        {
            if (decimals <= 0)
            {
                return (float)Math.Round(value);
            }
            else if (decimals > 7)
            {
                decimals = 7;
            }

            var multiplier = Multipliers[decimals];
            return (float)Math.Round(value * multiplier) / multiplier;
        }


        private static readonly float[] Multipliers =
        {
            1f,
            10f,
            100f,
            1000f,
            10000f,
            100000f,
            1000000f,
            10000000f,
        };
    }
}
