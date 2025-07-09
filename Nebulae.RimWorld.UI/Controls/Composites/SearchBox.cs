using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites.Components;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls.Composites
{
    /// <summary>
    /// <see cref="SearchBox.Search"/> 事件的处理程序
    /// </summary>
    /// <param name="sender">发起事件的 <see cref="SearchBox"/></param>
    /// <param name="args">事件数据</param>
    public delegate void SearchEventHandler(SearchBox sender, EventArgs args);


    /// <summary>
    /// 搜索框控件
    /// </summary>
    public sealed class SearchBox : InputControl
    {
        #region Search
        private readonly WeakEvent<SearchBox, EventArgs> _searchEvent = new WeakEvent<SearchBox, EventArgs>();

        /// <summary>
        /// 当 <see cref="SearchBox"/> 的搜索条件发生变化时发生
        /// </summary>
        public event SearchEventHandler Search
        {
            add { _searchEvent.AddHandler(value); }
            remove { _searchEvent.RemoveHandler(value); }
        }
        #endregion


        /// <inheritdoc/>
        public override IEnumerable<Control> LogicalChildren
        {
            get
            {
                yield return _clearButton;
            }
        }


        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region AutoHeight
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="SearchBox"/> 是否根据内容调整高度
        /// </summary>
        public bool AutoHeight
        {
            get { return (bool)GetValue(AutoHeightProperty); }
            set { SetValue(AutoHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="AutoHeight"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty AutoHeightProperty =
            DependencyProperty.Register(nameof(AutoHeight), typeof(bool), typeof(SearchBox),
                new ControlPropertyMetadata(false, ControlRelation.Measure));
        #endregion

        #region FontSize
        /// <summary>
        /// 获取或设置 <see cref="SearchBox"/> 的字体大小
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
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(SearchBox),
                new ControlPropertyMetadata(GameFont.Small, TextUtility.CoerceFontSize, UpdateDrawer, ControlRelation.Measure));
        #endregion

        #region InputValidator
        /// <summary>
        /// 获取或设置 <see cref="SearchBox"/> 验证输入文字的正则表达式
        /// </summary>
        public Regex InputValidator
        {
            get { return (Regex)GetValue(InputValidatorProperty); }
            set { SetValue(InputValidatorProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="InputValidator"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty InputValidatorProperty =
            DependencyProperty.Register(nameof(InputValidator), typeof(Regex), typeof(SearchBox),
                new ControlPropertyMetadata(null, OnInputValidatorChanged, ControlRelation.Arrange));

        private static void OnInputValidatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)d;

            if (e.NewValue is Regex regex && !regex.IsMatch(searchBox.Text))
            {
                searchBox.SetValue(TextProperty, string.Empty);
            }

            UpdateDrawer(d, e);
        }
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置 <see cref="SearchBox"/> 的文本
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
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBox),
                new ControlPropertyMetadata(string.Empty, OnTextChanged, ControlRelation.Measure));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)d;
            var text = (string)e.NewValue;

            searchBox._cache = text;
            searchBox._filter.Text = text;
            searchBox._searchEvent.Invoke(searchBox, EventArgs.Empty);
        }
        #endregion

        #endregion


        static SearchBox()
        {
            HeightProperty.OverrideMetadata(typeof(SearchBox),
                new ControlPropertyMetadata(34f, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="SearchBox"/> 的新实例
        /// </summary>
        public SearchBox()
        {
            _clearButton = new SolidButton { Icon = TexButton.CloseXSmall };
            _clearButton.Click += OnClearButtonClick;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断文本是否匹配搜索条件
        /// </summary>
        /// <param name="text">要判断的文本</param>
        /// <returns>若 <paramref name="text"/> 符合搜索条件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Matches(string text)
        {
            return !_filter.Active || _filter.Matches(text);
        }

        /// <summary>
        /// 判断 <see cref="ThingDef"/> 的标签是否匹配搜索条件
        /// </summary>
        /// <param name="def">要判断的 <see cref="ThingDef"/></param>
        /// <returns>若 <paramref name="def"/> 的标签符合搜索条件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Matches(ThingDef def)
        {
            return !Find.HiddenItemsManager.Hidden(def)
                && (!_filter.Active || _filter.Matches(def.label));
        }

        /// <summary>
        /// 判断 <see cref="SpecialThingFilterDef"/> 的标签是否匹配搜索条件
        /// </summary>
        /// <param name="def">要判断的 <see cref="SpecialThingFilterDef"/></param>
        /// <returns>若 <paramref name="def"/> 的标签符合搜索条件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Matches(SpecialThingFilterDef def)
        {
            return !_filter.Active || _filter.Matches(def.label);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeOverride(Rect availableRect)
        {
            var x = availableRect.x;
            var y = availableRect.y;

            _clearButton.Arrange(
                _clearButton.DesiredSize.AlignToArea(new Rect(x + RenderSize.Width - RenderSize.Height, y, RenderSize.Height, RenderSize.Height),
                    HorizontalAlignment.Center, VerticalAlignment.Center));

            _searchBoxRect = new Rect(x + RenderSize.Height, y, _clearButton.DesiredSize.IsEmpty ? RenderSize.Width - RenderSize.Height : RenderSize.Width - (RenderSize.Height + RenderSize.Height), RenderSize.Height);
            _searchIconRect = new Size(RenderSize.Height * 0.9f).AlignToArea(new Rect(x, y, RenderSize.Height, RenderSize.Height),
                HorizontalAlignment.Center, VerticalAlignment.Center);

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if ((bool)GetValue(AutoHeightProperty))
            {
                var height = _cache.CalculateTextBoxHeight(
                    availableSize.Width,
                    (GameFont)GetValue(FontSizeProperty),
                    false);

                availableSize = new Size(availableSize.Width, Mathf.Clamp(height, (float)GetValue(MinHeightProperty), (float)GetValue(MaxHeightProperty)));
            }

            if (string.IsNullOrEmpty(_cache) || ControlStates.HasState(ControlState.Disabled))
            {
                _clearButton.Measure(Size.Empty);
            }
            else
            {
                _clearButton.Measure(new Size(availableSize.Height * 0.85f));
            }

            return availableSize;
        }

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            _clearButton.Segment(visiableRect);

            return new SegmentResult(visiableRect.IntersectWith(_searchBoxRect), visiableRect.IntersectWith(RenderRect));
        }

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(Vector2 hitPoint)
        {
            var result = _clearButton.HitTest(hitPoint);

            if (result.IsHit)
            {
                return result;
            }

            return HitTestResult.HitTest(this, hitPoint);
        }

        /// <inheritdoc/>
        protected override void DrawControl(ControlState states)
        {
            _drawer(this, states);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static Action<SearchBox, ControlState> CreateDrawer(string text, GameFont fontSize, Regex inputValidator)
        {
            var drawClearButton = !string.IsNullOrEmpty(text);
            var shouldValidate = inputValidator != null;

            void Draw(SearchBox searchBox, ControlState states)
            {
                if (states.HasState(ControlState.Disabled))
                {
                    GUI.color *= Widgets.InactiveColor;

                    searchBox._cache.DrawTextBox(searchBox._searchBoxRect, fontSize, false, false);
                }
                else
                {
                    var buffer = searchBox._cache.DrawTextBox(searchBox._searchBoxRect, fontSize, states.HasState(ControlState.Focused | ControlState.CursorDirectlyOver | ControlState.Pressing), false);

                    if (!buffer.Equals(searchBox._cache) && (!shouldValidate || inputValidator.IsMatch(buffer)))
                    {
                        drawClearButton = !string.IsNullOrEmpty(buffer);
                        searchBox.SetValue(TextProperty, buffer);
                    }

                    if (drawClearButton)
                    {
                        searchBox._clearButton.Draw();
                    }
                }

                GUI.DrawTexture(searchBox._searchIconRect, TexButton.Search, ScaleMode.ScaleToFit);
            }

            return Draw;
        }

        private static void UpdateDrawer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)d;
            searchBox._drawer = CreateDrawer(searchBox._cache,
                (GameFont)d.GetValue(FontSizeProperty),
                (Regex)d.GetValue(InputValidatorProperty));
        }

        #endregion


        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            SetValue(TextProperty, string.Empty);
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            e.Handled = true;
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly Action<SearchBox, ControlState> DefaultDrawer = CreateDrawer(string.Empty, GameFont.Small, null);

        private readonly SolidButton _clearButton;
        private readonly QuickSearchFilter _filter = new QuickSearchFilter();

        private string _cache = string.Empty;
        private Action<SearchBox, ControlState> _drawer = DefaultDrawer;

        private Rect _searchBoxRect;
        private Rect _searchIconRect;

        #endregion
    }
}
