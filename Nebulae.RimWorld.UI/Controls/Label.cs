using Nebulae.RimWorld.UI.Data;
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
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

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


        /// <inheritdoc/>
        protected override Rect DrawCore(Rect renderRect)
        {
            TextAnchor currentAnchor = GameText.Anchor;
            GameFont currentFont = GameText.Font;

            GameText.Anchor = TextAnchor.MiddleCenter;
            GameText.Font = FontSize;
            Widgets.Label(renderRect, Text);
            GameText.Anchor = currentAnchor;
            GameText.Font = currentFont;

            return renderRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize) => Text.CalculateLineSize(FontSize);
    }
}
