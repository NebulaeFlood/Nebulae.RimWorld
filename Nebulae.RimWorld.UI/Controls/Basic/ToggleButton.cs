using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 可切换状态按钮的基类
    /// </summary>
    public abstract class ToggleButton : ButtonBase
    {
        //------------------------------------------------------
        //
        //  Dependency Property
        //
        //------------------------------------------------------

        #region Dependency Property

        #region State
        /// <summary>
        /// 获取或设置 <see cref="ToggleButton"/> 的状态
        /// </summary>
        public ToggleState State
        {
            get { return (ToggleState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="State"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(ToggleState), typeof(ToggleButton),
                new PropertyMetadata(ToggleState.Indeterminate, OnStateChanged));

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (ToggleButton)d;

            switch (e.NewValue)
            {
                case ToggleState.On:
                    button._icon = button._onIcon;
                    break;
                case ToggleState.Off:
                    button._icon = button._offIcon;
                    break;
                default:
                    button._icon = button._indeterminateIcon;
                    break;
            }
        }
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置 <see cref="ToggleButton"/> 的文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Text"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ToggleButton),
                new ControlPropertyMetadata(string.Empty, ControlRelation.Arrange));
        #endregion

        #endregion


        /// <summary>
        /// 为 <see cref="ToggleButton"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="onIcon">按钮开启时的图标</param>
        /// <param name="indeterminateIcon">按钮状态未确定时的图标</param>
        /// <param name="offIcon">按钮关闭时的图标</param>
        protected ToggleButton(Texture2D onIcon, Texture2D indeterminateIcon, Texture2D offIcon)
        {
            if (onIcon is null)
            {
                throw new ArgumentNullException(nameof(onIcon));
            }

            if (indeterminateIcon is null)
            {
                throw new ArgumentNullException(nameof(indeterminateIcon));
            }

            if (offIcon is null)
            {
                throw new ArgumentNullException(nameof(offIcon));
            }

            CursorEnterSound = SoundDefOf.Mouseover_Standard;

            _onIcon = onIcon;
            _indeterminateIcon = indeterminateIcon;
            _offIcon = offIcon;
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override sealed Rect ArrangeOverride(Rect availableRect)
        {
            _iconRect = new Size(24f).AlignToArea(availableRect, HorizontalAlignment.Right, VerticalAlignment.Center);

            var text = Text;

            if (RenderSize.Width < 24f || string.IsNullOrWhiteSpace(text))
            {
                _cache = string.Empty;
            }
            else
            {
                _cache = text.Truncate(RenderSize.Width - 24f, GameFont.Small);
            }

            return availableRect;
        }

        /// <inheritdoc/>
        protected override sealed void DrawCore(ControlState states)
        {
            if (states.HasState(ControlState.Disabled))
            {
                GUI.color *= Widgets.InactiveColor;
            }
            else if (states.HasState(ControlState.CursorDirectlyOver))
            {
                GUI.DrawTexture(RenderRect, TexUI.HighlightTex);
            }

            _cache.DrawLabel(RenderRect, TextAnchor.MiddleLeft);
            GUI.DrawTexture(_iconRect, _icon, ScaleMode.ScaleToFit);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Privte Fields
        //
        //------------------------------------------------------

        #region Privte Fields

        private string _cache = string.Empty;
        private Texture2D _icon;
        private Rect _iconRect;

        private readonly Texture2D _onIcon;
        private readonly Texture2D _indeterminateIcon;
        private readonly Texture2D _offIcon;

        #endregion
    }
}
