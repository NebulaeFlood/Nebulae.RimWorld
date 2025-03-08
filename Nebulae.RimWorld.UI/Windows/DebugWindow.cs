using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 用于显示控件信息的窗口
    /// </summary>
    public sealed class DebugWindow : ControlWindow
    {
        private static readonly DebugWindow _instance;
        private static readonly DebugPanel _debugPanel;


        static DebugWindow()
        {
            _debugPanel = new DebugPanel();
            _instance = new DebugWindow { Content = _debugPanel };
        }

        private DebugWindow()
        {
            draggable = true;
            doCloseX = true;
            drawShadow = false;
            resizeable = true;

            layer = WindowLayer.Super;

            InitialWidth = 300f;
            LayoutManager.DebugContent = DebugContent.Empty;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Method

        /// <inheritdoc/>
        public override void PostClose()
        {
            base.PostClose();
            _debugPanel.Reset();
        }

        /// <inheritdoc/>
        public override void PostOpen()
        {
            base.PostOpen();
            _debugPanel.ResetInfo();
        }

        /// <summary>
        /// 显示控件树信息
        /// </summary>
        /// <param name="root">根控件</param>
        public static void ShowWindow(Visual root)
        {
            _debugPanel.SetSourceTree(root);
            _instance.Show();
        }

        #endregion


        /// <inheritdoc/>
        protected override void SetInitialSizeAndPosition()
        {
            windowRect = new Rect(0f, 0f, InitialWidth, Verse.UI.screenHeight);
        }
    }
}
