using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.Utilities;
using UnityEngine;
using Verse;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 文本块控件
    /// </summary>
    public class TextBlock : FrameworkControl
    {
        private TextAnchor _anchor = TextAnchor.UpperLeft;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置文字在控件内显示的方位
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
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(TextBlock),
                new ControlPropertyMetadata(GameFont.Small, ControlRelation.Measure));
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置控件文本
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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBlock),
                new ControlPropertyMetadata(string.Empty, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="TextBlock"/> 的新实例
        /// </summary>
        public TextBlock()
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
            Rect renderRect = base.ArrangeCore(availableRect);
            renderRect.height = Mathf.Max(renderRect.height, RenderSize.Height);
            return renderRect;
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            TextAnchor currentAnchor = GameText.Anchor;
            GameFont currentFont = GameText.Font;

            GameText.Anchor = _anchor;
            GameText.Font = FontSize;

            Widgets.Label(RenderRect, Text);

            GameText.Anchor = currentAnchor;
            GameText.Font = currentFont;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            return new Size(
                availableSize.Width,
                Text.CalculateHeight(availableSize.Width, FontSize));
        }

        #endregion
    }
}
