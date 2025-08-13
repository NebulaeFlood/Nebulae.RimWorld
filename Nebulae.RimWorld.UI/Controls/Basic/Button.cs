using Nebulae.RimWorld.UI.Controls.Resources;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 按钮控件
    /// </summary>
    public class Button : ButtonBase
    {
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
            DependencyProperty.Register(nameof(Composition), typeof(Color), typeof(Button),
                new PropertyMetadata(new Color(1f, 1f, 1f, 1f), OnCompositionChanged));

        private static void OnCompositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (Button)d;

            button._composition = button.Distructive
                ? ((Color)e.NewValue) * ButtonResources.DistructiveComposition
                : ((Color)e.NewValue);

            if (button.ControlStates.HasState(ControlState.Disabled))
            {
                button._composition *= Widgets.InactiveColor;
            }
        }
        #endregion

        #region Distructive
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="Button"/> 的功能是否具有破坏性
        /// </summary>
        /// <remarks>将会影响按钮的混合色</remarks>
        public bool Distructive
        {
            get { return (bool)GetValue(DistructiveProperty); }
            set { SetValue(DistructiveProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Distructive"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty DistructiveProperty =
            DependencyProperty.Register(nameof(Distructive), typeof(bool), typeof(Button),
                new PropertyMetadata(false, OnDistructiveChanged));

        private static void OnDistructiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (Button)d;

            button._composition = (bool)e.NewValue
                ? button.Composition * ButtonResources.DistructiveComposition
                : button.Composition;

            if (button.ControlStates.HasState(ControlState.Disabled))
            {
                button._composition *= Widgets.InactiveColor;
            }
        }
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置 <see cref="Button"/> 的文本
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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Button),
                new ControlPropertyMetadata(string.Empty, ControlRelation.Arrange));
        #endregion

        #endregion


        static Button()
        {
            ClickSoundProperty.OverrideMetadata(typeof(Button),
                new PropertyMetadata(SoundDefOf.Click));
            CursorEnterSoundProperty.OverrideMetadata(typeof(Button),
                new PropertyMetadata(SoundDefOf.Mouseover_Standard));
        }

        /// <summary>
        /// 初始化 <see cref="Button"/> 的新实例
        /// </summary>
        public Button() { }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// inheritdoc/>
        protected override Rect ArrangeOverride(Rect availableRect)
        {
            var text = Text;

            _cache = string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Truncate(RenderSize.Width - 10f, GameFont.Small);

            return availableRect;
        }

        /// inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            GUI.color *= _composition;

            Widgets.DrawAtlas(RegionRect, _background);
            _cache.DrawLabel(RenderRect);
        }

        /// <inheritdoc/>
        protected override void OnStatesChanged(ControlState states)
        {
            _composition = Composition;

            if (Distructive)
            {
                _composition *= ButtonResources.DistructiveComposition;
            }

            if (states.HasState(ControlState.Disabled))
            {
                _background = ButtonResources.NormalBackground;
                _composition *= Widgets.InactiveColor;
            }
            else if (states.HasState(ControlState.Pressing))
            {
                _background = ButtonResources.PressedBackground;
            }
            else if (states.HasState(ControlState.CursorDirectlyOver))
            {
                _background = ButtonResources.CursorOverBackground;
            }
            else
            {
                _background = ButtonResources.NormalBackground;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Texture2D _background = ButtonResources.NormalBackground;
        private Color _composition = new Color(1f, 1f, 1f, 1f);

        private string _cache = string.Empty;

        #endregion
    }
}
