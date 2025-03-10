using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 使用 <see cref="Control"/> 进行内容呈现的窗口
    /// </summary>
    public class ControlWindow : Window, IUIEventListener
    {
        //------------------------------------------------------
        //
        //  Public Const
        //
        //------------------------------------------------------

        #region Public Const

        /// <summary>
        /// 默认的关闭按钮（X）要占用的高度
        /// </summary>
        public const float DefaultCloseButtonDesiredHeight = 26f;

        /// <summary>
        /// 默认的窗口高度
        /// </summary>
        public const float DefaultWindowHeight = 700f;

        /// <summary>
        /// 默认的窗口宽度
        /// </summary>
        public const float DefaultWindowWidth = 900f;

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly LayoutManager _layoutManager;

        private Rect _clientRect;
        private Rect _nonClientRect;

        private Thickness _padding = 18f;

        private float _initialHeight = DefaultWindowHeight;
        private float _initialWidth = DefaultWindowWidth;

        #endregion


        internal bool isOpen;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 窗口的客户区域
        /// </summary>
        public Rect ClientRect => _clientRect;

        /// <summary>
        /// 内容控件
        /// </summary>
        public Visual Content
        {
            get => _layoutManager.Root;
            set => _layoutManager.Root = value;
        }

        /// <summary>
        /// 窗口初始的大小
        /// </summary>
        public sealed override Vector2 InitialSize => new Vector2(_initialWidth, _initialHeight);

        /// <summary>
        /// 窗口的初始高度
        /// </summary>
        public float InitialHeight
        {
            get => _initialHeight;
            set
            {
                if (_initialHeight != value)
                {
                    _initialHeight = value;

                    _layoutManager.InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// 窗口控件布局管理器
        /// </summary>
        public LayoutManager LayoutManager => _layoutManager;

        /// <summary>
        /// 窗口是否正在呈现
        /// </summary>
        public new bool IsOpen => isOpen;

        /// <summary>
        /// 窗口的初始宽度
        /// </summary>
        public float InitialWidth
        {
            get => _initialWidth;
            set
            {
                if (_initialWidth != value)
                {
                    _initialWidth = value;

                    _layoutManager.InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// 窗口的非客户区
        /// </summary>
        public Rect NonClientRect => _nonClientRect;


        /// <summary>
        /// 窗口内容边距
        /// </summary>
        public Thickness Padding
        {
            get => _padding;
            set => _padding = value;
        }

        #endregion


        /// <summary>
        /// 窗口内容边距
        /// </summary>
        protected override sealed float Margin => 0f;


#if DEBUG
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        ~ControlWindow()
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
            System.Diagnostics.Debug.WriteLine($"[NebulaeFlood's Lib] A window of type {GetType()} has been collected.");
        }
#endif

        /// <summary>
        /// 初始化 <see cref="ControlWindow"/> 的新实例
        /// </summary>
        public ControlWindow()
        {
            _layoutManager = new LayoutManager(this);
            UIPatch.UIEvent.Manage(this);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="doCloseSound">是否播放关闭窗口的音效</param>
        public override void Close(bool doCloseSound = true)
        {
            if (isOpen)
            {
                Find.WindowStack.TryRemove(this, doCloseSound);
            }
        }

        /// <summary>
        /// 开始呈现窗口
        /// </summary>
        /// <param name="closeThenShow">是否关闭正在显示的窗口再打开</param>
        public void Show(bool closeThenShow = true)
        {
            if (closeThenShow)
            {
                Find.WindowStack.Add(this);
            }
            else if (isOpen)
            {
                SetInitialSizeAndPosition();
            }
            else
            {
                Find.WindowStack.Add(this);
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Virtual Methods
        //
        //------------------------------------------------------

        #region Public Virtual Methods

        /// <summary>
        /// 绘制窗口内容
        /// </summary>
        /// <param name="inRect">窗口允许绘制的区域</param>
        public sealed override void DoWindowContents(Rect inRect)
        {
            _nonClientRect = inRect;
            inRect -= _padding;

            if (doCloseX)
            {
                inRect.yMin += Mathf.Abs(_padding.Top - DefaultCloseButtonDesiredHeight);
            }

            if (doCloseButton)
            {
                inRect.height -= Mathf.Max(_padding.Bottom, FooterRowHeight);
            }

            if (_clientRect != inRect)
            {
                _layoutManager.InvalidateLayout();
                _clientRect = inRect;
            }

            _layoutManager.Draw(inRect);

            if (Prefs.DevMode
                && _layoutManager.DrawDebugButtons)
            {
                _layoutManager.DrawWindowDebugButtons(_nonClientRect);
            }
        }

        /// <inheritdoc/>
        public virtual void HandleUIEvent(UIEventType type)
        {
            _layoutManager.InvalidateLayout();
        }

        /// <summary>
        /// 当窗口关闭时引发的操作
        /// </summary>
        public override void PostClose()
        {
            base.PostClose();
            isOpen = false;
        }

        /// <summary>
        /// 当窗口打开时执行的操作
        /// </summary>
        public override void PostOpen()
        {
            base.PostOpen();
            isOpen = true;
        }

        #endregion
    }
}
