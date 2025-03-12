using Nebulae.RimWorld.UI.Utilities;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 具有类似 <see cref="ImmediateWindow"/> 特性的窗口
    /// </summary>
    /// <remarks>要显示弹窗，使用 <see cref="Popup"/> 而不是 <see cref="ControlWindow.Show(bool)"/>。</remarks>
    public class PopupWindow : ControlWindow
    {
        internal bool Popped;

        /// <summary>
        /// 保持窗口在这一刻打开
        /// </summary>
        internal bool StayOpen;


        /// <summary>
        /// 初始化 <see cref="PopupWindow"/> 的新实例
        /// </summary>
        public PopupWindow()
        {
            closeOnAccept = false;
            closeOnCancel = false;
            focusWhenOpened = false;
            preventCameraMotion = false;
        }


        /// <summary>
        /// 显示弹窗
        /// </summary>
        /// <remarks>使用 <see cref="ControlWindow.Show(bool)"/> 将会以默认行为打开窗口。</remarks>
        public void Popup()
        {
            if (Popped)
            {
                StayOpen = true;
            }
            else
            {
                Popped = true;
                PopupWindowUtility.Manage(this);
                Find.WindowStack.Add(this);
            }
        }
    }
}
