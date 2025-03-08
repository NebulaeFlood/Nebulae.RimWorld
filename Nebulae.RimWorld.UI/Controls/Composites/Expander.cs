using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls.Composites
{
    /// <summary>
    /// 拓展器控件
    /// </summary>
    public class Expander : Visual
    {
        /// <summary>
        /// 拓展内容左填充边距
        /// </summary>
        public const float ContentLeftPadding = 20f;

        /// <summary>
        /// 标题高度
        /// </summary>
        public const float HeadlineHight = 20f;

        /// <summary>
        /// 标签的左填充边距
        /// </summary>
        public const float LabelLeftPadding = 20f;

        /// <summary>
        /// 拓展内容为空时标签的左填充边距
        /// </summary>
        public const float LabelLeftPaddingWhenEmpty = 6f;


        #region Click

        /// <summary>
        /// 当标题被点击时发生的弱事件
        /// </summary>
        public event Action<Control, EventArgs> Click
        {
            add => _label.Click += value;
            remove => _label.Click -= value;
        }

        #endregion

        #region Collapsed 

        private readonly WeakEvent<Expander, Visual> _collapsed = new WeakEvent<Expander, Visual>();

        /// <summary>
        /// 收起拓展内容时发生的弱事件
        /// </summary>
        public event Action<Expander, Visual> Collapsed
        {
            add => _collapsed.Add(value, value.Invoke);
            remove => _collapsed.Remove(value);
        }

        #endregion

        #region Expanded 

        private readonly WeakEvent<Expander, Visual> _expanded = new WeakEvent<Expander, Visual>();

        /// <summary>
        /// 展开拓展内容时发生的弱事件
        /// </summary>
        public event Action<Expander, Visual> Expanded
        {
            add => _expanded.Add(value, value.Invoke);
            remove => _expanded.Remove(value);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly ExpandButton _expandButton;
        private readonly Label _label;

        private Visual _content;

        private bool _isEmpty = true;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 要展示的拓展内容
        /// </summary>
        public Visual Content
        {
            get => _content;
            set
            {
                if (!ReferenceEquals(value, _content))
                {
                    if (!_isEmpty)
                    {
                        _content.SetParent(null);
                    }

                    _content = value; ;
                    _isEmpty = value is null;

                    if (_isEmpty)
                    {
                        return;
                    }

                    _content.SetParent(this);

                    InvalidateMeasure();
                }
            }
        }

        #region IsExpanded
        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsExpanded"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Expander),
                new ControlPropertyMetadata(false, OnIsExpandedChanged, ControlRelation.Measure));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = (Expander)d;
            bool isExpanded = (bool)e.NewValue;

            if (isExpanded)
            {
                expander._expandButton.IsExpanded = true;
                SoundDefOf.TabOpen.PlayOneShotOnCamera();
                expander._collapsed.Invoke(expander, expander._content);
            }
            else
            {
                expander._expandButton.IsExpanded = false;
                SoundDefOf.TabClose.PlayOneShotOnCamera();
                expander._collapsed.Invoke(expander, expander._content);
            }
        }
        #endregion

        /// <summary>
        /// 标题文本
        /// </summary>
        public string Title
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        /// <summary>
        /// 标题文本是否能够进行交互
        /// </summary>
        public bool TitleHitTestVisible
        {
            get => _label.IsHitTestVisible;
            set => _label.IsHitTestVisible = value;
        }

        /// <summary>
        /// 标题文本的提示框内容
        /// </summary>
        public TipSignal Tooltip
        {
            get => _label.Tooltip;
            set => _label.Tooltip = value;
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="Expander"/> 的新实例
        /// </summary>
        public Expander()
        {
            _expandButton = new ExpandButton(this);
            _label = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                DrawHighlight = true
            };
            _label.SetParentSilently(this);
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
            float headlineHight = GameFont.Small.GetHeight();

            Rect renderRect;

            if (_isEmpty)
            {
                renderRect = new Rect(
                    availableRect.x + LabelLeftPaddingWhenEmpty,
                    availableRect.y,
                    availableRect.width - LabelLeftPaddingWhenEmpty,
                    headlineHight);

                _expandButton.Arrange(Rect.zero);
                _label.Arrange(renderRect);

                return renderRect;
            }

            renderRect = new Rect(availableRect.x, availableRect.y, availableRect.width, headlineHight);

            _expandButton.Arrange(renderRect);
            _label.Arrange(new Rect(renderRect.x + ContentLeftPadding, renderRect.y, renderRect.width - ContentLeftPadding, headlineHight));

            if (_expandButton.IsExpanded)
            {
                renderRect.height += _content.Arrange(new Rect(
                    renderRect.x + ContentLeftPadding,
                    renderRect.y + headlineHight,
                    renderRect.width - ContentLeftPadding,
                    float.PositiveInfinity)).height;
            }
            else
            {
                _content.Arrange(new Rect(renderRect.x, renderRect.y, 0f, 0f));
            }

            return renderRect;
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            if (_isEmpty)
            {
                _label.Draw();
            }
            else
            {
                _expandButton.Draw();
                _label.Draw();
                _content.Draw();
            }
        }

        /// <inheritdoc/>
        protected internal override IEnumerable<Visual> EnumerateLogicalChildren()
        {
            yield return _expandButton;
            yield return _label;

            if (_isEmpty)
            {
                yield break;
            }

            yield return _content;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            float headlineHight = GameFont.Small.GetHeight();

            _expandButton.Measure(availableSize);

            if (_isEmpty || !_expandButton.IsExpanded)
            {
                _label.Measure(_isEmpty
                    ? new Size(availableSize.Width - LabelLeftPaddingWhenEmpty, headlineHight)
                    : new Size(availableSize.Width - LabelLeftPadding, headlineHight));

                return new Size(availableSize.Width, headlineHight);
            }

            _label.Measure(new Size(availableSize.Width - LabelLeftPadding, headlineHight));

            return new Size(
                availableSize.Width,
                _content.Measure(new Size(
                    availableSize.Width - ContentLeftPadding,
                    float.PositiveInfinity))
                .Height + headlineHight);
        }

        /// <inheritdoc/>
        protected override void OnIsEnabledChanged(bool isEnabled)
        {
            if (!isEnabled)
            {
                IsExpanded = false;
            }
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            Rect contentRect = RenderRect.IntersectWith(visiableRect);

            if (_isEmpty)
            {
                _expandButton.Segment(Rect.zero);
            }
            else
            {
                _content.Segment(_expandButton.IsExpanded ? contentRect : Rect.zero);
                _expandButton.Segment(contentRect);
            }

            _label.Segment(contentRect);

            return contentRect;
        }

        #endregion
    }
}
