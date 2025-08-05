using Nebulae.RimWorld.UI.Automation.Diagnostics;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    // Priority 0 ~ 39

    /// <summary>
    /// Nebulae's Control 的基本控件
    /// </summary>
    [DebuggerStepThrough]
    public abstract partial class Control : DependencyObject
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 控件响应用户交互的区域
        /// </summary>
        [DebugMember(int.MinValue + 30)]
        public Rect ControlRect;

        /// <summary>
        /// 控件响应用户交互的尺寸
        /// </summary>
        [DebugMember(int.MinValue + 31)]
        public Size ControlSize = Size.Empty;

        /// <summary>
        /// 控件需要占用的布局区域
        /// </summary>
        [DebugMember(int.MinValue + 32)]
        public Rect DesiredRect;

        /// <summary>
        /// 控件需要占用的布局尺寸
        /// </summary>
        [DebugMember(int.MinValue + 33)]
        public Size DesiredSize = Size.Empty;

        /// <summary>
        /// 控件呈现内容的可见区域
        /// </summary>
        [DebugMember(int.MinValue + 38)]
        public Rect VisibleRect;

        /// <summary>
        /// 控件呈现内容的可见尺寸
        /// </summary>
        [DebugMember(int.MinValue + 39)]
        public Size VisibleSize = Size.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取 <see cref="Control"/> 的状态
        /// </summary>
        public ControlState ControlStates
        {
            get => _controlStates;
            internal set
            {
                if (_controlStates != value)
                {
                    _controlStates = value;

                    OnStatesChanged(value);
                }
            }
        }

        /// <summary>
        /// 获取一个值，该值表示光标是否直接位于 <see cref="Control"/> 的交互区域上方
        /// </summary>
        [DebugMember(int.MinValue + 6)]
        public bool CursorDirectlyOver => _controlStates.HasState(ControlState.CursorDirectlyOver);

        /// <summary>
        /// 获取一个值，该值表示光标是否位于 <see cref="Control"/> 的交互区域内
        /// </summary>
        [DebugMember(int.MinValue + 7)]
        public bool CursorOver => _controlStates.HasState(ControlState.CursorOver);

        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="Control"/> 要绘制的调试内容
        /// </summary>
        public DebugContent DebugContent
        {
            get => _debugContent;
            set => _debugContent = value;
        }

        /// <summary>
        /// 获取 <see cref="Control"/> 的所有子控件
        /// </summary>
        public IEnumerable<Control> Descendants
        {
            get
            {
                foreach (var child in LogicalChildren)
                {
                    yield return child;

                    foreach (var grandchild in child.Descendants)
                    {
                        yield return grandchild;
                    }
                }
            }
        }

        /// <summary>
        /// 获取一个值，该值指示 <see cref="Control"/> 的排布是否有效
        /// </summary>
        [DebugMember(int.MinValue + 9)]
        public bool IsArrangeValid
        {
            get
            {
                if (!_isArrangeValid)
                {
                    return false;
                }

                if (LayoutManager?.IsArrangeValid(this) ?? true)
                {
                    _isArrangeValid = false;

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 获取或设置 <see cref="Control"/> 的拖动效果透明度
        /// </summary>
        public float DragOpacity
        {
            get => _dragOpacity;
            set => _dragOpacity = value;
        }

        /// <summary>
        /// 获取一个值，该值指示 <see cref="Control"/> 是否正在被拖动
        /// </summary>
        [DebugMember(int.MinValue + 8)]
        public bool IsDragging => _controlStates.HasState(ControlState.Dragging);

        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="Control"/> 是否参与命中测试
        /// </summary>
        [DebugMember(int.MinValue + 25)]
        public bool IsHitTestVisible
        {
            get => _isHitTestVisible;
            set
            {
                if (_isHitTestVisible != value)
                {
                    _isHitTestVisible = value;
                    InvalidateSegment();
                }
            }
        }

        /// <summary>
        /// 获取一个值，该值指示 <see cref="Control"/> 的测量是否有效
        /// </summary>
        [DebugMember(int.MinValue + 10)]
        public bool IsMeasureValid
        {
            get
            {
                if (!_isMeasureValid)
                {
                    return false;
                }

                if (LayoutManager?.IsMeasureValid(this) ?? true)
                {
                    _isMeasureValid = false;

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示 <see cref="Control"/> 的分割是否有效
        /// </summary>
        [DebugMember(int.MinValue + 11)]
        public bool IsSegmentValid
        {
            get
            {
                if (!_isSegmentValid)
                {
                    return false;
                }

                if (LayoutManager?.IsSegmentValid(this) ?? true)
                {
                    _isSegmentValid = false;

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示子控件是否可影响 <see cref="Control"/> 的布局
        /// </summary>
        /// <remarks>该值应只在初始化时被修改。</remarks>
        [DebugMember(int.MinValue + 22)]
        public bool IsSolid
        {
            get => _isSolid;
            protected set
            {
                if (_isSolid != value)
                {
                    if (_isSolid)
                    {
                        InvalidateMeasure();
                    }

                    _isSolid = value;
                }
            }
        }

        /// <summary>
        /// 获取 <see cref="Control"/> 的布局管理器
        /// </summary>
        public LayoutManager LayoutManager
        {
            get
            {
                var control = this;

                while (control._isChild)
                {
                    control = control._parent;
                }

                return control._layoutManager;
            }
            internal set
            {
                if (_isChild)
                {
                    throw new LogicalTreeException("Only root controls can have a LayoutManager assigned. To assign a LayoutManager, ensure this control is not a child of another control.", this);
                }

                _layoutManager = value;
            }
        }

        /// <summary>
        /// 获取 <see cref="Control"/> 的逻辑子控件
        /// </summary>
        /// <remarks>需保证返回的集合没有空元素。</remarks>
        public virtual IEnumerable<Control> LogicalChildren => Enumerable.Empty<Control>();

        /// <summary>
        /// 获取或设置 <see cref="Control"/> 的可视化父级
        /// </summary>
        public Control Parent
        {
            get => _parent;
            set
            {
                if (_isChild)
                {
                    if (value is null)
                    {
                        _isChild = false;
                        _parent = null;
                    }
                    else if (!ReferenceEquals(_parent, value))
                    {
                        throw new LogicalTreeException("This control is already a child of another parent. To reparent, remove it from its current parent first.", this);
                    }
                }
                else
                {
                    _isChild = !(value is null);
                    _layoutManager = null;
                    _parent = value;
                }

                InvalidateMeasure();
            }
        }

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// 获取或设置 <see cref="Control"/> 的工具提示内容
        /// </summary>
        public TipSignal Tooltip
        {
            get => _tooltip;
            set
            {
                _shouldShowTooltip = !string.IsNullOrEmpty(
                    string.IsNullOrEmpty(value.text) ? value.textGetter?.Invoke() : value.text);
                _tooltip = value;
            }
        }

        #endregion


#if DEBUG
        /// <summary>
        /// <see cref="Control"/> 的析构函数
        /// </summary>
        ~Control()
        {
            System.Diagnostics.Debug.WriteLine($"[NebulaeFlood's Lib] A control of type:{Type} has been collected.");
        }
#endif


        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region AllowDrag
        /// <summary>
        /// 获取或设置一个值，该值指示该 <see cref="Control"/> 是否可被拖动
        /// </summary>
        [DebugMember(int.MinValue + 21)]
        public bool AllowDrag
        {
            get { return (bool)GetValue(AllowDragProperty); }
            set { SetValue(AllowDragProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="AllowDrag"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty AllowDragProperty =
            DependencyProperty.Register(nameof(AllowDrag), typeof(bool), typeof(Control),
                new PropertyMetadata(false));
        #endregion

        #region IsEnabled
        /// <summary>
        /// 获取或设置一个值，该值指示是否在用户界面中启用此 <see cref="Control"/>
        /// </summary>
        [DebugMember(int.MinValue + 20)]
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsEnabled"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(Control),
                new PropertyMetadata(true, OnIsEnabledChanged));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;

            if (e.NewValue is true)
            {
                control.ControlStates &= ~ControlState.Disabled;
            }
            else
            {
                control.ControlStates |= ControlState.Disabled;
            }
        }
        #endregion

        #region Opacity
        /// <summary>
        /// 获取或设置 <see cref="Control"/> 的不透明度
        /// </summary>
        [DebugMember(int.MinValue + 24)]
        public float Opacity
        {
            get { return (float)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Opacity"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(nameof(Opacity), typeof(float), typeof(Control),
                new PropertyMetadata(1f, CoerceOpacity, OnOpacityChanged));

        private static object CoerceOpacity(DependencyObject d, object baseValue)
        {
            return Mathf.Clamp((float)baseValue, 0f, 1f);
        }

        private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var opacity = (float)e.NewValue;
            var visual = (Control)d;

            visual._allowDraw = opacity > 0f && visual.Visibility is Visibility.Visible;
            visual._opacityColor = new Color(1f, 1f, 1f, opacity);
        }
        #endregion

        #region Visibility
        /// <summary>
        /// 获取或设置 <see cref="Control"/> 的显示状态
        /// </summary>
        [DebugMember(int.MinValue + 23)]
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Visibility"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register(nameof(Visibility), typeof(Visibility), typeof(Control),
                new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var visual = (Control)d;

            switch (e.NewValue)
            {
                case Visibility.Visible:
                    visual._allowOccupy = true;
                    visual._allowDraw = visual.Opacity > 0f;
                    break;
                case Visibility.Collapsed:
                    visual._allowOccupy = false;
                    visual._allowDraw = false;
                    visual.InvalidateMeasure();
                    break;
                case Visibility.Hidden:
                    visual._allowOccupy = true;
                    visual._allowDraw = false;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #endregion


        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        internal bool DebugControlRect
        {
            get => _debugContent.HasFlag(DebugContent.ControlRect);
            set
            {
                if (value)
                {
                    _debugContent |= DebugContent.ControlRect;
                }
                else
                {
                    _debugContent &= ~DebugContent.ControlRect;
                }
            }
        }

        internal bool DebugDesiredRect
        {
            get => _debugContent.HasFlag(DebugContent.DesiredRect);
            set
            {
                if (value)
                {
                    _debugContent |= DebugContent.DesiredRect;
                }
                else
                {
                    _debugContent &= ~DebugContent.DesiredRect;
                }
            }
        }

        internal bool DebugRenderRect
        {
            get => _debugContent.HasFlag(DebugContent.RenderRect);
            set
            {
                if (value)
                {
                    _debugContent |= DebugContent.RenderRect;
                }
                else
                {
                    _debugContent &= ~DebugContent.RenderRect;
                }
            }
        }

        internal bool DebugRegionRect
        {
            get => _debugContent.HasFlag(DebugContent.RegionRect);
            set
            {
                if (value)
                {
                    _debugContent |= DebugContent.RegionRect;
                }
                else
                {
                    _debugContent &= ~DebugContent.RegionRect;
                }
            }
        }

        internal bool DebugVisibleRect
        {
            get => _debugContent.HasFlag(DebugContent.VisibleRect);
            set
            {
                if (value)
                {
                    _debugContent |= DebugContent.VisibleRect;
                }
                else
                {
                    _debugContent &= ~DebugContent.VisibleRect;
                }
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 计算 <see cref="Control"/> 需要占用的区域
        /// </summary>
        /// <param name="availableRect">分配的区域</param>
        /// <returns>需要占用的区域。</returns>
        public Rect Arrange(Rect availableRect)
        {
            _isArrangeValid = true;

            DesiredRect = _allowOccupy
                ? ArrangeCore(availableRect).Rounded()
                : new Rect(availableRect.x, availableRect.y, 0f, 0f).Rounded();

            return DesiredRect;
        }

        /// <summary>
        /// 无效化 <see cref="Control"/> 的排布
        /// </summary>
        public void InvalidateArrange()
        {
            if (!_isArrangeValid)
            {
                return;
            }

            LayoutManager?.InvalidateArrange(this);

            _isArrangeValid = false;
            _isSegmentValid = false;
        }

        /// <summary>
        /// 计算 <see cref="Control"/> 需要占用的尺寸
        /// </summary>
        /// <param name="availableSize">分配的可用空间</param>
        /// <returns>需要占用的尺寸。</returns>
        public Size Measure(Size availableSize)
        {
            _isMeasureValid = true;

            DesiredSize = _allowOccupy
                ? MeasureCore(availableSize).Format()
                : Size.Empty;

            return DesiredSize;
        }

        /// <summary>
        /// 无效化 <see cref="Control"/> 的测量
        /// </summary>
        public void InvalidateMeasure()
        {
            if (!_isMeasureValid || _isSolid)
            {
                return;
            }

            LayoutManager?.InvalidateMeasure(this);

            _isArrangeValid = false;
            _isMeasureValid = false;
            _isSegmentValid = false;
        }

        /// <summary>
        /// 计算 <see cref="Control"/> 的交互区域和可见区域
        /// </summary>
        /// <param name="visiableRect">允许显示的区域</param>
        /// <returns>交互区域和可见区域。</returns>
        public SegmentResult Segment(Rect visiableRect)
        {
            _isSegmentValid = true;

            if (_allowDraw)
            {
                var result = SegmentCore(visiableRect);

                ControlRect = result.ControlRect;
                ControlSize = ControlRect;

                VisibleRect = result.VisibleRect;
                VisibleSize = VisibleRect;

                _tooltipRect = result.TooltipRect;

                return result;
            }
            else
            {
                ControlRect = new Rect(visiableRect.x, visiableRect.y, 0f, 0f).Rounded();
                ControlSize = Size.Empty;

                VisibleRect = ControlRect;
                VisibleSize = Size.Empty;

                return ControlRect;
            }
        }

        /// <summary>
        /// 无效化 <see cref="Control"/> 的分割
        /// </summary>
        public void InvalidateSegment()
        {
            if (!_isSegmentValid || LayoutManager is null)
            {
                return;
            }

            LayoutManager.InvalidateSegment(this);

            _isSegmentValid = false;
        }

        /// <summary>
        /// 命中测试 <see cref="Control"/> 的交互区域
        /// </summary>
        /// <param name="hitPoint">进行命中测试的点</param>
        /// <returns><see cref="Control"/> 被命中的实例。</returns>
        public HitTestResult HitTest(Vector2 hitPoint)
        {
            return (!_allowDraw || VisibleSize.IsEmpty) ? HitTestResult.Empty : HitTestCore(hitPoint);
        }

        /// <summary>
        /// 绘制 <see cref="Control"/> 的内容
        /// </summary>
        public void Draw()
        {
            if (!_allowDraw || VisibleSize.IsEmpty)
            {
                return;
            }

            Color color = GUI.color;
            Color contentColor = GUI.contentColor;

            GUI.color = _opacityColor * color;
            GUI.contentColor = _opacityColor * contentColor;
            DrawCore(_controlStates);
            GUI.color = color;
            GUI.contentColor = contentColor;

            if (_shouldShowTooltip && !InputUtility.MouseTracker.anyButtonPressing)
            {
                TooltipHandler.TipRegion(_tooltipRect, _tooltip);
            }

            if (UIUtility.DebugMode)
            {
                DrawDebugContent();
            }
        }

        /// <summary>
        /// 获取表示该 <see cref="Control"/> 的字符串
        /// </summary>
        /// <returns>表示该 <see cref="Control"/> 的字符串。</returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(_name) ? Type.FullName : _name;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal bool CanHitTest()
        {
            return _isHitTestVisible && !_controlStates.HasState(ControlState.Disabled);
        }

        internal virtual void DrawDebugContent()
        {
            if (DebugVisibleRect && VisibleSize != Size.Empty)
            {
                UIUtility.DrawBorder(VisibleRect, BrushUtility.VisibleRectBorderBrush);
            }

            if (DebugDesiredRect && DesiredSize != Size.Empty)
            {
                UIUtility.DrawBorder(DesiredRect, BrushUtility.DesiredRectBorderBrush);
            }

            if (DebugControlRect && ControlSize != Size.Empty)
            {
                UIUtility.DrawBorder(ControlRect, BrushUtility.ControlRectBorderBrush);
            }
        }

        internal void DrawDragEffect()
        {
            if (_controlStates.HasState(ControlState.Dragging))
            {
                DrawDragEffectCore();
            }
        }

        internal void DrawLightly(float opacity)
        {
            if (!_allowDraw || VisibleSize.IsEmpty)
            {
                return;
            }

            var color = GUI.color;
            var contentColor = GUI.contentColor;
            var opacityColor = new Color(1f, 1f, 1f, opacity);

            GUI.color = opacityColor * color;
            GUI.contentColor = opacityColor * contentColor;
            DrawCore(ControlState.Normal);
            GUI.color = color;
            GUI.contentColor = contentColor;
        }

        internal Control GetSolidParent() => (!_isChild || _parent._isSolid) ? _parent : _parent.GetSolidParent();

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 计算 <see cref="Control"/> 需要占用的区域
        /// </summary>
        /// <param name="availableRect">分配的区域</param>
        /// <returns>需要占用的区域。</returns>
        protected virtual Rect ArrangeCore(Rect availableRect) => availableRect;

        /// <summary>
        /// 计算 <see cref="Control"/> 需要占用的尺寸
        /// </summary>
        /// <param name="availableSize">分配的可用空间</param>
        /// <returns>需要占用的尺寸。</returns>
        protected virtual Size MeasureCore(Size availableSize) => availableSize;

        /// <summary>
        /// 计算 <see cref="Control"/> 的可见区域
        /// </summary>
        /// <param name="visiableRect">允许显示的区域</param>
        /// <returns><see cref="Control"/> 的交互区域和可见区域。</returns>
        /// <remarks>需要调用子对象的 <see cref="Segment(Rect)"/> 方法。</remarks>
        protected virtual SegmentResult SegmentCore(Rect visiableRect) => visiableRect.IntersectWith(DesiredRect);

        /// <summary>
        /// 命中测试 <see cref="Control"/> 的交互区域
        /// </summary>
        /// <param name="hitPoint">进行命中测试的点</param>
        /// <returns>命中测试结果。</returns>
        /// <remarks>必须先保证自身被 <see cref="HitTestResult.HitTest(Control, bool)"/> 或 <see cref="HitTestResult.HitTest(Control, Vector2)"/> 处理过，再保证子控件都调用过 <see cref="HitTest(Vector2)"/>。</remarks>
        protected virtual HitTestResult HitTestCore(Vector2 hitPoint) => HitTestResult.HitTest(this, hitPoint);

        /// <summary>
        /// 绘制 <see cref="Control"/> 的内容
        /// </summary>
        /// <param name="states">当前的状态</param>
        protected abstract void DrawCore(ControlState states);

        /// <summary>
        /// 计算 <see cref="Control"/> 拖动效果的绘制区域
        /// </summary>
        /// <param name="cursorPos">光标位置</param>
        /// <returns><see cref="Control"/> 拖动效果的绘制区域。</returns>
        protected internal virtual Rect CalculateDragEffectRect(Vector2 cursorPos)
        {
            return new Rect(
                cursorPos.x - DesiredSize.Width * 0.5f,
                cursorPos.y - DesiredSize.Height * 0.5f,
                DesiredSize.Width,
                DesiredSize.Height);
        }

        /// <summary>
        /// 绘制 <see cref="Control"/> 的拖动效果
        /// </summary>
        protected internal virtual void DrawDragEffectCore()
        {
            float x = DesiredRect.x;
            float y = DesiredRect.y;

            float width = Mathf.Abs(x) + DesiredSize.Width;
            float height = Mathf.Abs(y) + DesiredSize.Height;

            GUI.BeginClip(new Rect(0f, 0f, DesiredSize.Width, DesiredSize.Height));
            GUI.BeginGroup(new Rect(-x, -y, width, height));

            DrawLightly(_dragOpacity);

            GUI.EndGroup();
            GUI.EndClip();
        }

        /// <inheritdoc/>
        protected override void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.Metadata is ControlPropertyMetadata metadata)
            {
                switch (metadata.Relation)
                {
                    case ControlRelation.Arrange:
                        InvalidateArrange();
                        break;
                    case ControlRelation.Measure:
                        InvalidateMeasure();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 当 <see cref="ControlStates"/> 发生变化时调用
        /// </summary>
        /// <param name="states">当前状态</param>
        protected virtual void OnStatesChanged(ControlState states) { }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private ControlState _controlStates = ControlState.Normal;
        private DebugContent _debugContent = DebugContent.Empty;

        private float _dragOpacity = 1f;

        private LayoutManager _layoutManager;

        private bool _isHitTestVisible;
        private bool _isSolid;

        [DebugMember(int.MinValue + 1, Name = "Name")]
        private string _name = string.Empty;

        private Control _parent;

        private TipSignal _tooltip = string.Empty;

        // Caches

        [DebugMember(int.MinValue + 2, Name = "AllowDraw")]
        private bool _allowDraw = true;
        [DebugMember(int.MinValue + 3, Name = "AllowOccupy")]
        private bool _allowOccupy = true;

        private bool _isArrangeValid;
        private bool _isMeasureValid;
        private bool _isSegmentValid;

        private bool _isChild;

        private Color _opacityColor = new Color(1f, 1f, 1f, 1f);

        private bool _shouldShowTooltip;

        private Rect _tooltipRect;

        #endregion
    }
}
