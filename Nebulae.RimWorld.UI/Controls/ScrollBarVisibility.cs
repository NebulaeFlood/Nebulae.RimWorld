namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 滚动栏的可见性
    /// </summary>
    public enum ScrollBarVisibility : byte
    {
        /// <summary>
        /// 当可见区域无法显示所有内容时显示
        /// </summary>
        Auto,

        /// <summary>
        /// 始终隐藏
        /// </summary>
        Hidden,

        /// <summary>
        /// 始终显示
        /// </summary>
        Visible
    }
}
