using Nebulae.RimWorld.UI.Controls.Basic;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 可虚拟化子控件的面板控件的基类
    /// </summary>
    public abstract class VirtualizingPanel : Panel
    {
        /// <inheritdoc/>
        protected override bool IsDrawable(Visual child)
        {
            return !(child.RenderSize < Size.Epsilon)
                && ContentRect.Overlaps(child.RenderRect);
        }
    }
}
