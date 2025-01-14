﻿using HarmonyLib;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.Utilities;
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

            UIPatch.UIEvent += UIPatch_UIEvent;
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

        #endregion


        private void UIPatch_UIEvent(Harmony sender, UIEventType e)
        {
            // 对于 Label 等控件，不只是改变界面缩放会影响尺寸
            InvalidateMeasure();
        }
    }
}
