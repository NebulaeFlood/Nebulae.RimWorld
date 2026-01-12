using System;
using System.Runtime.CompilerServices;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 哈希帮助类
    /// </summary>
    public static class HashUtility
    {
        /// <summary>
        /// 最大哈希表大小
        /// </summary>
        public const int MaxSize = 0x7FFFFFC3;


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

#if TARGET_64BIT
        /// <summary>
        /// 计算快速取模运算的乘数
        /// </summary>
        /// <param name="divisor">除数</param>
        /// <returns>快速取模运算的乘数。</returns>
        /// <remarks>仅适用于 64 位系统。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong CalculateMultipier(uint divisor)
        {
            return ulong.MaxValue / divisor + 1;
        }
#endif

        /// <summary>
        /// 计算下一个合适的质数哈希表大小
        /// </summary>
        /// <param name="oldSize">哈希表当前的大小</param>
        /// <returns>新的质数哈希表大小。</returns>
        /// <remarks>新大小将不会大于 <see cref="MaxSize"/>。</remarks>
        public static int Expand(int oldSize)
        {
            if (oldSize >= MaxSize)
            {
                return MaxSize;
            }

            int newSize = Math.Max(
                oldSize + Math.Max(4, oldSize / 4),
                (int)Math.Ceiling(oldSize * GoldenRatio));

            if ((uint)newSize > MaxSize)
            {
                return MaxSize;
            }

            return GetPrime(newSize);
        }

        /// <summary>
        /// 获取适合哈希表的质数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <returns>大于 <paramref name="min"/> 的适合哈希表的质数。</returns>
        /// <remarks>当找不到适合的质数时，返回 <see cref="MaxSize"/>。</remarks>
        public static int GetPrime(int min)
        {
            for (int i = 0; i < Count; i++)
            {
                var num = Primes[i];

                if (num > min)
                {
                    return num;
                }
            }

#if TARGET_64BIT
            for (int i = min | 1; i < MaxSize; i += 2)
            {
                if (IsPrime(i) && Modulo((uint)(i - 1u), HashPrime, HashPrimeMultiplier) != 0)
                {
                    return i;
                }
            }
#else
            for (int i = min | 1; i < MaxSize; i += 2)
            {
                if (IsPrime(i) && ((i - 1) % 101 != 0))
                {
                    return i;
                }
            }
#endif

            return MaxSize;
        }

        /// <summary>
        /// 判断一个数是否为质数
        /// </summary>
        /// <param name="candidate">要判断的数</param>
        /// <returns>若 <paramref name="candidate"/> 是质数，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) is 0)
            {
                return candidate is 2;
            }

            int limit = (int)Math.Sqrt(candidate);

            for (int divisor = 3; divisor <= limit; divisor += 2)
            {
                if ((candidate % divisor) is 0)
                {
                    return false;
                }
            }

            return true;
        }

#if TARGET_64BIT
        /// <summary>
        /// 快速取模运算
        /// </summary>
        /// <param name="value">要取模的值</param>
        /// <param name="divisor">除数</param>
        /// <param name="multiplier">快速取模运算的乘数</param>
        /// <returns>对 <paramref name="value"/> 取模后的结果。</returns>
        /// <remarks>仅适用于 64 位系统。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Modulo(uint value, uint divisor, ulong multiplier)
        {
            return (uint)(((((multiplier * value) >> 32) + 1) * divisor) >> 32);
        }
#endif

        #endregion


        //------------------------------------------------------
        //
        //  Private Constants
        //
        //------------------------------------------------------

        #region Private Constants

        private const int Count = 72;

        private const int HashPrime = 101;
        private const ulong HashPrimeMultiplier = ulong.MaxValue / 101ul + 1;

        private const double GoldenRatio = 1.618033988749895;

        #endregion


        private static readonly int[] Primes = new int[Count]
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71,
            89, 107, 131, 163, 197, 239, 293, 353, 431, 521,
            631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371,
            4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
            25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363,
            156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
            968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559,
            5999471, 7199369
        };
    }
}
