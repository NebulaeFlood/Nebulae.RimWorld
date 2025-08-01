using System;
using System.Diagnostics;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 描述边框的粗细
    /// </summary>
    [DebuggerStepThrough]
    public readonly struct Thickness : IEquatable<Thickness>
    {
        /// <summary>
        /// <see cref="Left"/>、<see cref="Top"/>、<see cref="Right"/> 和 <see cref="Bottom"/> 为 0 的 <see cref="Thickness"/>
        /// </summary>
        public static readonly Thickness Empty = new Thickness(0f);

        /// <summary>
        /// 表示大于零的最小正 <see cref="Thickness"/>
        /// </summary>
        public static readonly Thickness Epsilon = new Thickness(float.Epsilon);


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
        /// 如果此 <see cref="Thickness"/> 的 <see cref="Left"/>, <see cref="Top"/>, <see cref="Right"/> 和 <see cref="Bottom"/> 与 <paramref name="other"/> 一致，
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
        /// <returns>如果指定的对象等于此 <see cref="Thickness"/> ，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            return obj is Thickness other
                && this == other;
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
        /// 规范化此 <see cref="Thickness"/>
        /// </summary>
        /// <returns>规范后的该 <see cref="Thickness"/>。</returns>
        public Thickness Normalize()
        {
            return new Thickness(
                Left > 0 ? Mathf.Round(Left) : 0f,
                Top > 0 ? Mathf.Round(Top) : 0f,
                Right > 0 ? Mathf.Round(Right) : 0f,
                Bottom > 0 ? Mathf.Round(Bottom) : 0f);
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

        /// <summary>
        /// 将一个作为统一长度的 <see cref="float"/> 隐式转换为 <see cref="Thickness"/> 对象
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <returns>返回一个新的 <see cref="Thickness"/> 对象，其四个边的值均为 <paramref name="uniformLength"/>。</returns>
        public static implicit operator Thickness(float uniformLength) => new Thickness(uniformLength);

        /// <summary>
        /// 将元组隐式转换为 <see cref="Thickness"/>
        /// </summary>
        /// <param name="thickness">元组对象</param>
        /// <returns>返回一个新的 <see cref="Thickness"/> 对象，四个边的值按照构造函数的参数顺序对应元组的四个值。</returns>
        public static implicit operator Thickness((float Left, float Top, float Right, float Bottom) thickness) => new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);

        /// <summary>
        /// 判断 <see cref="Thickness"/> 对象的四个边是否等于指定的统一长度
        /// </summary>
        /// <param name="thickness"><see cref="Thickness"/> 对象</param>
        /// <param name="uniformLength">统一长度</param>
        /// <returns>如果 <paramref name="thickness"/> 的四个边均等于 <paramref name="uniformLength"/>，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(Thickness thickness, float uniformLength) =>
            thickness.Left == uniformLength && thickness.Top == uniformLength && thickness.Right == uniformLength && thickness.Bottom == uniformLength;

        /// <summary>
        /// 判断 <see cref="Thickness"/> 对象的四个边是否不等于指定的统一长度
        /// </summary>
        /// <param name="thickness"><see cref="Thickness"/> 对象</param>
        /// <param name="uniformLength">统一长度</param>
        /// <returns>如果 <paramref name="thickness"/> 的任意边不等于 <paramref name="uniformLength"/>，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(Thickness thickness, float uniformLength) =>
            thickness.Left != uniformLength || thickness.Top != uniformLength || thickness.Right != uniformLength || thickness.Bottom != uniformLength;

        /// <summary>
        /// 判断指定的统一长度是否等于 <see cref="Thickness"/> 对象的四个边。
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <param name="thickness"><see cref="Thickness"/> 对象</param>
        /// <returns>如果 <paramref name="uniformLength"/> 等于 <paramref name="thickness"/> 的四个边，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(float uniformLength, Thickness thickness) =>
            thickness.Left == uniformLength && thickness.Top == uniformLength && thickness.Right == uniformLength && thickness.Bottom == uniformLength;

        /// <summary>
        /// 判断指定的统一长度是否不等于 <see cref="Thickness"/> 对象的四个边。
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <param name="thickness"><see cref="Thickness"/> 对象</param>
        /// <returns>如果 <paramref name="uniformLength"/> 与 <paramref name="thickness"/> 的任意边不相等，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(float uniformLength, Thickness thickness) =>
            thickness.Left != uniformLength || thickness.Top != uniformLength || thickness.Right != uniformLength || thickness.Bottom != uniformLength;

        /// <summary>
        /// 比较两个 <see cref="Thickness"/> 对象是否相等（四个边的值均相等）。
        /// </summary>
        /// <param name="left">第一个 <see cref="Thickness"/> 对象</param>
        /// <param name="right">第二个 <see cref="Thickness"/> 对象</param>
        /// <returns>如果两个 <see cref="Thickness"/> 对象的四个边值均相等，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(Thickness left, Thickness right) =>
            left.Left == right.Left && left.Top == right.Top && left.Right == right.Right && left.Bottom == right.Bottom;

        /// <summary>
        /// 比较两个 <see cref="Thickness"/> 对象是否不相等（任意一个边的值不相等）。
        /// </summary>
        /// <param name="left">第一个 <see cref="Thickness"/> 对象</param>
        /// <param name="right">第二个 <see cref="Thickness"/> 对象</param>
        /// <returns>如果两个 <see cref="Thickness"/> 对象的任意边值不相等，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(Thickness left, Thickness right) =>
            left.Left != right.Left || left.Top != right.Top || left.Right != right.Right || left.Bottom != right.Bottom;

        /// <summary>
        /// 判断 <see cref="Thickness"/> 对象的四个边是否均大于指定的统一长度。
        /// </summary>
        /// <param name="thickness"> <see cref="Thickness"/> 对象</param>
        /// <param name="uniformLength">统一长度</param>
        /// <returns>如果 <see cref="Thickness"/> 对象的四个边均大于 <paramref name="uniformLength"/>，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator >(Thickness thickness, float uniformLength) =>
            thickness.Left > uniformLength && thickness.Top > uniformLength && thickness.Right > uniformLength && thickness.Bottom > uniformLength;

        /// <summary>
        /// 判断 <see cref="Thickness"/> 对象的四个边是否均小于指定的统一长度。
        /// </summary>
        /// <param name="thickness"> <see cref="Thickness"/> 对象</param>
        /// <param name="uniformLength">统一长度</param>
        /// <returns>如果 <see cref="Thickness"/> 对象有一边边小于 <paramref name="uniformLength"/>，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator <(Thickness thickness, float uniformLength) =>
            thickness.Left < uniformLength || thickness.Top < uniformLength || thickness.Right < uniformLength || thickness.Bottom < uniformLength;

        /// <summary>
        /// 判断指定的统一长度是否大于 <see cref="Thickness"/> 对象的四个边。
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <param name="thickness"> <see cref="Thickness"/> 对象</param>
        /// <returns>如果 <paramref name="uniformLength"/> 大于 <paramref name="thickness"/> 的四个边，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator >(float uniformLength, Thickness thickness) =>
            thickness.Left > uniformLength && thickness.Top > uniformLength && thickness.Right > uniformLength && thickness.Bottom > uniformLength;

        /// <summary>
        /// 判断指定的统一长度是否小于 <see cref="Thickness"/> 对象的四个边。
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <param name="thickness"> <see cref="Thickness"/> 对象</param>
        /// <returns>如果 <paramref name="uniformLength"/> 小于 <paramref name="thickness"/> 的四个边，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator <(float uniformLength, Thickness thickness) =>
            thickness.Left < uniformLength && thickness.Top < uniformLength && thickness.Right < uniformLength && thickness.Bottom < uniformLength;

        /// <summary>
        /// 比较两个 <see cref="Thickness"/> 对象是否大于（四个边的值均大于另一个对象）。
        /// </summary>
        /// <param name="left">第一个 <see cref="Thickness"/> 对象</param>
        /// <param name="right">第二个 <see cref="Thickness"/> 对象</param>
        /// <returns>如果 <paramref name="left"/> 的四个边均大于 <paramref name="right"/> 的边，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator >(Thickness left, Thickness right) =>
            left.Left > right.Left && left.Top > right.Top && left.Right > right.Right && left.Bottom > right.Bottom;

        /// <summary>
        /// 比较两个 <see cref="Thickness"/> 对象是否小于（四个边的值均小于另一个对象）。
        /// </summary>
        /// <param name="left">第一个 <see cref="Thickness"/> 对象</param>
        /// <param name="right">第二个 <see cref="Thickness"/> 对象</param>
        /// <returns>如果 <paramref name="left"/> 的四个边均小于 <paramref name="right"/> 的边，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator <(Thickness left, Thickness right) =>
            left.Left < right.Left && left.Top < right.Top && left.Right < right.Right && left.Bottom < right.Bottom;

        /// <summary>
        /// 为两个 <see cref="Thickness"/> 对象相加，返回新的 <see cref="Thickness"/> 对象
        /// </summary>
        /// <param name="left">第一个 <see cref="Thickness"/> 对象</param>
        /// <param name="right">第二个 <see cref="Thickness"/> 对象</param>
        /// <returns>返回两个 <see cref="Thickness"/> 对象相加后的新 <see cref="Thickness"/> 对象。</returns>
        public static Thickness operator +(Thickness left, Thickness right) => Add(left, right);

        /// <summary>
        /// 为两个 <see cref="Thickness"/> 对象相减，返回新的 <see cref="Thickness"/> 对象
        /// </summary>
        /// <param name="left">第一个 <see cref="Thickness"/> 对象</param>
        /// <param name="right">第二个 <see cref="Thickness"/> 对象</param>
        /// <returns>返回两个 <see cref="Thickness"/> 对象相减后的新 <see cref="Thickness"/> 对象。</returns>
        public static Thickness operator -(Thickness left, Thickness right) => Subtract(left, right);

        /// <summary>
        /// 将 <see cref="Thickness"/> 对象与指定倍数相乘，返回新的 <see cref="Thickness"/> 对象
        /// </summary>
        /// <param name="thickness"> <see cref="Thickness"/> 对象</param>
        /// <param name="multiplier">乘数</param>
        /// <returns>返回 <paramref name="thickness"/> 乘以 <paramref name="multiplier"/> 后的新 <see cref="Thickness"/> 对象。</returns>
        public static Thickness operator *(Thickness thickness, float multiplier) => Multiply(thickness, multiplier);

        /// <summary>
        /// 将 <see cref="Thickness"/> 对象与指定除数相除，返回新的 <see cref="Thickness"/> 对象
        /// </summary>
        /// <param name="thickness"> <see cref="Thickness"/> 对象</param>
        /// <param name="divisor">除数。</param>
        /// <returns>返回 <paramref name="thickness"/> 除以 <paramref name="divisor"/> 后的新 <see cref="Thickness"/> 对象。</returns>
        public static Thickness operator /(Thickness thickness, float divisor) => Multiply(thickness, 1f / divisor);

        /// <summary>
        /// 将 <see cref="Rect"/> 对象与 <see cref="Thickness"/> 相加，返回新的矩形对象。
        /// </summary>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <param name="thickness"> <see cref="Thickness"/> 对象</param>
        /// <returns>返回添加 <paramref name="thickness"/> 后的 <see cref="Rect"/> 对象。</returns>
        public static Rect operator +(Rect rect, Thickness thickness)
        {
            float left = rect.x - thickness.Left;
            float top = rect.y - thickness.Top;
            float right = rect.xMax + thickness.Right;
            float bottom = rect.yMax + thickness.Bottom;
            return new Rect(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// 将 <see cref="Rect"/> 对象与 <see cref="Thickness"/> 相减，返回新的矩形对象。
        /// </summary>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <param name="thickness"><see cref="Thickness"/> 对象</param>
        /// <returns>返回减去 <paramref name="thickness"/> 后的 新<see cref="Rect"/> 对象。</returns>
        public static Rect operator -(Rect rect, Thickness thickness)
        {
            float left = rect.x + thickness.Left;
            float top = rect.y + thickness.Top;
            float right = Mathf.Max(left, rect.xMax - thickness.Right);
            float bottom = Mathf.Max(top, rect.yMax - thickness.Bottom);
            return new Rect(left, top, right - left, bottom - top);
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
