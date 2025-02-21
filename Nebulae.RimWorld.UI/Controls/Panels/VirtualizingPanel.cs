namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 可虚拟化子控件的面板控件的基类
    /// </summary>
    public abstract class VirtualizingPanel : Panel
    {
        /// <inheritdoc/>
        protected override bool IsDrawable(Control child)
        {
            return child.RenderSize.Height > float.Epsilon
                && child.RenderSize.Width > float.Epsilon
                && ContentRect.Overlaps(child.RenderRect);
        }
    }
}
