using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites.Components;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls.Composites
{
    /// <summary>
    /// 拓展器控件
    /// </summary>
    public sealed class Expander : Control
    {
        /// <summary>
        /// <see cref="Expander"/> 的按钮尺寸
        /// </summary>
        public const float ExpandButtonSize = 18f;

        /// <summary>
        /// <see cref="Expander"/> 的标题高度
        /// </summary>
        public const float HeaderHeight = 24f;


        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------

        #region Public Events

        #region Click
        /// <summary>
        /// 当 <see cref="Expander"/> 的标题被点击时发生
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ButtonBase.ClickEvent, value); }
            remove { RemoveHandler(ButtonBase.ClickEvent, value); }
        }
        #endregion

        #region Collapsed
        /// <summary>
        /// 当 <see cref="Expander"/> 收起 <see cref="Content"/> 时发生
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="Collapsed"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent =
            RoutedEvent.Register(nameof(Collapsed), RoutingStrategy.Direct, typeof(Expander), typeof(RoutedEventArgs));
        #endregion

        #region Expanded
        /// <summary>
        /// 当 <see cref="Expander"/> 展开 <see cref="Content"/> 时发生
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="Expanded"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent =
            RoutedEvent.Register(nameof(Expanded), RoutingStrategy.Direct, typeof(Expander), typeof(RoutedEventArgs));
        #endregion

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置 <see cref="Click"/> 触发时播放的音效
        /// </summary>
        public SoundDef ClickSound
        {
            get => _labelButton.ClickSound;
            set => _labelButton.ClickSound = value;
        }

        /// <summary>
        /// 获取或设置 <see cref="Expander"/> 的内容控件
        /// </summary>
        public Control Content
        {
            get => _content;
            set
            {
                if (!ReferenceEquals(_content, value))
                {
                    if (!_isEmpty)
                    {
                        _content.Parent = null;
                    }

                    _content = value;
                    _isEmpty = value is null;

                    if (!_isEmpty)
                    {
                        _content.Parent = this;
                        _content.Visibility = IsExpanded ? Visibility.Visible : Visibility.Collapsed;
                        _expandButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _expandButton.Visibility = Visibility.Collapsed;
                    }

                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 获取一个值，指示 <see cref="Expander"/> 是否没有内容控件
        /// </summary>
        public bool IsEmpty => _isEmpty;

        /// <inheritdoc/>
        public override IEnumerable<Control> LogicalChildren
        {
            get
            {
                yield return _expandButton;
                yield return _labelButton;

                if (!_isEmpty)
                {
                    yield return _content;
                }
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region Header
        /// <summary>
        /// 获取或设置 <see cref="Expander"/> 的标题内容
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Header"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(Expander),
                new PropertyMetadata(string.Empty, OnHeaderChanged));

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Expander)d)._labelButton.Text = (string)e.NewValue;
        }
        #endregion

        #region IsExpanded
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="Expander"/> 是否处于展开状态
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsExpanded"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Expander),
                new ControlPropertyMetadata(false, OnIsExpandedChanged, ControlRelation.Measure));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = (Expander)d;
            var isExpanded = (bool)e.NewValue;

            expander._expandButton.Icon = isExpanded ? TexButton.Collapse : TexButton.Reveal;

            if (!expander._isEmpty)
            {
                expander._content.Visibility = isExpanded ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="Expander"/> 的新实例
        /// </summary>
        public Expander()
        {
            _expandButton = new SolidButton
            {
                Icon = TexButton.Reveal,
                Parent = this
            };
            _expandButton.Click += OnExpandButtonClick;

            _labelButton = new LabelButton
            {
                Anchor = TextAnchor.MiddleLeft,
                Text = Header,
                Parent = this
            };
            _labelButton.Click += OnLabelButtonClick;
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
            if (_isEmpty)
            {
                return _labelButton.Arrange(new Rect(
                    availableRect.x + 6f,
                    availableRect.y,
                    _labelButton.DesiredSize.Width,
                    _labelButton.DesiredSize.Height));
            }

            _expandButton.Arrange(_expandButton.DesiredSize.AlignToArea(new Rect(availableRect.x, availableRect.y, DesiredSize.Width, HeaderHeight),
                HorizontalAlignment.Left, VerticalAlignment.Center));

            _labelButton.Arrange(new Rect(availableRect.x + ExpandButtonSize, availableRect.y, _labelButton.DesiredSize.Width, HeaderHeight));

            if (IsExpanded)
            {
                _content.Arrange(new Rect(
                    availableRect.x + ExpandButtonSize,
                    availableRect.y + HeaderHeight,
                    _content.DesiredSize.Width,
                    _content.DesiredSize.Height));
            }

            return new Rect(availableRect.x, availableRect.y, DesiredSize.Width, DesiredSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            if (_isEmpty)
            {
                _expandButton.Measure(Size.Empty);
                _labelButton.Measure(new Size(availableSize.Width - 6f, HeaderHeight));

                return new Size(availableSize.Width, HeaderHeight);
            }

            _expandButton.Measure(new Size(ExpandButtonSize));

            var hearderWidth = availableSize.Width - ExpandButtonSize;

            _labelButton.Measure(new Size(hearderWidth, HeaderHeight));

            var desiredHeight = IsExpanded
                ? Content.Measure(new Size(hearderWidth, float.NaN)).Height + HeaderHeight
                : HeaderHeight;

            return new Size(availableSize.Width, desiredHeight);
        }

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            visiableRect = visiableRect.IntersectWith(DesiredRect);

            _expandButton.Segment(visiableRect);
            _labelButton.Segment(visiableRect);

            if (!_isEmpty)
            {
                Content.Segment(visiableRect);
            }

            return new SegmentResult(visiableRect, _labelButton.ControlRect, visiableRect);
        }

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(Vector2 hitPoint)
        {
            var result = HitTestResult.HitTest(this, hitPoint);

            if (!result.IsHit)
            {
                return result;
            }

            var childResult = _labelButton.HitTest(hitPoint);

            if (childResult.IsHit)
            {
                return childResult;
            }

            if (_isEmpty)
            {
                return result;
            }

            childResult = _expandButton.HitTest(hitPoint);

            if (childResult.IsHit)
            {
                return childResult;
            }

            childResult = _content.HitTest(hitPoint);

            if (childResult.IsHit)
            {
                return childResult;
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            _expandButton.Draw();
            _labelButton.Draw();

            if (!_isEmpty)
            {
                _content.Draw();
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private void OnExpandButtonClick(object sender, RoutedEventArgs e)
        {
            var isExpanded = IsExpanded;

            if (isExpanded)
            {
                IsExpanded = false;

                _expandButton.Icon = TexButton.Reveal;

                SoundDefOf.TabClose.PlayOneShotOnCamera();
                RaiseEvent(new RoutedEventArgs(this, CollapsedEvent));
            }
            else
            {
                IsExpanded = true;

                _expandButton.Icon = TexButton.Collapse;

                SoundDefOf.TabOpen.PlayOneShotOnCamera();
                RaiseEvent(new RoutedEventArgs(this, ExpandedEvent));
            }

            e.Handled = true;
        }

        private void OnLabelButtonClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Control _content;
        private bool _isEmpty = true;

        private readonly SolidButton _expandButton;
        private readonly LabelButton _labelButton;

        #endregion
    }
}
