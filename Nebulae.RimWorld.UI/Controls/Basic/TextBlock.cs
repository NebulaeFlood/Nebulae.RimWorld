using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 文本块控件
    /// </summary>
    public sealed class TextBlock : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region FontSize
        /// <summary>
        /// 获取或设置 <see cref="TextBlock"/> 的字体大小
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
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(TextBlock),
                new ControlPropertyMetadata(GameFont.Small, TextUtility.CoerceFontSize, OnFontSizeChanged, ControlRelation.Measure));

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = (TextBlock)d;
            textBlock._drawer = TextUtility.CreateLabelDrawer(TextAnchor.MiddleCenter, (GameFont)e.NewValue);
        }
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置 <see cref="TextBlock"/> 的文本
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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBlock),
                new ControlPropertyMetadata(string.Empty, CoerceText, OnTextChanged, ControlRelation.Measure));

        private static object CoerceText(DependencyObject d, object baseValue)
        {
            return string.IsNullOrWhiteSpace((string)baseValue) ? string.Empty : baseValue;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBlock)d)._cache = (string)e.NewValue;
        }
        #endregion

        #endregion


        static TextBlock()
        {
            VerticalAlignmentProperty.OverrideMetadata(typeof(TextBlock),
                new PropertyMetadata(VerticalAlignment.Top));
        }

        /// <summary>
        /// 初始化 <see cref="TextBlock"/> 的新实例
        /// </summary>
        public TextBlock() { }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            return _cache.CalculateSize(availableSize.Width, FontSize, TextAnchor.MiddleCenter);
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            if (states.HasState(ControlState.Disabled))
            {
                GUI.color *= Widgets.InactiveColor;
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

        private static readonly LabelCache DefaultDrawer = TextUtility.CreateLabelDrawer(TextAnchor.MiddleCenter, GameFont.Small);

        private string _cache = string.Empty;
        private LabelCache _drawer = DefaultDrawer;

        #endregion
    }
}
