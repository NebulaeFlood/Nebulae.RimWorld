using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// <see cref="ScrollViewer"/> 水平方向初始滚动位置
    /// </summary>
    public enum HorizontalScrollOrigin
    {
        /// <summary>
        /// 从左侧开始
        /// </summary>
        Left,

        /// <summary>
        /// 从右侧开始
        /// </summary>
        Right
    }

    /// <summary>
    /// <see cref="ScrollViewer"/> 垂直方向初始滚动位置
    /// </summary>
    public enum VerticalScrollOrigin
    {
        /// <summary>
        /// 从顶部开始
        /// </summary>
        Top,

        /// <summary>
        /// 从底部开始
        /// </summary>
        Bottom
    }
}
