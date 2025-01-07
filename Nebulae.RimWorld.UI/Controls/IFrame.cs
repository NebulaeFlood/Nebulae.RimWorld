using Nebulae.RimWorld.UI.Windows;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 表示一个限制内容显示区域的控件
    /// </summary>
    /// <remarks>实现该接口的类应派生自 <see cref="Control"/> 或 <see cref="ControlWindow"/>。</remarks>
    public interface IFrame
    {
        /// <summary>
        /// 无效化控件分割
        /// </summary>
        void InvalidateSegment();

        /// <summary>
        /// 无效化控件布局
        /// </summary>
        void InvalidateArrange();

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        /// <remarks>这会导致控件布局也被无效化。</remarks>
        void InvalidateMeasure();

        /// <summary>
        /// 分割控件的可显示区域给内容控件
        /// </summary>
        /// <returns>内容控件的可显示区域。</returns>
        Rect Segment();
    }
}
