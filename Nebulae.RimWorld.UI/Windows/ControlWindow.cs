using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Utilities;
using System;
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


        internal readonly LayoutManager LayoutManager;


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Rect _clientRect;

        private bool _isOpen = false;

        private float _initialHeight = DefaultWindowHeight;
        private float _initialWidth = DefaultWindowWidth;

        #endregion


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
        public Control Content
        {
            get => LayoutManager.Root;
            set
            {
                Control root = LayoutManager.Root;

                if (!ReferenceEquals(root, value))
                {
                    root?.RemoveParent();

                    if (value is null)
                    {
                        LayoutManager.Root = null;
                    }
                    else
                    {
                        value.Owner = this;
                        LayoutManager.Root = value;
                    }
                }
            }
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

                    LayoutManager.InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// 窗口是否正在呈现
        /// </summary>
        public new bool IsOpen => _isOpen;

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

                    LayoutManager.InvalidateLayout();
                }
            }
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="ControlWindow"/> 的新实例
        /// </summary>
        public ControlWindow()
        {
            doCloseButton = true;
            doCloseX = true;

            LayoutManager = new LayoutManager(this);

            UIPatch.UIEvent.Manage(this);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 点击 <see cref="ButtonBase"/> 后关闭窗口的 <see cref="ButtonBase.Click"/> 事件处理器
        /// </summary>
        /// <param name="button">被点击的按钮</param>
        /// <param name="args">事件数据</param>
        public void CloseWindow(ButtonBase button, EventArgs args) => Close();

        /// <summary>
        /// 创建默认内容
        /// </summary>
        /// <returns>默认内容控件。</returns>
        public virtual Control CreateDefaultContent()
        {
            Button button = new Button
            {
                Width = 220f,
                Height = 40f,
                Text = "Hello RimWorld!",
                ClickSound = null
            };
            button.Click += CloseWindow;

            return button;
        }

        /// <summary>
        /// 开始呈现窗口
        /// </summary>
        public void Show() => Find.WindowStack.Add(this);

        /// <inheritdoc/>
        public void UIEventHandler(UIEventType type)
        {
            LayoutManager.InvalidateLayout();
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
            if (doCloseX && Margin < DefaultCloseButtonDesiredHeight)
            {
                inRect.yMin += DefaultCloseButtonDesiredHeight - Margin;
            }

            if (doCloseButton)
            {
                inRect.height -= FooterRowHeight;
            }

            if (_clientRect != inRect)
            {
                if (!_clientRect.IsPositionOnlyChanged(inRect))
                {
                    LayoutManager.InvalidateLayout();
                }

                _clientRect = inRect;
            }

            LayoutManager.Draw(inRect);
        }

        /// <summary>
        /// 当窗口关闭时引发的操作
        /// </summary>
        public override void PostClose()
        {
            base.PostClose();
            _isOpen = false;
        }

        /// <summary>
        /// 当窗口打开时执行的操作
        /// </summary>
        public override void PostOpen()
        {
            base.PostOpen();
            _isOpen = true;
        }

        #endregion
    }
}
