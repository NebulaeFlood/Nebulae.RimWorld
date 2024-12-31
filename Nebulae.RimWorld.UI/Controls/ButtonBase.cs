using Nebulae.RimWorld.UI.Data;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 所有按钮控件的基类，定义了其共同特性
    /// </summary>
    public abstract class ButtonBase : ContentControl
    {
        #region Click
        private readonly WeakEvent<ButtonBase, EventArgs> click = new WeakEvent<ButtonBase, EventArgs>();

        /// <summary>
        /// 单击按钮时发生的弱事件
        /// </summary>
        public event WeakEventHandler<ButtonBase, EventArgs> Click
        {
            add => click.Add(value);
            remove => click.Remove(value);
        }
        #endregion

        private SoundDef _clickSound;
        private bool _isEnabled;
        private SoundDef _mouseOverSound;
        private bool _playMouseOverSound;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

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
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(Label),
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
        /// 当光标位于按钮上方时播放的音效
        /// </summary>
        public SoundDef MouseOverSound
        {
            get => _mouseOverSound;
            set => _mouseOverSound = value;
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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Label),
                new PropertyMetadata(string.Empty));
        #endregion

        #endregion


        /// <summary>
        /// 为 <see cref="ButtonBase"/> 派生类实现基本初始化
        /// </summary>
        protected ButtonBase()
        {
            _isEnabled = true;
            _playMouseOverSound = true;
            _mouseOverSound = SoundDefOf.Mouseover_Standard;
        }


        /// <summary>
        /// 绘制按钮
        /// </summary>
        /// <param name="renderRect">允许绘制的区域</param>
        /// <returns>实际绘制的区域。</returns>
        protected abstract Rect DrawButton(Rect renderRect);

        /// <inheritdoc/>
        protected sealed override Rect DrawCore(Rect renderRect)
        {
            Color currentColor = GUI.color;
            if (!_isEnabled)
            {
                GUI.color = Widgets.InactiveColor;
            }

            renderRect = DrawButton(renderRect);

            if (_playMouseOverSound) { MouseoverSounds.DoRegion(renderRect, _mouseOverSound); }
            if (!_isEnabled) { return renderRect; }
            if (GUI.Button(renderRect, string.Empty, Widgets.EmptyStyle))
            {
                OnClick();
                click.Invoke(this, EventArgs.Empty);
                _clickSound?.PlayOneShotOnCamera();
            }

            GUI.color = currentColor;
            return renderRect;
        }

        /// <summary>
        /// 按钮被单击时发生
        /// </summary>
        protected virtual void OnClick() { }
    }
}
