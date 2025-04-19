using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 保存布局区域信息的结构
    /// </summary>
    public readonly struct LayoutInfo : IEquatable<LayoutInfo>
    {
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


        /// <summary>
        /// 初始化 <see cref="LayoutInfo"/> 的新实例
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">宽度</param>
        public LayoutInfo(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


        /// <summary>
        /// 判断指定对象是否等于此 <see cref="LayoutInfo"/> 
        /// </summary>
        /// <param name="obj">要与此 <see cref="LayoutInfo"/> 进行比较的对象</param>
        /// <returns>如果指定的对象等于此 <see cref="LayoutInfo"/> ，则返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj) => obj is LayoutInfo info && this == info;

        /// <summary>
        /// 判断此 <see cref="LayoutInfo"/> 是否与另一个 <see cref="LayoutInfo"/> 表示相同区域
        /// </summary>
        /// <param name="other">用于比较的对象</param>
        /// <returns>
        /// 如果此 <see cref="LayoutInfo"/> 和另一个 <see cref="LayoutInfo"/> 表示相同区域，
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
        /// 将 <see cref="Rect"/> 隐式转换为 <see cref="LayoutInfo"/>
        /// </summary>
        /// <param name="rect"><see cref="Rect"/> 对象</param>
        /// <returns>返回一个新的 <see cref="LayoutInfo"/> 对象。</returns>
        public static implicit operator LayoutInfo(Rect rect) => new LayoutInfo(rect.x, rect.y ,rect.width, rect.height);

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
    }
}
