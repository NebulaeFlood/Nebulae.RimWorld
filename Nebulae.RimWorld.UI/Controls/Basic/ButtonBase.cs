using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 按钮的 UI 状态
    /// </summary>
    [Flags]
    public enum ButtonStatus : int
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        Normal = 0b000,

        /// <summary>
        /// 禁止状态
        /// </summary>
        Disabled = 0b001,

        /// <summary>
        /// 高亮状态
        /// </summary>
        Hovered = 0b010,

        /// <summary>
        /// 按下状态
        /// </summary>
        Pressed = 0b110,
    }

    /// <summary>
    /// 所有按钮控件的基类，定义了其共同特性
    /// </summary>
    public abstract class ButtonBase : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private SoundDef _clickSound;
        private SoundDef _cursorOverSound;

        private bool _playCursorOverSound;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 当光标位于按钮上方时播放的音效
        /// </summary>
        public SoundDef CursorOverSound
        {
            get => _cursorOverSound;
            set => _cursorOverSound = value;
        }

        /// <summary>
        /// 单击按钮时触发的声音
        /// </summary>
        public SoundDef ClickSound
        {
            get => _clickSound;
            set => _clickSound = value;
        }

        #region FontSize
        /// <summary>
        /// 获取或设置字体大小
        /// </summary>
        public GameFont FontSize
        {
            get { return (GameFont)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="FontSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(ButtonBase),
                new PropertyMetadata(GameFont.Small));
        #endregion

        /// <summary>
        /// 当光标位于按钮上方时是否播放音效
        /// </summary>
        public bool PlayCursorOverSound
        {
            get => _playCursorOverSound;
            set => _playCursorOverSound = value;
        }

        #endregion


        /// <summary>
        /// 为 <see cref="ButtonBase"/> 派生类实现基本初始化
        /// </summary>
        protected ButtonBase()
        {
            HitTestVisible = true;
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 绘制按钮
        /// </summary>
        /// <param name="status">按钮状态</param>
        protected abstract void DrawButton(ButtonStatus status);

        /// <inheritdoc/>
        protected sealed override void DrawCore()
        {
            bool isPressing = IsPressing;
            bool isCursorOver = MouseUtility.LeftButtonPressing
                ? isPressing
                : IsCursorOver;

            ButtonStatus status;

            if (IsEnabled)
            {
                status = ButtonStatus.Normal;
            }
            else
            {
                status = ButtonStatus.Disabled;
            }

            if (isPressing)
            {
                status |= ButtonStatus.Pressed;
            }
            else if (isCursorOver)
            {
                status |= ButtonStatus.Hovered;
            }

            DrawButton(status);
        }

        /// <inheritdoc/>
        protected internal override void OnClick()
        {
            _clickSound?.PlayOneShotOnCamera();
        }

        /// <inheritdoc/>
        protected internal override void OnCursorEnter()
        {
            if (MouseUtility.IsPressing
                || !IsEnabled
                || !_playCursorOverSound
                || _cursorOverSound is null)
            {
                return;
            }

            _cursorOverSound.PlayOneShotOnCamera();
        }

        /// <summary>
        /// 播放点击音效
        /// </summary>
        protected void PlayClickSound()
        {
            _clickSound?.PlayOneShotOnCamera();
        }

        #endregion
    }
}
