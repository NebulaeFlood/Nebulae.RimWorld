using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 面板控件的基类，定义了其共同特性
    /// </summary>
    public abstract class Panel : FrameworkControl
    {
        private ControlCollection _children;
        private ScrollBarVisibility _horizontalScrollBarVisibility;
        private bool _horizontalScroll;
        private ScrollBarVisibility _verticalScrollBarVisibility;
        

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// <see cref="Panel"/> 包含的子控件
        /// </summary>
        /// <remarks>对于 <see cref="Grid"/>，部分项可能为 <see langword="null"/>。</remarks>
        public ControlCollection Children
        {
            get => _children;
            private set => _children = value;
        }

        /// <summary>
        /// 水平滚动条可见性
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => _horizontalScrollBarVisibility;
            set => _horizontalScrollBarVisibility = value;
        }

        /// <summary>
        /// 默认使用水平滚动
        /// </summary>
        public bool HorizontalScroll
        {
            get => _horizontalScroll;
            set => _horizontalScroll = value;
        }

        /// <summary>
        /// 垂直滚动条可见性
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => _verticalScrollBarVisibility;
            set => _verticalScrollBarVisibility = value;
        }

        #endregion
    }
}
