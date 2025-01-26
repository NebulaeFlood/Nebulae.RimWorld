using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.UI.Windows;
using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 所有控件的基类，定义了控件的共同特性
    /// </summary>
    public abstract class Control : DependencyObject
    {
        //------------------------------------------------------
        //
        //  Public Static Fields
        //
        //------------------------------------------------------

        #region Public Static Fields

        /// <summary>
        /// 点击控件是否强制重新排布控件
        /// </summary>
        public static bool ClickForceLayout = false;

        /// <summary>
        /// 是否绘制控件绘制区域
        /// </summary>
        public static bool DrawRenderRect = false;

        /// <summary>
        /// 是否绘制控件布局区域
        /// </summary>
        public static bool DrawLayoutRect = false;

        /// <summary>
        ///是否在信息窗口显示控件信息
        /// </summary>
        public static bool ShowInfo = false;

        #endregion


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 控件控制呈现的可见区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Segment(Rect)"/>。</remarks>
        public Rect ContentRect;

        /// <summary>
        /// 控件控制呈现的可见尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Segment(Rect)"/>。</remarks>
        public Size ContentSize;

        /// <summary>
        /// 控件需要占用的布局区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Arrange(Rect)"/>。</remarks>
        public Rect DesiredRect;

        /// <summary>
        /// 控件需要占用的布局尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Measure(Size)"/>。</remarks>
        public Size DesiredSize = Size.Empty;

        /// <summary>
        /// 计算的将要绘制的区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Arrange(Rect)"/>。</remarks>
        public Rect RenderRect;

        /// <summary>
        /// 计算的将要绘制的尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Measure(Size)"/>。</remarks>
        public Size RenderSize = Size.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        /// <summary>
        /// 控件是否为另一个控件的子控件
        /// </summary>
        internal bool IsChild = false;

        /// <summary>
        /// 拥有该控件的窗口
        /// </summary>
        internal ControlWindow Owner;

        /// <summary>
        /// 控件的父控件
        /// </summary>
        internal Control Parent;

        /// <summary>
        /// 控件在逻辑树中的层次
        /// </summary>
        internal int Rank = 0;

        #endregion


        //------------------------------------------------------
        //
        //  Privaet Fields
        //
        //------------------------------------------------------

        #region Privaet Fields

        private string _name = string.Empty;

        private bool _isArrangeValid = false;

        private bool _isIndependent = false;

        private bool _isMeasureValid = false;
        private bool _isSegmentValid = false;

        private bool _shouldShowTooltip = false;
        private bool _showTooltip = false;
        private TipSignal _tooltip = string.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Pubilc Properties
        //
        //------------------------------------------------------

        #region Pubilc Properties

        /// <summary>
        /// 控件布局是否有效
        /// </summary>
        public bool IsArrangeValid
        {
            get
            {
                if (!_isArrangeValid)
                {
                    return false;
                }

                if (Owner.LayoutManager.IsArrangeValid(this))
                {
                    return true;
                }

                _isArrangeValid = false;

                return false;
            }
        }

        /// <summary>
        /// 控件是否独立于 <see cref="ControlWindow"/>
        /// </summary>
        public bool IsIndependent
        {
            get => _isIndependent;
            set => _isIndependent = value;
        }

        /// <summary>
        /// 控件度量是否有效
        /// </summary>
        public bool IsMeasureValid
        {
            get
            {
                if (!_isMeasureValid)
                {
                    return false;
                }

                if (Owner.LayoutManager.IsMeasureValid(this))
                {
                    return true;
                }

                _isMeasureValid = false;

                return false;
            }
        }

        /// <summary>
        /// 控件分割是否有效
        /// </summary>
        public bool IsSegmentValid
        {
            get
            {
                if (!_isSegmentValid)
                {
                    return false;
                }

                if (Owner.LayoutManager.IsSegmentValid(this))
                {
                    return true;
                }

                _isSegmentValid = false;

                return false;
            }
        }

        #region Margin
        /// <summary>
        /// 获取或设置控件的外部边距
        /// </summary>
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Margin"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(nameof(Margin), typeof(Thickness), typeof(Control),
                new ControlPropertyMetadata(Thickness.Empty, ControlRelation.Measure));
        #endregion

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// 光标悬浮于控件上时是否显示提示框
        /// </summary>
        public bool ShowTooltip
        {
            get => _showTooltip;
            set => _showTooltip = value;
        }

        /// <summary>
        /// 提示框文字
        /// </summary>
        public TipSignal Tooltip
        {
            get => _tooltip;
            set
            {
                _tooltip = value;
                _shouldShowTooltip = !string.IsNullOrEmpty(value.text)
                    || value.textGetter != null;
            }
        }

        #region Visibility
        /// <summary>
        /// 获取或设置控件的显示状态
        /// </summary>
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Visibility"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register(nameof(Visibility), typeof(Visibility), typeof(Control),
                new PropertyMetadata(Visibility.Visible));
        #endregion

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 计算控件需要占用的布局区域
        /// </summary>
        /// <param name="availableRect">分配给控件的区域</param>
        /// <returns>控件需要占用的布局区域。</returns>
        /// <remarks>需要保证调用过 <see cref="Measure(Size)"/>。</remarks>
        public Rect Arrange(Rect availableRect)
        {
            if (!(Visibility is Visibility.Collapsed))
            {
                Thickness margin = Margin;
                RenderRect = ArrangeCore(availableRect - margin).Rounded();
                DesiredRect = RenderRect + margin;
            }
            else
            {
                RenderRect = new Rect(availableRect.x, availableRect.y, 0f, 0f);
                DesiredRect = RenderRect;
            }

            _isArrangeValid = true;

            return DesiredRect;
        }

        /// <summary>
        /// 使用当前计算的布局信息绘制控件
        /// </summary>
        public void Draw()
        {
            if (RenderSize.Width <= float.Epsilon
                || RenderSize.Height <= float.Epsilon)
            {
                return;
            }

            DrawCore();

            if (_showTooltip && _shouldShowTooltip)
            {
                TooltipHandler.TipRegion(
                    RenderRect.IntersectWith(ContentRect),
                    _tooltip);
            }

            if (Prefs.DevMode)
            {
                if (DrawRenderRect)
                {
                    Widgets.DrawBox(RenderRect);
                }

                if (DrawLayoutRect && (!DrawRenderRect || Margin != 0f))
                {
                    Widgets.DrawBox(DesiredRect, lineTexture: BaseContent.YellowTex);
                }

                if (ClickForceLayout
                    && Event.current.type is EventType.MouseDown
                    && RenderRect.IntersectWith(ContentRect).Contains(Event.current.mousePosition))
                {
                    if (ClickForceLayout)
                    {
                        Owner.LayoutManager.InvalidateLayout();
                    }
                }

                if (ShowInfo)
                {
                    if (!ControlInfoWindow.Instance.IsOpen)
                    {
                        ControlInfoWindow.Instance.Show();
                    }

                    if (RenderRect.IntersectWith(ContentRect).Contains(Event.current.mousePosition)
                        && (ControlInfoWindow.Instance.Source is null
                            || ControlInfoWindow.Instance.Source.Rank <= Rank))
                    {
                        ControlInfoWindow.Instance.SetSource(this);
                    }
                }
            }
        }

        /// <summary>
        /// 无效化控件排布
        /// </summary>
        /// <exception cref="InvalidOperationException">当一个不是 <see cref="ControlWindow"/> 的 <see cref="ControlWindow.Content"/> 的控件或其子控件尝试无效化排布时发生。</exception>
        public void InvalidateArrange()
        {
            if (!_isArrangeValid || _isIndependent)
            {
                return;
            }

            if (_isSegmentValid)
            {
                InvalidateSegment();
            }

            if (Owner is null)
            {
                Owner = LogicalTreeUtility.GetOwner(this)
                    ?? throw new InvalidOperationException($"Can not invalidate arrange for the control: {this} which is not the content of {typeof(ControlWindow)}."); ;
            }

            Owner.LayoutManager.InvalidateArrange(this);

            _isArrangeValid = false;
        }

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        /// <exception cref="InvalidOperationException">当一个不是 <see cref="ControlWindow"/> 的 <see cref="ControlWindow.Content"/> 的控件或其子控件尝试无效化度量时发生。</exception>
        public void InvalidateMeasure()
        {
            if (!_isMeasureValid || _isIndependent)
            {
                return;
            }

            if (_isArrangeValid)
            {
                InvalidateArrange();
            }

            if (Owner is null)
            {
                Owner = LogicalTreeUtility.GetOwner(this)
                    ?? throw new InvalidOperationException($"Can not invalidate measure for the control: {this} which is not the content of {typeof(ControlWindow)}."); ;
            }

            Owner.LayoutManager.InvalidateMeasure(this);

            _isMeasureValid = false;
        }

        /// <summary>
        /// 无效化控件分割
        /// </summary>
        /// <remarks>控件分割：由该控件控制呈现的可见区域。</remarks>
        /// <exception cref="InvalidOperationException">当一个不是 <see cref="ControlWindow"/> 的 <see cref="ControlWindow.Content"/> 的控件或其子控件尝试无效化分割时发生。</exception>
        public void InvalidateSegment()
        {
            if (!_isSegmentValid || _isIndependent)
            {
                return;
            }

            if (Owner is null)
            {
                Owner = LogicalTreeUtility.GetOwner(this)
                    ?? throw new InvalidOperationException($"Can not invalidate segment for the control: {this} which is not the content of {typeof(ControlWindow)}."); ;
            }

            Owner.LayoutManager.InvalidateSegment(this);

            _isSegmentValid = false;
        }

        /// <summary>
        /// 计算控件需要占用的布局尺寸
        /// </summary>
        /// <param name="availableSize">分配给控件的可用空间</param>
        /// <returns>控件需要占用的布局尺寸。</returns>
        public Size Measure(Size availableSize)
        {
            if (!(Visibility is Visibility.Collapsed))
            {
                Thickness margin = Margin;
                RenderSize = MeasureCore(availableSize - margin).Round();
                DesiredSize = RenderSize + margin;
            }
            else
            {
                RenderSize = Size.Empty;
                DesiredSize = Size.Empty;
            }

            _isMeasureValid = true;

            return DesiredSize;
        }

        /// <summary>
        /// 计算该控件控制呈现的可见区域
        /// </summary>
        /// <param name="visiableRect">控件的可见区域</param>
        /// <returns>该控件控制呈现的可见区域。</returns>
        public Rect Segment(Rect visiableRect)
        {
            if (!(Visibility is Visibility.Collapsed))
            {
                ContentRect = SegmentCore(visiableRect);
            }
            else
            {
                ContentRect = new Rect(visiableRect.x, visiableRect.y, 0f, 0f);
            }

            ContentSize = ContentRect;

            _isSegmentValid = true;

            return ContentRect;
        }

        /// <summary>
        /// 获取该控件的字符串表示形式
        /// </summary>
        /// <returns>该控件的字符串表示形式.</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(_name)
                ? base.ToString()
                : _name;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 计算呈现控件内容需要的区域
        /// </summary>
        /// <param name="availableRect">允许排布的区域</param>
        /// <returns>呈现控件内容需要的区域。</returns>
        protected abstract Rect ArrangeCore(Rect availableRect);

        /// <summary>
        /// 绘制控件
        /// </summary>
        protected abstract void DrawCore();

        /// <summary>
        /// 计算呈现控件内容需要的尺寸
        /// </summary>
        /// <param name="availableSize">分配给控件的可用空间</param>
        /// <returns>呈现控件内容需要的尺寸。</returns>
        protected virtual Size MeasureCore(Size availableSize) => availableSize;

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if ((_isArrangeValid || _isMeasureValid)
                && args.Metadata is ControlPropertyMetadata metadata)
            {
                if (metadata.Relation is ControlRelation.Measure)
                {
                    InvalidateMeasure();
                }
                else if (metadata.Relation is ControlRelation.Arrange)
                {
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// 计算该控件控制呈现的可见区域
        /// </summary>
        /// <param name="visiableRect">控件的可见区域</param>
        /// <returns>该控件控制呈现的可见区域。</returns>
        protected virtual Rect SegmentCore(Rect visiableRect)
        {
            return visiableRect.IntersectWith(RenderRect);
        }

        #endregion


        internal sealed class ControlInfoWindow : ControlWindow
        {
            internal static readonly ControlInfoWindow Instance = new ControlInfoWindow();


            internal Control Source;
            private Type _sourceType;

            private bool _isPanel;
            private Panel _panel;

            private TextBlock _infoBox;

            internal ControlInfoWindow()
            {
                draggable = true;
                drawShadow = false;
                resizeable = true;

                layer = WindowLayer.Super;

                InitialWidth = 450f;
                InitialHeight = 220f;

                Content = CreateDefaultContent();
            }


            public override Control CreateDefaultContent()
            {
                var scrollViewer = new ScrollViewer();
                _infoBox = new TextBlock
                {
                    Anchor = TextAnchor.MiddleLeft,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                };

                scrollViewer.Content = _infoBox;
                return scrollViewer;
            }

            public override bool OnCloseRequest()
            {
                return !ShowInfo;
            }

            public void SetSource(Control source)
            {
                Source = source;
                _sourceType = source.GetType();

                if (source is Panel panel)
                {
                    _isPanel = true;
                    _panel = panel;
                }
            }


            protected override void LateWindowOnGUI(Rect inRect)
            {
                base.LateWindowOnGUI(inRect);

                _infoBox.Text =
                    $"Type: {_sourceType}\n" +
                    $"Name: {Source._name}\n" +
                    $"RenderRect: {Source.RenderRect}\n" +
                    $"DesiredRect: {Source.DesiredRect}\n" +
                    $"ContentRect: {Source.ContentRect}\n";

                if (_isPanel)
                {
                    _infoBox.Text +=
                        $"FilteredChildren.Count: {_panel.FilteredChildren.Length}";
                }
            }
        }
    }
}
