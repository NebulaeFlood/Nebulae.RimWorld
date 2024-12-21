using System;
using UnityEngine;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 描述边框的粗细
    /// </summary>
    public struct Thickness : IEquatable<Thickness>
    {
        /// <summary>
        /// <see cref="Left"/>、<see cref="Top"/>、<see cref="Right"/> 和 <see cref="Bottom"/> 为 0 的 <see cref="Thickness"/>
        /// </summary>
        public static readonly Thickness Empty = new Thickness(0f);


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取此 <see cref="Thickness"/> 结构的左边距
        /// </summary>
        public readonly float Left;
        /// <summary>
        /// 获取此 <see cref="Thickness"/> 结构的上边距
        /// </summary>
        public readonly float Top;  
        /// <summary>
        /// 获取此 <see cref="Thickness"/> 结构的右边距
        /// </summary>
        public readonly float Right;
        /// <summary>
        /// 获取此 <see cref="Thickness"/> 结构的下边距
        /// </summary>
        public readonly float Bottom;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="Thickness"/> 结构的新实例
        /// </summary>
        /// <param name="uniformLength">边框统一尺寸</param>
        /// <remarks>此结构的各边框使用统一尺寸。</remarks>
        public Thickness(float uniformLength)
        {
            Left = Top = Right = Bottom = uniformLength;
        }

        /// <summary>
        /// 初始化 <see cref="Thickness"/> 结构的新实例
        /// </summary>
        /// <param name="left">左边框尺寸</param>
        /// <param name="top">上边框尺寸</param>
        /// <param name="right">右边框尺寸</param>
        /// <param name="bottom">下边框尺寸</param>
        public Thickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断指定的 <see cref="Thickness"/> 是否等于此 <see cref="Thickness"/>
        /// </summary>
        /// <param name="other">要判断的 <see cref="Thickness"/></param>
        /// <returns>
        /// 如果此 <see cref="Thickness"> 的 <see cref="Left"/>, <see cref="Top"/>, <see cref="Right"/> 和 <see cref="Bottom"/> 与 <paramref name="other"/> 一致，
        /// 则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。
        /// </returns>
        public bool Equals(Thickness other)
        {
            return this == other;
        }

        /// <summary>
        /// 判断指定的对象是否等于此 <see cref="Thickness"/>
        /// </summary>
        /// <param name="obj">要判断的对象</param>
        /// <returns>如果指定的对象等于此 <see cref="Thickness"> ，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Thickness other && this == other;
        }

        /// <summary>
        /// 获取此 <see cref="Thickness"/> 的哈希代码
        /// </summary>
        /// <returns>此 <see cref="Thickness"/> 的哈希代码</returns>
        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串</returns>
        public override string ToString()
        {
            return $"{{Left:{Left}, Top:{Top}, Right:{Right}, Bottom:{Bottom}}}";
        }

        #endregion


        //------------------------------------------------------
        //
        //  Operators
        //
        //------------------------------------------------------

        #region Operators

        public static implicit operator Thickness(float uniformLength) => new Thickness(uniformLength);

        public static bool operator ==(Thickness thickness, float uniformLength) => thickness.Left == uniformLength && thickness.Top == uniformLength && thickness.Right == uniformLength && thickness.Bottom == uniformLength;
        public static bool operator !=(Thickness thickness, float uniformLength) => thickness.Left != uniformLength || thickness.Top != uniformLength || thickness.Right != uniformLength || thickness.Bottom != uniformLength;

        public static bool operator ==(float uniformLength, Thickness thickness) => thickness.Left == uniformLength && thickness.Top == uniformLength && thickness.Right == uniformLength && thickness.Bottom == uniformLength;
        public static bool operator !=(float uniformLength, Thickness thickness) => thickness.Left != uniformLength || thickness.Top != uniformLength || thickness.Right != uniformLength || thickness.Bottom != uniformLength;

        public static bool operator ==(Thickness left, Thickness right) => left.Left == right.Left && left.Top == right.Top && left.Right == right.Right && left.Bottom == right.Bottom;
        public static bool operator !=(Thickness left, Thickness right) => left.Left != right.Left || left.Top != right.Top || left.Right != right.Right || left.Bottom != right.Bottom;

        public static bool operator >(Thickness thickness, float uniformLength) => thickness.Left > uniformLength && thickness.Top > uniformLength && thickness.Right > uniformLength && thickness.Bottom > uniformLength;
        public static bool operator <(Thickness thickness, float uniformLength) => thickness.Left < uniformLength && thickness.Top < uniformLength && thickness.Right < uniformLength && thickness.Bottom < uniformLength;

        public static bool operator >(float uniformLength, Thickness thickness) => thickness.Left > uniformLength && thickness.Top > uniformLength && thickness.Right > uniformLength && thickness.Bottom > uniformLength;
        public static bool operator <(float uniformLength, Thickness thickness) => thickness.Left < uniformLength && thickness.Top < uniformLength && thickness.Right < uniformLength && thickness.Bottom < uniformLength;

        public static bool operator >(Thickness left, Thickness right) => left.Left > right.Left && left.Top > right.Top && left.Right > right.Right && left.Bottom > right.Bottom;
        public static bool operator <(Thickness left, Thickness right) => left.Left < right.Left && left.Top < right.Top && left.Right < right.Right && left.Bottom < right.Bottom;

        public static Thickness operator +(Thickness left, Thickness right) => Add(left, right);
        public static Thickness operator -(Thickness left, Thickness right) => Subtract(left, right);
        public static Thickness operator *(Thickness thickness, float multiplier) => Multiply(thickness, multiplier);
        public static Thickness operator /(Thickness thickness, float divisor) => Multiply(thickness, 1f / divisor);

        public static Rect operator +(Rect rect, Thickness thickness)
        {
            rect.x = Mathf.Max(rect.x - thickness.Left, 0f);
            rect.y = Mathf.Max(rect.y - thickness.Top, 0f);
            rect.width += thickness.Left + thickness.Right;
            rect.height += thickness.Top + thickness.Bottom;
            return rect;
        }
        public static Rect operator -(Rect rect, Thickness thickness)
        {
            rect.x += thickness.Left;
            rect.y += thickness.Top;
            rect.width = Mathf.Max(rect.width - thickness.Left - thickness.Right, 0f);
            rect.height = Mathf.Max(rect.height - thickness.Top - thickness.Bottom, 0f);
            return rect;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Static Methods
        //
        //------------------------------------------------------

        #region Static Methods

        internal static Thickness Add(Thickness left, Thickness right) => new Thickness(
            left.Left + right.Left,
            left.Top + right.Top,
            left.Right + right.Right,
            left.Bottom + right.Bottom);

        internal static Thickness Subtract(Thickness left, Thickness right) => new Thickness(
            left.Left - right.Left,
            left.Top - right.Top,
            left.Right - right.Right,
            left.Bottom - right.Bottom);

        internal static Thickness Multiply(Thickness thickness, float multiplier) => new Thickness(
            thickness.Left * multiplier,
            thickness.Top * multiplier,
            thickness.Right * multiplier,
            thickness.Bottom * multiplier);

        #endregion
    }
}
