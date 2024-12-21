using System;
using UnityEngine;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 用于指定宽度和高度
    /// </summary>
    public struct Size : IEquatable<Size>
    {
        /// <summary>
        /// <see cref="Width"/> 和 <see cref="Height"/> 为 0 的 <see cref="Size"/>
        /// </summary>
        public static readonly Size Empty = new Size(0f);

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取此 <see cref="Size"/> 结构的垂直长度
        /// </summary>
        public readonly float Height;

        /// <summary>
        /// 获取此 <see cref="Size"/> 结构的水平长度
        /// </summary>
        public readonly float Width;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="Size"/> 结构的新实例
        /// </summary>
        /// <param name="width">新的 <see cref="Size"/> 结构的宽度</param>
        /// <param name="height">新的 <see cref="Size"/> 结构的高度</param>
        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 初始化 <see cref="Size"/> 结构的新实例
        /// </summary>
        /// <param name="size">新的 <see cref="Size"/> 结构的宽度和高度</param>
        public Size(float size)
        {
            Width = size;
            Height = size;
        }

        /// <summary>
        /// 初始化 <see cref="Size"/> 结构的新实例
        /// </summary>
        public Size(Rect rect)
        {
            Width = rect.width;
            Height = rect.height;
        }

        /// <summary>
        /// 初始化 <see cref="Size"/> 结构的新实例
        /// </summary>
        public Size(Vector2 vector)
        {
            Width = vector.x;
            Height = vector.y;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 限定此 <see cref="Size"> 的 <see cref="Width"/> 和 <see cref="Height"/> 到指定范围
        /// </summary>
        /// <param name="minSize">最小值</param>
        /// <param name="maxSize">最大值</param>
        /// <returns>限定到范围内后的 <see cref="Size"/>。</returns>
        public Size Clamp(Size minSize, Size maxSize)
        {
            return new Size(
                Mathf.Clamp(Width, minSize.Width, minSize.Width),
                Mathf.Clamp(Height, minSize.Height, maxSize.Height));
        }

        /// <summary>
        /// 按照“四舍六进五成双”的规则将此 <see cref="Size"> 的 <see cref="Width"/> 和 <see cref="Height"/> 精确到整数
        /// </summary>
        /// <returns>精确到整数后的此 <see cref="Size"> 。</returns>
        public Size Round()
        {
            return new Size(
                Mathf.Round(Width),
                Mathf.Round(Height));
        }

        /// <summary>
        /// 判断指定对象是否等于此 <see cref="Size"> 
        /// </summary>
        /// <param name="obj">要与此 <see cref="Size"> 进行比较的对象</param>
        /// <returns>如果指定的对象等于此 <see cref="Size"> ，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            return obj is Size other && this == other;
        }

        /// <summary>
        /// 判断 <see cref="Width"/> 和 <see cref="Height"/> 是否与另一个 <see cref="Size"/> 的实例一致
        /// </summary>
        /// <param name="other">用于比较的对象</param>
        /// <returns>
        /// 如果此 <see cref="Size"> 的 <see cref="Width"/> 和 <see cref="Height"/> 与 <paramref name="other"/> 一致，
        /// 则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。
        /// </returns>
        public bool Equals(Size other) => this == other;

        /// <summary>
        /// 哈希函数
        /// </summary>
        /// <returns>此 <see cref="Size"> 的哈希代码</returns>
        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        /// <summary>
        /// 按照指定的倍率缩放 <see cref="Size"/> 结构的宽高
        /// </summary>
        /// <param name="scale">缩放倍率</param>
        /// <returns>缩放后的 <see cref="Size"/> 结构。</returns>
        public Size Scale(float scale)
        {
            return new Size(
                Width * scale,
                Height * scale);
        }

        /// <summary>
        /// 按照指定的倍率缩放 <see cref="Size"/> 结构的宽高
        /// </summary>
        /// <param name="widthScale">宽度缩放倍率</param>
        /// <param name="heightScale">高度缩放倍率</param>
        /// <returns>缩放后的 <see cref="Size"/> 结构。</returns>
        public Size Scale(float widthScale, float heightScale)
        {
            return new Size(
                Width * widthScale,
                Height * heightScale);
        }

        /// <summary>
        /// 返回表示此 <see cref="Size"> 的字符串
        /// </summary>
        /// <returns>表示此 <see cref="Size"> 的字符串。</returns>
        public override string ToString()
        {
            return $"{{Width:{Width}, Height:{Height}}}";
        }

        #endregion


        //------------------------------------------------------
        //
        //  Operators
        //
        //------------------------------------------------------

        #region Operators

        public static implicit operator Size(Rect rect) => new Size(rect.width, rect.height);
        public static implicit operator Size(Vector2 vector) => new Size(vector.x, vector.y);

        public static implicit operator Vector2(Size size) => new Vector2(size.Width, size.Height);


        public static bool operator ==(Size size, float uniformLength) => size.Width == uniformLength && size.Height == uniformLength;
        public static bool operator !=(Size size, float uniformLength) => size.Width != uniformLength || size.Height != uniformLength;

        public static bool operator ==(float uniformLength, Size size) => size.Width == uniformLength && size.Height == uniformLength;
        public static bool operator !=(float uniformLength, Size size) => size.Width != uniformLength || size.Height != uniformLength;

        public static bool operator ==(Size left, Size right) => left.Width == right.Width && left.Height == right.Height;
        public static bool operator !=(Size left, Size right) => left.Width != right.Width || left.Height != right.Height;

        public static bool operator >(Size size, float uniformLength) => size.Width > uniformLength && size.Height > uniformLength;
        public static bool operator <(Size size, float uniformLength) => size.Width < uniformLength && size.Height < uniformLength;

        public static bool operator >(float uniformLength, Size size) => uniformLength > size.Width && uniformLength > size.Height;
        public static bool operator <(float uniformLength, Size size) => uniformLength < size.Width && uniformLength < size.Height;

        public static bool operator >(Size left, Size right) => left.Width > right.Width && left.Height > right.Height;
        public static bool operator <(Size left, Size right) => left.Width < right.Width && left.Height < right.Height;


        public static Size operator +(Size left, Size right) => Add(left, right);
        public static Size operator -(Size left, Size right) => Subtract(left, right);
        public static Size operator *(Size size, float multiplier) => Multiply(size, multiplier);
        public static Size operator /(Size size, float divisor) => Multiply(size, 1f / divisor);

        public static Size operator +(Size size, Thickness thickness)
        {
            return new Size(
                thickness.Left + size.Width + thickness.Right,
                thickness.Top + size.Height + thickness.Bottom);
        }
        public static Size operator -(Size size, Thickness thickness)
        {
            return new Size(
                size.Width - thickness.Left - thickness.Right,
                size.Height - thickness.Top - thickness.Bottom
                );
        }

        #endregion


        //------------------------------------------------------
        //
        //  Static Methods
        //
        //------------------------------------------------------

        #region Static Methods

        internal static Size Add(Size left, Size right) => new Size(left.Width + right.Width, left.Height + right.Height);

        internal static Size Subtract(Size left, Size right) => new Size(left.Width - right.Width, left.Height - right.Height);

        internal static Size Multiply(Size size, float multiplier) => new Size(size.Width * multiplier, size.Height * multiplier);

        #endregion
    }
}
