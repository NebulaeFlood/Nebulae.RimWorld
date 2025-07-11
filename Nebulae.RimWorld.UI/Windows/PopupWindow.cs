using Nebulae.RimWorld.UI.Utilities;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 具有类似 <see cref="ImmediateWindow"/> 特性的窗口
    /// </summary>
    /// <remarks>要显示弹窗，请使用 <see cref="Popup"/>。</remarks>
    public class PopupWindow : ControlWindow
    {
        /// <summary>
        /// 初始化 <see cref="PopupWindow"/> 的新实例
        /// </summary>
        public PopupWindow()
        {
            closeOnAccept = false;
            closeOnCancel = false;
            doCloseButton = false;
            doCloseX = false;
            focusWhenOpened = false;
            preventCameraMotion = false;
            soundAppear = null;
            soundClose = null;
        }


        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="doCloseSound">是否播放关闭窗口的音效</param>
        public override void Close(bool doCloseSound = true)
        {
            if (!_popuped)
            {
                return;
            }

            _stayOpen = false;
            Find.WindowStack.TryRemove(this, doCloseSound);
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        /// <remarks>除此方法以外无法打开弹窗。</remarks>
        public void Popup()
        {
            if (_popuped)
            {
                _stayOpen = true;
            }
            else
            {
                _popuped = true;
                _stayOpen = true;
                PopupWindowUtility.Manage(this);
                Find.WindowStack.Add(this);
            }
        }

        /// <summary>
        /// 窗口被关闭后执行的方法
        /// </summary>
        public override void PostClose()
        {
            _popuped = false;
        }


        internal bool _stayOpen;

        private bool _popuped;
    }
}
