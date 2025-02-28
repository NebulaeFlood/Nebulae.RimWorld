using Nebulae.RimWorld.UI.Data;
using RimWorld;
using System;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls
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
        #region Clicked

        private readonly WeakEvent<ButtonBase, EventArgs> _clicked = new WeakEvent<ButtonBase, EventArgs>();

        /// <summary>
        /// 单击按钮时发生的弱事件
        /// </summary>
        public event Action<ButtonBase, EventArgs> Clicked
        {
            add => _clicked.Add(value, value.Invoke);
            remove => _clicked.Remove(value);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private SoundDef _clickSound = SoundDefOf.Click;
        private SoundDef _cursorOverSound = SoundDefOf.Mouseover_Standard;

        private bool _cursorOverSoundPlayed = false;
        private bool _isEnabled = true;
        private bool _playMouseOverSound = true;

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
        /// 按钮是否启用
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        /// <summary>
        /// 当光标位于按钮上方时是否播放音效
        /// </summary>
        public bool PlayMouseOverSound
        {
            get => _playMouseOverSound;
            set => _playMouseOverSound = value;
        }

        #region Text
        /// <summary>
        /// 获取或设置按钮文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Text"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ButtonBase),
                new PropertyMetadata(string.Empty));
        #endregion

        #endregion


        /// <summary>
        /// 为 <see cref="ButtonBase"/> 派生类实现基本初始化
        /// </summary>
        protected ButtonBase()
        {
            IsHitTestVisible = true;
        }


        internal void Click()
        {
            OnClick();
            _clicked.Invoke(this, EventArgs.Empty);
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
            bool isCursorOver = IsCursorOver;
            bool isPressing = IsPressing;

            ButtonStatus status;

            if (!_isEnabled)
            {
                status = ButtonStatus.Disabled;
            }
            else if (isPressing)
            {
                status = ButtonStatus.Pressed;
            }
            else if (isCursorOver)
            {
                status = ButtonStatus.Hovered;
            }
            else
            {
                status = ButtonStatus.Normal;
            }

            DrawButton(status);

            if (!_playMouseOverSound)
            {
                return;
            }

            if (isCursorOver || isPressing)
            {
                if (!_cursorOverSoundPlayed)
                {
                    _cursorOverSoundPlayed = true;
                    _cursorOverSound.PlayOneShotOnCamera();
                }
            }
            else
            {
                _cursorOverSoundPlayed = false;
            }
        }

        /// <summary>
        /// 按钮被单击时执行的方法
        /// </summary>
        protected virtual void OnClick()
        {
            _clickSound?.PlayOneShotOnCamera();
        }

        #endregion
    }
}
