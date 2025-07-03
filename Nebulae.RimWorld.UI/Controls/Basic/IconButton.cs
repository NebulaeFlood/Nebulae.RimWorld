using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 图标按钮
    /// </summary>
    public sealed class IconButton : ButtonBase
    {
        /// <summary>
        /// 获取或设置 <see cref="IconButton"/> 的图标
        /// </summary>
        public Texture2D Icon
        {
            get => _icon;
            set => _icon = value ?? throw new InvalidOperationException($"Cannot asign a null value to {Type}.{nameof(Icon)}.");
        }


        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region Composition
        /// <summary>
        /// 获取或设置 <see cref="Button"/> 的混合色
        /// </summary>
        public Color Composition
        {
            get { return (Color)GetValue(CompositionProperty); }
            set { SetValue(CompositionProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Composition"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty CompositionProperty =
            DependencyProperty.Register(nameof(Composition), typeof(Color), typeof(IconButton),
                new PropertyMetadata(new Color(1f, 1f, 1f, 1f), OnCompositionChanged));

        private static void OnCompositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (IconButton)d;
            var states = button.ControlStates;

            if (states.HasState(ControlState.Disabled))
            {
                button._composition = (Color)e.NewValue * Widgets.InactiveColor;
            }
            else if (states.HasState(ControlState.CursorDirectlyOver))
            {
                button._composition = (Color)e.NewValue * button.Highlight;
            }
            else
            {
                button._composition = (Color)e.NewValue;
            }
        }
        #endregion

        #region Highlight
        /// <summary>
        /// 获取或设置 <see cref="IconButton"/> 的高亮颜色
        /// </summary>
        public Color Highlight
        {
            get { return (Color)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Highlight"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register(nameof(Highlight), typeof(Color), typeof(IconButton),
                new PropertyMetadata(GenUI.MouseoverColor, OnHighlightChanged));

        private static void OnHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (IconButton)d;
            button._composition = button.ControlStates.HasState(ControlState.Disabled) ? Widgets.InactiveColor : (Color)e.NewValue;
        }
        #endregion

        #endregion


        static IconButton()
        {
            WidthProperty.OverrideMetadata(typeof(IconButton),
                new ControlPropertyMetadata(24f, ControlRelation.Measure));
            HeightProperty.OverrideMetadata(typeof(IconButton),
                new ControlPropertyMetadata(24f, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="IconButton"/> 的新实例
        /// </summary>
        /// <param name="icon">按钮图标</param>
        public IconButton(Texture2D icon)
        {
            _icon = icon ?? throw new ArgumentNullException(nameof(icon));
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            GUI.color *= _composition;
            GUI.DrawTexture(RenderRect, _icon, ScaleMode.ScaleToFit);
        }

        /// <inheritdoc/>
        protected override void OnStatesChanged(ControlState states)
        {
            if (ControlStates.HasState(ControlState.Disabled))
            {
                _composition = Composition * Widgets.InactiveColor;
            }
            else if (ControlStates.HasState(ControlState.CursorDirectlyOver))
            {
                _composition = Composition * Highlight;
            }
            else
            {
                _composition = Composition;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Color _composition = new Color(1f, 1f, 1f, 1f);

        private Texture2D _icon;

        #endregion
    }
}
