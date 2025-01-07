using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Patches;
using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 使用 <see cref="Control"/> 进行内容呈现的窗口
    /// </summary>
    public class ControlWindow : Window, IFrame
    {
        /// <summary>
        /// 默认的关闭按钮（X）要占用的高度
        /// </summary>
        public const float DefaultCloseButtonDesiredHeight = 26f;

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Control _content;

        private bool _isSegmentValid = false;

        private Rect _windowContentRectCache = Rect.zero;

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
        /// 是否绘制窗口底部的窗口关闭按钮
        /// </summary>
        public bool DrawCloseButton
        {
            get => doCloseButton;
            set => doCloseButton = value;
        }

        /// <summary>
        /// 是否绘制标题栏的窗口关闭按钮
        /// </summary>
        public bool DrawTitleCloseButton
        {
            get => doCloseX;
            set => doCloseX = value;
        }

        /// <summary>
        /// 窗口初始的大小
        /// </summary>
        public override Vector2 InitialSize => new Vector2(900f, 700f);

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
            button.Click += OnDefaultButtonClicked;

            Content = button;

            ResolutionUtility_Patch.ScaleChanged += ResolutionUtility_Patch_ScaleChanged;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 关闭窗口并播放 <see cref="Window.soundClose"/> 设置的音效
        /// </summary>
        public void Close() => Close(true);

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
        public Rect Segment() => windowRect;

        /// <summary>
        /// 开始呈现窗口
        /// </summary>
        public void Show() => Find.WindowStack.Add(this);

        #endregion


        //------------------------------------------------------
        //
        //  Public Virtual Methods
        //
        //------------------------------------------------------

        #region Public Virtual Methods

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="doCloseSound">是否播放 <see cref="Window.soundClose"/> 设置的音效</param>
        public sealed override void Close(bool doCloseSound = true) => base.Close(doCloseSound);

        /// <summary>
        /// 绘制窗口内容
        /// </summary>
        /// <param name="inRect">窗口允许绘制的区域</param>
        public sealed override void DoWindowContents(Rect inRect)
        {
            if (doCloseX && Margin < DefaultCloseButtonDesiredHeight)
            {
                float offset = DefaultCloseButtonDesiredHeight - Margin;
                inRect.y += offset;
                inRect.height -= offset;
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

                InvalidateSegment();

                _windowContentRectCache = inRect;
            }

            _content?.Draw(inRect);
        }

        /// <summary>
        /// 额外的绘制内容，可在窗口外部绘制，坐标相对于整个游戏界面的左上角
        /// </summary>
        /// <remarks>此处使用 <see cref="GUI"/>, <see cref="Widgets"/> 等类进行绘制。</remarks>
        public override void ExtraOnGUI() { }

        #endregion


        /// <summary>
        /// 当界面缩放 <see cref="Prefs.UIScale"/> 变化时执行的操作
        /// </summary>
        /// <param name="newScale">新的缩放系数</param>
        protected virtual void OnScaleChanged(float newScale) { }


        private static void OnDefaultButtonClicked(ButtonBase button, EventArgs args)
        {
            ((Window)button.Container).Close();
        }

        private void ResolutionUtility_Patch_ScaleChanged(object sender, float newScale)
        {
            InvalidateMeasure();
            OnScaleChanged(newScale);
        }
    }
}
