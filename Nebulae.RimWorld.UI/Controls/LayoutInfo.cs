using System;
using System.Diagnostics;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 保存布局区域信息的结构
    /// </summary>
    [DebuggerStepThrough]
    public readonly struct LayoutInfo : IEquatable<LayoutInfo>
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// X 坐标
        /// </summary>
        public readonly float X;

        /// <summary>
        /// Y 坐标
        /// </summary>
        public readonly float Y;

        /// <summary>
        /// 宽度
        /// </summary>
        public readonly float Width;

        /// <summary>
        /// 高度
        /// </summary>
        public readonly float Height;

        #endregion


        /// <summary>
        /// 初始化 <see cref="LayoutInfo"/> 的新实例
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public LayoutInfo(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断指定坐标是否在布局区域内
        /// </summary>
        /// <param name="point">要判断的坐标</param>
        /// <returns>若该布局区域包括指定坐标，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(Vector2 point)
        {
            return point.x >= X && point.x < X + Width && point.y >= Y && point.y < Y + Height;
        }

        /// <summary>
        /// 判断指定坐标是否在布局区域内
        /// </summary>
        /// <param name="point">要判断的坐标</param>
        /// <returns>若该布局区域包括指定坐标，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(Vector3 point)
        {
            return point.x >= X && point.x < X + Width && point.y >= Y && point.y < Y + Height;
        }

        /// <summary>
        /// 判断指定坐标是否在布局区域内
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        /// <returns>若该布局区域包括指定坐标，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(float x, float y)
        {
            return x >= X && x < X + Width && y >= Y && y < Y + Height;
        }

        /// <summary>
        /// 计算与另一个布局区域重叠的区域
        /// </summary>
        /// <param name="layoutInfo">另一个布局区域</param>
        /// <returns>重叠的布局区域。</returns>
        public LayoutInfo IntersectWith(LayoutInfo layoutInfo)
        {
            float left = Mathf.Max(X, layoutInfo.X);
            float top = Mathf.Max(Y, layoutInfo.Y);
            float width = Mathf.Min(X + Width, layoutInfo.X + layoutInfo.Width) - left;
            float height = Mathf.Min(Y + Height, layoutInfo.Y + layoutInfo.Height) - top;

            return new LayoutInfo(left, top, width > 0 ? width : 0, height > 0 ? height : 0);
        }

        /// <summary>
        /// 判断指定对象是否等于此 <see cref="LayoutInfo"/> 
        /// </summary>
        /// <param name="obj">要与此 <see cref="LayoutInfo"/> 进行比较的对象</param>
        /// <returns>若指定的对象等于此 <see cref="LayoutInfo"/> ，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj) => obj is LayoutInfo info && this == info;

        /// <summary>
        /// 判断此 <see cref="LayoutInfo"/> 是否与另一个 <see cref="LayoutInfo"/> 表示相同区域
        /// </summary>
        /// <param name="other">用于比较的对象</param>
        /// <returns>
        /// 若此 <see cref="LayoutInfo"/> 和另一个 <see cref="LayoutInfo"/> 表示相同区域，
        /// 返回 <see langword="true"/>；反之则返回 <see langword="false"/>。
        /// </returns>
        public bool Equals(LayoutInfo other) => this == other;

        /// <summary>
        /// 哈希函数
        /// </summary>
        /// <returns>此 <see cref="Size"/> 的哈希代码</returns>
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();

        /// <summary>
        /// 判断是否需要重新排布
        /// </summary>
        /// <param name="info">新的布局信息</param>
        /// <returns>若返回 <see langword="true"/>，需要无效化控件排布。</returns>
        public bool IsArrangeDirty(LayoutInfo info) => X != info.X || Y != info.Y;

        /// <summary>
        /// 判断是否需要重新度量
        /// </summary>
        /// <param name="info">新的布局信息</param>
        /// <returns>若返回 <see langword="true"/>，需要无效化控件度量。</returns>
        public bool IsMeasureDirty(LayoutInfo info) => Width != info.Width || Height != info.Height;

        /// <summary>
        /// 将 <see cref="LayoutInfo"/> 转换为 <see cref="Rect"/>
        /// </summary>
        /// <returns>转换后的 <see cref="Rect"/>。</returns>
        public Rect ToRect() => new Rect(X, Y, Width, Height);

        /// <summary>
        /// 将 <see cref="LayoutInfo"/> 转换为 <see cref="Size"/>
        /// </summary>
        /// <returns>转换后的 <see cref="Rect"/>。</returns>
        public Size ToSize() => new Size(Width, Height);

        #endregion


        //------------------------------------------------------
        //
        //  Operators
        //
        //------------------------------------------------------

        #region Operators

        /// <summary>
        /// 将 <see cref="Rect"/> 隐式转换为 <see cref="LayoutInfo"/>
        /// </summary>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <returns>返回一个新的 <see cref="LayoutInfo"/> 对象。</returns>
        public static implicit operator LayoutInfo(Rect rect) => new LayoutInfo(rect.x, rect.y, rect.width, rect.height);

        /// <summary>
        /// 将 <see cref="LayoutInfo"/> 隐式转换为 <see cref="Rect"/>
        /// </summary>
        /// <param name="info"><see cref="LayoutInfo"/> 对象</param>
        /// <returns>返回一个新的 <see cref="Rect"/> 对象。</returns>
        public static implicit operator Rect(LayoutInfo info) => new Rect(info.X, info.Y, info.Width, info.Height);

        /// <summary>
        /// 将 <see cref="LayoutInfo"/> 隐式转换为 <see cref="Size"/>
        /// </summary>
        /// <param name="info"><see cref="LayoutInfo"/> 对象</param>
        /// <returns>返回一个新的 <see cref="Size"/> 对象。</returns>
        public static implicit operator Size(LayoutInfo info) => new Size(info.Width, info.Height);


        /// <summary>
        /// 判断 <see cref="LayoutInfo"/> 是否表示相同区域
        /// </summary>
        /// <param name="left">第一个 <see cref="LayoutInfo"/></param>
        /// <param name="right">第二个 <see cref="LayoutInfo"/></param>
        /// <returns><paramref name="left"/> 和 <paramref name="right"/> 表示相同区域，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(LayoutInfo left, LayoutInfo right) => left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;

        /// <summary>
        /// 判断 <see cref="LayoutInfo"/> 是否表示不同区域
        /// </summary>
        /// <param name="left">第一个 <see cref="LayoutInfo"/></param>
        /// <param name="right">第二个 <see cref="LayoutInfo"/></param>
        /// <returns><paramref name="left"/> 和 <paramref name="right"/> 表示不同区域，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(LayoutInfo left, LayoutInfo right) => left.X != right.X || left.Y != right.Y || left.Width != right.Width || left.Height != right.Height;

        /// <summary>
        /// 判断 <see cref="LayoutInfo"/> 和 <see cref="Rect"/> 是否表示相同区域
        /// </summary>
        /// <param name="info"><see cref="LayoutInfo"/> 对象</param>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <returns><paramref name="info"/> 和 <paramref name="rect"/> 表示相同区域，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(LayoutInfo info, Rect rect) => info.X == rect.x && info.Y == rect.y && info.Width == rect.width && info.Height == rect.height;

        /// <summary>
        /// 判断 <see cref="LayoutInfo"/> 和 <see cref="Rect"/> 是否表示不同区域
        /// </summary>
        /// <param name="info"><see cref="LayoutInfo"/> 对象</param>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <returns><paramref name="info"/> 和 <paramref name="rect"/> 表示相同区域，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(LayoutInfo info, Rect rect) => info.X != rect.x || info.Y != rect.y || info.Width != rect.width || info.Height != rect.height;

        /// <summary>
        /// 判断 <see cref="LayoutInfo"/> 和 <see cref="Rect"/> 是否表示相同区域
        /// </summary>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <param name="info"><see cref="LayoutInfo"/> 对象</param>
        /// <returns><paramref name="rect"/> 和 <paramref name="info"/> 表示相同区域，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator ==(Rect rect, LayoutInfo info) => info.X == rect.x && info.Y == rect.y && info.Width == rect.width && info.Height == rect.height;

        /// <summary>
        /// 判断 <see cref="LayoutInfo"/> 和 <see cref="Rect"/> 是否表示不同区域
        /// </summary>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <param name="info"><see cref="LayoutInfo"/> 对象</param>
        /// <returns><paramref name="rect"/> 和 <paramref name="info"/> 表示相同区域，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool operator !=(Rect rect, LayoutInfo info) => info.X != rect.x || info.Y != rect.y || info.Width != rect.width || info.Height != rect.height;

        #endregion
    }
}
