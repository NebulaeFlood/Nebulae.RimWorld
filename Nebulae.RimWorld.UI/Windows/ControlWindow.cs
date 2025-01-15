using HarmonyLib;
using Nebulae.RimWorld.UI.Controls;
using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 使用 <see cref="Control"/> 进行内容呈现的窗口
    /// </summary>
    public class ControlWindow : Window, IFrame, IUIEventListener
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

        private Control _content;

        private Rect _windowContentRectCache;

        private bool _isOpen = false;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 内容控件
        /// </summary>
        public Control Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    if (value is null)
                    {
                        _content.RemoveContainer();
                    }
                    else
                    {
                        value.SetContainer(this);
                    }

                    _content = value;

                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 窗口初始的大小
        /// </summary>
        public override Vector2 InitialSize => new Vector2(DefaultWindowWidth, DefaultWindowHeight);

        /// <summary>
        /// 窗口是否正在呈现
        /// </summary>
        public new bool IsOpen => _isOpen;

        #endregion


        /// <summary>
        /// 初始化 <see cref="ControlWindow"/> 的新实例
        /// </summary>
        public ControlWindow()
        {
            doCloseButton = true;
            doCloseX = true;

            Button button = new Button
            {
                Width = 220f,
                Height = 40f,
                Text = "Hello RimWorld!",
                ClickSound = null
            };
            button.Click += CloseWindow;

            Content = button;
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

        /// <inheritdoc/>
        public void InvalidateArrange() => _content?.InvalidateArrange();

        /// <inheritdoc/>
        public void InvalidateMeasure() => _content?.InvalidateMeasure();

        /// <inheritdoc/>
        public void InvalidateSegment()
        {
            if (_content is IFrame frame)
            {
                frame.InvalidateSegment();
            }
        }

        /// <inheritdoc/>
        public Rect Segment() => _windowContentRectCache;

        /// <summary>
        /// 开始呈现窗口
        /// </summary>
        public void Show() => Find.WindowStack.Add(this);

        /// <inheritdoc/>
        public void UIEventHandler(UIEventType type)
        {
            // if (type is UIEventType.ScaleChanged)
            // {
            //     InvalidateMeasure();
            // }

            // 对于 Label 等控件，不只是改变界面缩放会影响尺寸
            InvalidateMeasure();
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

            if (_windowContentRectCache != inRect)
            {

                if (_windowContentRectCache.IsPositionOnlyChanged(inRect))
                {
                    InvalidateArrange();
                }
                else
                {
                    InvalidateMeasure();
                }

                _windowContentRectCache = inRect;
            }

            _content?.Draw(inRect);
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
