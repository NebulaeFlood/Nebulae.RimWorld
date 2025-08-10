namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 指示控件的显示状态
    /// </summary>
    public enum Visibility : byte
    {
        /// <summary>
        /// 显示控件
        /// </summary>
        Visible,
        /// <summary>
        /// 不显示控件，但是在布局中为控件保留空间
        /// </summary>
        Hidden,
        /// <summary>
        /// 不显示控件，并且不在布局中为它保留空间
        /// </summary>
        Collapsed
    }
}
