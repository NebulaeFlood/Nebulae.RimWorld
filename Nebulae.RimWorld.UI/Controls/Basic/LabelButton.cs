using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 标签按钮控件
    /// </summary>
    public sealed class LabelButton : ButtonBase
    {
        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region Anchor
        /// <summary>
        /// 获取或设置 <see cref="LabelButton"/> 的文本锚点
        /// </summary>
        public TextAnchor Anchor
        {
            get { return (TextAnchor)GetValue(AnchorProperty); }
            set { SetValue(AnchorProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Anchor"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty AnchorProperty =
            DependencyProperty.Register(nameof(Anchor), typeof(TextAnchor), typeof(LabelButton),
                new PropertyMetadata(TextAnchor.MiddleCenter, OnAnchorChanged));

        private static void OnAnchorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var labelButton = (LabelButton)d;
            labelButton._drawer = TextUtility.CreateLabelDrawer((TextAnchor)e.NewValue, labelButton.FontSize);
        }
        #endregion

        #region FontSize
        /// <summary>
        /// 获取或设置 <see cref="LabelButton"/> 的字体大小
        /// </summary>
        public GameFont FontSize
        {
            get { return (GameFont)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="FontSize"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(LabelButton),
                new ControlPropertyMetadata(GameFont.Small, TextUtility.CoerceFontSize, OnFontSizeChanged, ControlRelation.Arrange));

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var labelButton = (LabelButton)d;
            labelButton._drawer = TextUtility.CreateLabelDrawer(labelButton.Anchor, (GameFont)e.NewValue);
        }
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置 <see cref="LabelButton"/> 的文本内容
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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(LabelButton),
                new ControlPropertyMetadata(string.Empty, CoerceText, ControlRelation.Measure));

        private static object CoerceText(DependencyObject d, object baseValue)
        {
            return string.IsNullOrWhiteSpace((string)baseValue) ? string.Empty : baseValue;
        }
        #endregion

        #endregion


        static LabelButton()
        {
            HeightProperty.OverrideMetadata(typeof(LabelButton),
                new ControlPropertyMetadata(24f, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="LabelButton"/> 的新实例
        /// </summary>
        public LabelButton()
        {
            ClickSound = SoundDefOf.Tick_Tiny;
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            var fontSize = (GameFont)GetValue(FontSizeProperty);
            var text = (string)GetValue(TextProperty);

            var width = text.CalculateLength(fontSize);

            // Usually means grid is calculating auto size
            if (availableSize.Width is 0f)
            {
                _cache = text;
            }
            else
            {
                width = availableSize.Width;
                _cache = string.IsNullOrEmpty(text) ? string.Empty : text.Truncate(width, fontSize);
            }

            return new Size(width, fontSize.GetHeight());
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            if (states.HasState(ControlState.Disabled))
            {
                GUI.color *= Widgets.InactiveColor;
            }
            else if (states.HasState(ControlState.Pressing))
            {
                GUI.DrawTexture(RegionRect, TexUI.HighlightSelectedTex, ScaleMode.StretchToFill);
            }
            else if (states.HasState(ControlState.CursorDirectlyOver))
            {
                GUI.DrawTexture(RegionRect, TexUI.HighlightTex, ScaleMode.StretchToFill);
            }

            _drawer(_cache, RenderRect);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly LabelCache DefaultDrawer = TextUtility.CreateLabelDrawer(TextAnchor.MiddleLeft, GameFont.Small);

        private string _cache = string.Empty;
        private LabelCache _drawer = DefaultDrawer;

        #endregion
    }
}
