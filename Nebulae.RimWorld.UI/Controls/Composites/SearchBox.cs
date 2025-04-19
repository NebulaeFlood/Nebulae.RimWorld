using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls.Composites
{
    /// <summary>
    /// 搜索框控件
    /// </summary>
    public class SearchBox : FocusableControl
    {
        /// <summary>
        /// 当文本框的文本发生变化时发生
        /// </summary>
        public event Action<SearchBox, string> OnTextChanged;


        /// <summary>
        /// 搜索框的图标与其他内容之间的间距
        /// </summary>
        public const float IconMargin = 4f;


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
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(SearchBox),
                new ControlPropertyMetadata(GameFont.Small, TextUtility.CoerceFontSize, ControlRelation.Arrange));
        #endregion

        #region IconSize
        /// <summary>
        /// 获取或设置图标尺寸
        /// </summary>
        public Size IconSize
        {
            get { return (Size)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IconSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(Size), typeof(SearchBox),
                new ControlPropertyMetadata(new Size(16f), ControlRelation.Measure));
        #endregion

        #region InputValidator
        /// <summary>
        /// 获取或设置验证输入文字的正则表达式
        /// </summary>
        public Regex InputValidator
        {
            get { return (Regex)GetValue(InputValidatorProperty); }
            set { SetValue(InputValidatorProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="InputValidator"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty InputValidatorProperty =
            DependencyProperty.Register(nameof(InputValidator), typeof(Regex), typeof(SearchBox),
                new ControlPropertyMetadata(null, OnInputValidatorChanged, ControlRelation.Arrange));

        private static void OnInputValidatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBox)d).Text = string.Empty;
        }
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置搜索框的文本
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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBox),
                new PropertyMetadata(string.Empty, OnTextTextChanged));

        private static void OnTextTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox searchBox = (SearchBox)d;
            string text = (string)e.NewValue;

            searchBox._filter.Text = text;
            searchBox.OnTextChanged?.Invoke(searchBox, text);
        }
        #endregion


        /// <summary>
        /// 搜索框的背景拓展量
        /// </summary>
        /// <remarks>只有 <see cref="Thickness.Top"/> 和 <see cref="Thickness.Bottom"/> 有效。</remarks>
        public Thickness TextBoxExtension
        {
            get => _textBoxExtension;
            set
            {
                value = value.Normalize();
                value = new Thickness(0f, value.Top, 0f, value.Bottom);

                if (_textBoxExtension != value)
                {
                    _textBoxExtension = value;

                    InvalidateMeasure();
                }
            }
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="SearchBox"/> 的新实例
        /// </summary>
        public SearchBox()
        {
            _clearButton = new ClearButton();
            _filter = new QuickSearchFilter();

            _clearButton.SetParentSilently(this);
        }


        /// <summary>
        /// 判断文本是否匹配搜索条件
        /// </summary>
        /// <param name="text">要判断的文本</param>
        /// <returns>若 <paramref name="text"/> 符合搜索条件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Matches(string text)
        {
            return !_filter.Active || _filter.Matches(text);
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect AnalyseCore(Rect contentRect)
        {
            return _innerRect.IntersectWith(contentRect);
        }

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect renderRect = base.ArrangeCore(availableRect);

            Size iconSize = IconSize;

            _searchIconRect = iconSize.AlignToArea(renderRect, HorizontalAlignment.Left, VerticalAlignment.Center);
            _innerRect = _innerSize.AlignToArea(renderRect, HorizontalAlignment.Right, VerticalAlignment.Center);

            _clearButton.Arrange(renderRect);

            return renderRect;
        }

        /// <inheritdoc/>
        protected override void DrawControl(bool isFocused)
        {
            GameFont controlFont = FontSize;
            GameFont font = GameText.Font;
            GameText.Font = controlFont;

            bool isDisabled = !IsEnabled;

            if (isDisabled)
            {
                GUI.color *= Widgets.InactiveColor;
            }

            GUI.DrawTexture(_searchIconRect, TexButton.Search, ScaleMode.ScaleToFit);

            if (isDisabled)
            {
                Text.DrawTextBox(_innerRect, FontSize, false, false);
            }
            else
            {
                string text = Text;
                string buffer = text.DrawTextBox(_innerRect, FontSize, isFocused || IsCursorOver || IsPressing, false);

                if (text != buffer && (InputValidator is null || InputValidator.IsMatch(buffer)))
                {
                    text = buffer;
                    Text = buffer;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    _clearButton.Draw();
                }
            }

            GameText.Font = font;
        }

        /// <inheritdoc/>
        protected internal override IEnumerable<Visual> EnumerateLogicalChildren()
        {
            yield return _clearButton;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size renderSize = base.MeasureCore(availableSize);

            float fontHeight = FontSize.GetHeight();

            _innerSize = new Size(renderSize.Width - IconSize.Width - IconMargin,
                fontHeight + _textBoxExtension.Top + _textBoxExtension.Bottom);

            if (_innerSize.Height > renderSize.Height)
            {
                renderSize = new Size(renderSize.Width,
                    _innerSize.Height + Mathf.Max(0f, AutoLayoutUtility.StandardRowHeight - (_textBoxExtension.Top + fontHeight + _textBoxExtension.Bottom)));
            }

            _clearButton.Measure(new Size(fontHeight));

            return renderSize;
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            _clearButton.Segment(visiableRect);
            return visiableRect;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly QuickSearchFilter _filter;
        private readonly ClearButton _clearButton;

        private Thickness _textBoxExtension = Thickness.Empty;

        private Rect _searchIconRect;

        private Rect _innerRect;
        private Size _innerSize;

        #endregion


        private sealed class ClearButton : ButtonBase
        {
            static ClearButton()
            {
                HorizontalAlignmentProperty.OverrideMetadata(typeof(ClearButton),
                    new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));
                VerticalAlignmentProperty.OverrideMetadata(typeof(ClearButton),
                    new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
            }

            public ClearButton()
            {
                ClickSound = SoundDefOf.CancelMode;
            }


            protected override Rect ArrangeCore(Rect availableRect)
            {
                Rect renderRect = RenderSize.AlignToArea(availableRect, HorizontalAlignment.Right, VerticalAlignment.Center);
                renderRect.x -= 4f;
                return renderRect;
            }

            protected override void DrawButton(ButtonStatus status)
            {
                if (status.HasFlag(ButtonStatus.Hovered))
                {
                    GUI.color *= GenUI.MouseoverColor;
                }

                GUI.DrawTexture(RenderRect, TexButton.CloseXSmall);
            }

            protected internal override void OnClick()
            {
                ((SearchBox)Parent).Text = string.Empty;
                base.OnClick();
            }
        }
    }
}
