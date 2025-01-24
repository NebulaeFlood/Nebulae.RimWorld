using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using UnityEngine;
using Verse;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 标签控件
    /// </summary>
    public class Label : Control
    {
        private TextAnchor _anchor = TextAnchor.MiddleCenter;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置文本锚点
        /// </summary>
        public TextAnchor Anchor
        {
            get => _anchor;
            set => _anchor = value;
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
                new ControlPropertyMetadata(GameFont.Small, ControlRelation.Measure));
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置标签文本
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
                new ControlPropertyMetadata(string.Empty, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="Label"/> 的新实例
        /// </summary>
        public Label()
        {
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            HorizontalAlignment horizontalAlignment;

            if (_anchor is TextAnchor.UpperLeft
                || _anchor is TextAnchor.MiddleLeft
                || _anchor is TextAnchor.LowerLeft)
            {
                horizontalAlignment = HorizontalAlignment.Left;
            }
            else if (_anchor is TextAnchor.UpperRight
                || _anchor is TextAnchor.MiddleRight
                || _anchor is TextAnchor.LowerRight)
            {
                horizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                horizontalAlignment = HorizontalAlignment.Center;
            }

            VerticalAlignment verticalAlignment;

            if (_anchor is TextAnchor.UpperLeft
                || _anchor is TextAnchor.UpperCenter
                || _anchor is TextAnchor.UpperRight)
            {
                verticalAlignment = VerticalAlignment.Top;
            }
            else if (_anchor is TextAnchor.LowerLeft
                || _anchor is TextAnchor.LowerCenter
                || _anchor is TextAnchor.LowerRight)
            {
                verticalAlignment = VerticalAlignment.Bottom;
            }
            else
            {
                verticalAlignment = VerticalAlignment.Center;
            }

            return RenderSize.AlignToArea(availableRect,
                horizontalAlignment, verticalAlignment);
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            TextAnchor currentAnchor = GameText.Anchor;
            GameFont currentFont = GameText.Font;

            GameText.Anchor = TextAnchor.MiddleCenter;
            GameText.Font = FontSize;

            Widgets.Label(RenderRect, Text);

            GameText.Anchor = currentAnchor;
            GameText.Font = currentFont;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize) => Text.CalculateLineSize(FontSize);

        #endregion
    }
}
