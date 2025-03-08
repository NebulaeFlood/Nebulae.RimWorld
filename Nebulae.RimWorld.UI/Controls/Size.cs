using System;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 用于指定宽度和高度
    /// </summary>
    public readonly struct Size : IEquatable<Size>
    {
        /// <summary>
        /// <see cref="Width"/> 和 <see cref="Height"/> 为 0 的 <see cref="Size"/>
        /// </summary>
        public static readonly Size Empty = new Size(0f);

        /// <summary>
        /// 表示大于零的最小正 <see cref="Size"/>
        /// </summary>
        public static readonly Size Epsilon = new Size(1f);


        //------------------------------------------------------
        //
        //  Public Feilds
        //
        //------------------------------------------------------

        #region Public Feilds

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
        /// 限定此 <see cref="Size"/> 的 <see cref="Width"/> 和 <see cref="Height"/> 到指定范围
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
        /// 按照“四舍六进五成双”的规则将此 <see cref="Size"/> 的 <see cref="Width"/> 和 <see cref="Height"/> 精确到整数
        /// </summary>
        /// <returns>精确到整数后的此 <see cref="Size"/> 。</returns>
        public Size Round()
        {
            return new Size(
                Mathf.Round(Width),
                Mathf.Round(Height));
        }

        /// <summary>
        /// 判断指定对象是否等于此 <see cref="Size"/> 
        /// </summary>
        /// <param name="obj">要与此 <see cref="Size"/> 进行比较的对象</param>
        /// <returns>如果指定的对象等于此 <see cref="Size"/> ，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            return obj is Size other
                && this == other;
        }

        /// <summary>
        /// 判断 <see cref="Width"/> 和 <see cref="Height"/> 是否与另一个 <see cref="Size"/> 的实例一致
        /// </summary>
        /// <param name="other">用于比较的对象</param>
        /// <returns>
        /// 如果此 <see cref="Size"/> 的 <see cref="Width"/> 和 <see cref="Height"/> 与 <paramref name="other"/> 一致，
        /// 则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。
        /// </returns>
        public bool Equals(Size other) => this == other;

        /// <summary>
        /// 哈希函数
        /// </summary>
        /// <returns>此 <see cref="Size"/> 的哈希代码</returns>
        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        /// <summary>
        /// 规范化此 <see cref="Size"/>
        /// </summary>
        /// <returns>规范后的该 <see cref="Size"/>。</returns>
        public Size Normalize()
        {
            return new Size(
                Width > 0f ? Width : 0f,
                Height > 0f ? Height : 0f);
        }

        /// <summary>
        /// 解析此 <see cref="Size"/> 最终的实际尺寸
        /// </summary>
        /// <param name="availableSize">可用的尺寸</param>
        /// <returns>规范后的该 <see cref="Size"/>。</returns>
        public Size Resolve(Size availableSize)
        {
            float width, height;

            if (Width < 0f)
            {
                width = 0f;
            }
            else if (Width > 1f)
            {
                width = Width;
            }
            else
            {
                width = Width * availableSize.Width;
            }

            if (Height < 0f)
            {
                height = 0f;
            }
            else if (Height > 1f)
            {
                height = Height;
            }
            else
            {
                height = Height * availableSize.Height;
            }

            return new Size(width, height);
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
        /// 返回表示此 <see cref="Size"/> 的字符串
        /// </summary>
        /// <returns>表示此 <see cref="Size"/> 的字符串。</returns>
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

        /// <summary>
        /// 将 <see cref="Rect"/> 隐式转换为 <see cref="Size"/>
        /// </summary>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <returns>返回一个新的 <see cref="Size"/> 对象，宽度和高度分别为 <paramref name="rect"/> 的宽度和高度。</returns>
        public static implicit operator Size(Rect rect) => new Size(rect.width, rect.height);

        /// <summary>
        /// 将 <see cref="Vector2"/> 隐式转换为 <see cref="Size"/>
        /// </summary>
        /// <param name="vector"><see cref="Vector2"/> 对象</param>
        /// <returns>返回一个新的 <see cref="Size"/> 对象，宽度和高度分别为 <see cref="Vector2"/> 的 x 和 y。</returns>
        public static implicit operator Size(Vector2 vector) => new Size(vector.x, vector.y);

        /// <summary>
        /// 将 <see cref="Size"/> 隐式转换为 <see cref="Vector2"/>
        /// </summary>
        /// <param name="size"><see cref="Rect"/> 对象</param>
        /// <returns>返回一个新的 <see cref="Vector2"/> 对象，x 和 y 分别为 <paramref name="size"/> 的宽度和高度。</returns>
        public static implicit operator Vector2(Size size) => new Vector2(size.Width, size.Height);

        /// <summary>
        /// 判断 <see cref="Size"/> 对象的宽度和高度是否等于指定的统一长度
        /// </summary>
        /// <param name="size">尺寸对象。</param>
        /// <param name="uniformLength">统一长度值。</param>
        /// <returns>如果尺寸的宽度和高度均等于统一长度，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(Size size, float uniformLength) => size.Width == uniformLength && size.Height == uniformLength;

        /// <summary>
        /// 判断 <see cref="Size"/> 对象的宽度和高度是否不等于指定的统一长度
        /// </summary>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <param name="uniformLength">统一长度</param>
        /// <returns>如果 <paramref name="size"/> 的宽度或高度不等于 <paramref name="uniformLength"/>，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(Size size, float uniformLength) => size.Width != uniformLength || size.Height != uniformLength;

        /// <summary>
        /// 判断指定的统一长度是否等于 <see cref="Size"/> 对象的宽度和高度
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <returns>如果 <paramref name="uniformLength"/> 等于 <paramref name="size"/> 的宽度和高度，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(float uniformLength, Size size) => size.Width == uniformLength && size.Height == uniformLength;

        /// <summary>
        /// 判断指定的统一长度是否不等于 <see cref="Size"/> 对象的宽度和高度
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <returns>如果 <paramref name="uniformLength"/> 不等于 <paramref name="size"/> 的宽度或高度，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(float uniformLength, Size size) => size.Width != uniformLength || size.Height != uniformLength;

        /// <summary>
        /// 比较两个 <see cref="Size"/> 对象是否相等（宽度和高度均相等）
        /// </summary>
        /// <param name="left">第一个 <see cref="Size"/> 对象</param>
        /// <param name="right">第二个 <see cref="Size"/> 对象</param>
        /// <returns>如果两个 <see cref="Size"/> 对象的宽度和高度均相等，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(Size left, Size right) => left.Width == right.Width && left.Height == right.Height;

        /// <summary>
        /// 比较两个 <see cref="Size"/> 对象是否不相等（宽度或高度不相等）
        /// </summary>
        /// <param name="left">第一个<see cref="Size"/> 对象</param>
        /// <param name="right">第二个<see cref="Size"/> 对象</param>
        /// <returns>如果两个 <see cref="Size"/> 对象的宽度或高度不相等，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(Size left, Size right) => left.Width != right.Width || left.Height != right.Height;

        /// <summary>
        /// 判断 <see cref="Size"/> 对象是否大于指定的统一长度（宽度和高度均大于统一长度）
        /// </summary>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <param name="uniformLength">统一长度</param>
        /// <returns>如果 <paramref name="size"/> 的宽度和高度均大于 <paramref name="uniformLength"/>，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator >(Size size, float uniformLength) => size.Width > uniformLength && size.Height > uniformLength;

        /// <summary>
        /// 判断 <see cref="Size"/> 对象是否小于指定的统一长度（宽度和高度均小于统一长度）
        /// </summary>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <param name="uniformLength">统一长度</param>
        /// <returns>如果 <paramref name="size"/> 的宽度和高度有一项小于 <paramref name="uniformLength"/>，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator <(Size size, float uniformLength) => size.Width < uniformLength || size.Height < uniformLength;

        /// <summary>
        /// 判断指定的统一长度是否大于 <see cref="Size"/> 对象的宽度和高度
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <returns>如果 <paramref name="uniformLength"/> 大于 <paramref name="size"/> 的宽度和高度，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator >(float uniformLength, Size size) => uniformLength > size.Width && uniformLength > size.Height;

        /// <summary>
        /// 判断指定的统一长度是否小于 <see cref="Size"/> 对象的宽度和高度
        /// </summary>
        /// <param name="uniformLength">统一长度</param>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <returns>如果 <paramref name="uniformLength"/> 小于 <paramref name="size"/> 的宽度和高度，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator <(float uniformLength, Size size) => uniformLength < size.Width && uniformLength < size.Height;

        /// <summary>
        /// 比较两个 <see cref="Size"/> 对象是否大于（宽度和高度均大于另一个尺寸）。
        /// </summary>
        /// <param name="left">第一个尺寸对象。</param>
        /// <param name="right">第二个尺寸对象。</param>
        /// <returns>如果第一个尺寸的宽度和高度均大于第二个尺寸，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator >(Size left, Size right) => left.Width > right.Width && left.Height > right.Height;

        /// <summary>
        /// 比较两个 <see cref="Size"/> 对象是否小于（宽度和高度均小于另一个尺寸）。
        /// </summary>
        /// <param name="left">第一个 <see cref="Size"/> 对象</param>
        /// <param name="right">第二个 <see cref="Size"/> 对象</param>
        /// <returns>如果 <paramref name="left"/> 的宽度或高度小于 <paramref name="right"/>，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator <(Size left, Size right) => left.Width < right.Width || left.Height < right.Height;

        /// <summary>
        /// 将两个 <see cref="Size"/> 对象相加，返回新 <see cref="Size"/> 对象
        /// </summary>
        /// <param name="left">第一个 <see cref="Size"/> 对象</param>
        /// <param name="right">第二个 <see cref="Size"/> 对象</param>
        /// <returns>返回两个 <see cref="Size"/> 对象相加后的新 <see cref="Size"/> 对象。</returns>
        public static Size operator +(Size left, Size right) => Add(left, right);

        /// <summary>
        /// 将两个 <see cref="Size"/> 对象相减，返回新 <see cref="Size"/> 对象
        /// </summary>
        /// <param name="left">第一个 <see cref="Size"/> 对象</param>
        /// <param name="right">第二个 <see cref="Size"/> 对象</param>
        /// <returns>返回两个 <see cref="Size"/> 对象相减后的新 <see cref="Size"/> 对象。</returns>
        public static Size operator -(Size left, Size right) => Subtract(left, right);

        /// <summary>
        /// 将 <see cref="Size"/> 对象与指定倍数相乘，返回新 <see cref="Size"/> 对象
        /// </summary>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <param name="multiplier">乘数</param>
        /// <returns>返回 <paramref name="size"/> 乘以 <paramref name="multiplier"/> 后的新 <see cref="Size"/> 对象。</returns>
        public static Size operator *(Size size, float multiplier) => Multiply(size, multiplier);

        /// <summary>
        /// 将 <see cref="Size"/> 对象与指定除数相除，返回新 <see cref="Size"/> 对象
        /// </summary>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <param name="divisor">除数</param>
        /// <returns>返回 <paramref name="size"/> 除以 <paramref name="divisor"/> 后的新 <see cref="Size"/> 对象。</returns>
        public static Size operator /(Size size, float divisor) => new Size(size.Width / divisor, size.Height / divisor);

        /// <summary>
        /// 将 <see cref="Size"/> 对象与 <see cref="Thickness"/> 对象相加，返回新 <see cref="Size"/> 对象
        /// </summary>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <param name="thickness"><see cref="Thickness"/> 对象</param>
        /// <returns>返回 <paramref name="size"/> 与 <paramref name="thickness"/> 相加后的新 <see cref="Size"/> 对象。</returns>
        public static Size operator +(Size size, Thickness thickness)
        {
            return new Size(
                thickness.Left + size.Width + thickness.Right,
                thickness.Top + size.Height + thickness.Bottom);
        }

        /// <summary>
        /// 将 <see cref="Size"/> 对象与 <see cref="Thickness"/> 对象相减，返回新 <see cref="Size"/> 对象
        /// </summary>
        /// <param name="size"><see cref="Size"/> 对象</param>
        /// <param name="thickness"><see cref="Thickness"/> 对象</param>
        /// <returns>返回 <paramref name="size"/> 与 <paramref name="thickness"/> 相减后的新 <see cref="Size"/> 对象。</returns>
        public static Size operator -(Size size, Thickness thickness)
        {
            return new Size(
                size.Width - thickness.Left - thickness.Right,
                size.Height - thickness.Top - thickness.Bottom);
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
