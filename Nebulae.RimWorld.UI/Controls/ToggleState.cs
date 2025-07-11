using Nebulae.RimWorld.UI.Controls.Basic;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 指示 <see cref="ToggleButton"/> 的状态
    /// </summary>
    public enum ToggleState : short
    {
        /// <summary>
        /// 已开启
        /// </summary>
        Off = -1,
        /// <summary>
        /// 已关闭
        /// </summary>
        On = 1,
        /// <summary>
        /// 未确定
        /// </summary>
        Indeterminate = 0
    }
}
