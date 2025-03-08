using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 标签控件
    /// </summary>
    public class Label : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private TextAnchor _anchor = TextAnchor.MiddleCenter;

        private string _buffer = string.Empty;

        private SoundDef _clickSound = SoundDefOf.Tick_Tiny;

        private bool _drawHighlight;
        private bool _onlyTextHitTestVisible;

        private Rect _labelRect;
        private Rect _textRect;

        #endregion


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

        /// <summary>
        /// 点击标签时的音效
        /// </summary>
        public SoundDef ClickSound
        {
            get => _clickSound;
            set => _clickSound = value;
        }

        /// <summary>
        /// 是否绘制高亮效果
        /// </summary>
        /// <remarks>仅当 <see cref="Visual.IsHitTestVisible"/> 为 <see langword="true"/> 时有效。</remarks>
        public bool DrawHighlight
        {
            get => _drawHighlight;
            set => _drawHighlight = value;
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
                new ControlPropertyMetadata(GameFont.Small, ControlRelation.Arrange));
        #endregion

        /// <summary>
        /// 是否只有文字区域可交互
        /// </summary>
        public bool OnlyTextHitTestVisible
        {
            get => _onlyTextHitTestVisible;
            set => _onlyTextHitTestVisible = value;
        }

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
                new ControlPropertyMetadata(string.Empty, ControlRelation.Arrange));
        #endregion

        #endregion


        static Label()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(Label),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));
            VerticalAlignmentProperty.OverrideMetadata(typeof(Label),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="Label"/> 的新实例
        /// </summary>
        public Label()
        {
        }


        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect renderRect = RenderSize.AlignToArea(availableRect,
                HorizontalAlignment, VerticalAlignment);

            _buffer = Text.Truncate(RenderSize.Width);
            _textRect = _buffer.CalculateLineSize(FontSize).AlignToArea(renderRect, _anchor);
            _labelRect = new Rect(renderRect.x, _textRect.y, renderRect.width, _textRect.height);

            return renderRect;
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            if (_drawHighlight && IsCursorOver)
            {
                GUI.DrawTexture(
                    _onlyTextHitTestVisible ? _textRect : _labelRect,
                    TexUI.HighlightTex);
            }

            if (!IsEnabled)
            {
                GUI.color *= Widgets.InactiveColor;
            }

            TextAnchor currentAnchor = GameText.Anchor;
            GameFont currentFont = GameText.Font;

            GameText.Anchor = _anchor;
            GameText.Font = FontSize;

            Widgets.Label(RenderRect, _buffer);

            GameText.Anchor = currentAnchor;
            GameText.Font = currentFont;
        }

        /// <inheritdoc/>
        protected override Rect AnalyseCore(Rect contentRect)
        {
            return contentRect.IntersectWith(_onlyTextHitTestVisible
                ? _textRect
                : _labelRect);
        }

        /// <inheritdoc/>
        protected internal override void OnClick()
        {
            _clickSound?.PlayOneShotOnCamera();
        }
    }
}
