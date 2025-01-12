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


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private SoundDef _clickSound = SoundDefOf.Mouseover_Standard;
        private SoundDef _mouseOverSound;

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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ButtonBase),
                new PropertyMetadata(string.Empty));
        #endregion

        #endregion


        /// <summary>
        /// 为 <see cref="ButtonBase"/> 派生类实现基本初始化
        /// </summary>
        protected ButtonBase()
        {
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
        /// <param name="renderRect">允许绘制的区域</param>
        /// <param name="isEnabled">按钮是否被启用</param>
        /// <param name="isCursorOver">光标是否位于按钮上方</param>
        /// <param name="isPressing">按钮是否被按下</param>
        /// <returns>实际绘制的区域。</returns>
        protected abstract Rect DrawButton(
            Rect renderRect,
            bool isEnabled,
            bool isCursorOver,
            bool isPressing);

        /// <inheritdoc/>
        protected sealed override Rect DrawCore(Rect renderRect)
        {
            EventType eventType = Event.current.type;
            Rect visiableRect = Segment().IntersectWith(renderRect);

            bool isCursorOver = visiableRect.Contains(Event.current.mousePosition);
            bool isPressing = isCursorOver && Input.GetMouseButton(0);

            renderRect = DrawButton(
                renderRect,
                _isEnabled,
                isCursorOver,
                isPressing);

            if (_playMouseOverSound)
            {
                MouseoverSounds.DoRegion(visiableRect, _mouseOverSound);
            }

            if (!_isEnabled)
            {
                return renderRect;
            }

            if (isCursorOver 
                && eventType is EventType.MouseUp)
            {
                OnClick();
                click.Invoke(this, EventArgs.Empty);
                _clickSound?.PlayOneShotOnCamera();
            }

            return renderRect;
        }

        /// <summary>
        /// 按钮被单击时发生
        /// </summary>
        protected virtual void OnClick() { }

        #endregion
    }
}
