using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 标签控件
    /// </summary>
    public sealed class Label : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region Anchor
        /// <summary>
        /// 获取或设置 <see cref="Label"/> 的文本锚点
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
            DependencyProperty.Register(nameof(Anchor), typeof(TextAnchor), typeof(Label),
                new PropertyMetadata(TextAnchor.MiddleCenter, OnAnchorChanged));

        private static void OnAnchorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var label = (Label)d;
            label._drawer = TextUtility.CreateLabelDrawer((TextAnchor)e.NewValue, label.FontSize);
        }
        #endregion

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
        /// 标识 <see cref="FontSize"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(Label),
                new ControlPropertyMetadata(GameFont.Small, TextUtility.CoerceFontSize, OnFontSizeChanged, ControlRelation.Arrange));

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var label = (Label)d;
            label._drawer = TextUtility.CreateLabelDrawer(label.Anchor, (GameFont)e.NewValue);
        }
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置 <see cref="Label"/> 的文本内容
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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Label),
                new ControlPropertyMetadata(string.Empty, CoerceText, ControlRelation.Measure));

        private static object CoerceText(DependencyObject d, object baseValue)
        {
            return string.IsNullOrWhiteSpace((string)baseValue) ? string.Empty : baseValue;
        }
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="Label"/> 的新实例
        /// </summary>
        public Label() { }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            var fontSize = FontSize;
            var text = Text;

            _cache = string.IsNullOrEmpty(text) 
                ? string.Empty 
                : text.Truncate(availableSize.Width, fontSize);

            return new Size(availableSize.Width, fontSize.GetHeight());
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
