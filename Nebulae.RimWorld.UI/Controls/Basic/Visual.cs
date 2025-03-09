using Nebulae.RimWorld.UI.Automation;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 为 Nebulae Control 呈现提供支持
    /// </summary>
    public abstract partial class Visual : DependencyObject
    {
        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        /// <summary>
        /// 控件是否为其他控件的逻辑子控件
        /// </summary>
        internal bool IsChild = false;

        /// <summary>
        /// 控件的父控件
        /// </summary>
        internal Visual Parent;

        /// <summary>
        /// 绘制预览状态
        /// </summary>
        internal static bool PreviewDraw = false;

        /// <summary>
        /// 控件在逻辑树中的层次
        /// </summary>
        [DebugInfoEntry(Priority = int.MinValue + 5)]
        internal int Rank = 0;

        /// <summary>
        /// 是否要绘制提示框
        /// </summary>
        internal bool ShouldShowTooltip;

        /// <summary>
        /// 提示框内容
        /// </summary>
        internal TipSignal TooltipContent = string.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 控件呈现内容的可见区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Segment(Rect)"/>。</remarks>
        [DebugInfoEntry(Priority = 0)]
        public Rect ContentRect;

        /// <summary>
        /// 控件呈现内容的可见尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Segment(Rect)"/>。</remarks>
        [DebugInfoEntry(Priority = 0)]
        public Size ContentSize;

        /// <summary>
        /// 控件响应用户交互的区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Segment(Rect)"/>。</remarks>
        [DebugInfoEntry(Priority = 1)]
        public Rect ControlRect;

        /// <summary>
        /// 控件响应用户交互的尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Segment(Rect)"/>。</remarks>
        [DebugInfoEntry(Priority = 1)]
        public Size ControlSize;

        /// <summary>
        /// 控件需要占用的布局区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Arrange(Rect)"/>。</remarks>
        [DebugInfoEntry(Priority = 2)]
        public Rect DesiredRect;

        /// <summary>
        /// 控件需要占用的布局尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Measure(Size)"/>。</remarks>
        [DebugInfoEntry(Priority = 2)]
        public Size DesiredSize = Size.Empty;

        /// <summary>
        /// 计算的将要绘制的区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Arrange(Rect)"/>。</remarks>
        [DebugInfoEntry(Priority = 3)]
        public Rect RenderRect;

        /// <summary>
        /// 计算的将要绘制的尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Measure(Size)"/>。</remarks>
        [DebugInfoEntry(Priority = 3)]
        public Size RenderSize = Size.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Privaet Fields
        //
        //------------------------------------------------------

        #region Privaet Fields

        private bool _isArrangeValid = false;

        private bool _isDraggable = false;
        private bool _isEnabled = true;
        private bool _isHitTestVisible = false;
        private bool _isIndependent = true;
        private bool _isMeasureValid = false;
        private bool _isSegmentValid = false;

        private LayoutManager _layoutManager;

        private string _name = string.Empty;

        private Color _opacityColor = new Color(1f, 1f, 1f, 1f);
        private Window _owner;

        private DebugContent _debugContent = DebugContent.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Public Debug Properties
        //
        //------------------------------------------------------

        #region Public Debug Properties

        /// <summary>
        /// 要绘制的调试内容
        /// </summary>
        public DebugContent DebugContent
        {
            get => _debugContent;
            set => _debugContent = value;
        }

        /// <summary>
        /// 是否绘制控件可见区域
        /// </summary>
        public bool DebugDrawContentRect
        {
            get => _debugContent.HasFlag(DebugContent.ContentRect);
            set
            {
                if (value)
                {
                    _debugContent |= DebugContent.ContentRect;
                }
                else
                {
                    _debugContent &= ~DebugContent.ContentRect;
                }
            }
        }

        /// <summary>
        /// 是否绘制可交互区域
        /// </summary>
        public bool DebugDrawControlRect
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

        /// <summary>
        /// 是否绘制控件布局区域
        /// </summary>
        public bool DebugDrawDesiredRect
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

        /// <summary>
        /// 是否绘制控件绘制区域
        /// </summary>
        public bool DebugDrawRenderRect
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

        #endregion


        //------------------------------------------------------
        //
        //  Public Dependency Properties
        //
        //------------------------------------------------------

        #region Public Dependency Properties

        #region Margin
        /// <summary>
        /// 获取或设置控件的外部边距
        /// </summary>
        [DebugInfoEntry(Priority = 4)]
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value.Normalize()); }
        }

        /// <summary>
        /// 标识 <see cref="Margin"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(nameof(Margin), typeof(Thickness), typeof(Visual),
                new ControlPropertyMetadata(Thickness.Empty, ControlRelation.Measure));
        #endregion

        #region Opacity
        /// <summary>
        /// 获取或设置控件的不透明度
        /// </summary>
        public float Opacity
        {
            get { return (float)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Opacity"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(nameof(Opacity), typeof(float), typeof(Visual),
                new PropertyMetadata(1f, OnOpacityChanged),
                ValidateOpacity);

        private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Visual)d;
            float opacity = (float)e.NewValue;

            control._opacityColor = new Color(1f, 1f, 1f, opacity);
        }

        private static bool ValidateOpacity(object value)
        {
            return (float)value >= 0f;
        }
        #endregion

        #region Padding
        /// <summary>
        /// 获取或设置控件内容的内部边距
        /// </summary>
        [DebugInfoEntry(Priority = 5)]
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value.Normalize()); }
        }

        /// <summary>
        /// 标识 <see cref="Padding"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(Visual),
                new ControlPropertyMetadata(Thickness.Empty, ControlRelation.Measure));
        #endregion

        #region Visibility
        /// <summary>
        /// 获取或设置控件的显示状态
        /// </summary>
        [DebugInfoEntry(Priority = int.MinValue + 4)]
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Visibility"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register(nameof(Visibility), typeof(Visibility), typeof(Visual),
                new PropertyMetadata(Visibility.Visible));

        #endregion

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 控件布局是否有效
        /// </summary>
        [DebugInfoEntry(Priority = 6)]
        public bool IsArrangeValid
        {
            get
            {
                if (!_isArrangeValid || _isIndependent)
                {
                    return false;
                }

                if (LayoutManager.IsArrangeValid(this))
                {
                    return true;
                }

                _isArrangeValid = false;

                return false;
            }
        }

        /// <summary>
        /// 控件是否能够交互
        /// </summary>
        [DebugInfoEntry(Priority = 9)]
        public bool IsHitTestVisible
        {
            get => _isHitTestVisible;
            set
            {
                if (_isHitTestVisible != value)
                {
                    _isHitTestVisible = value;

                    ControlRect = _isHitTestVisible
                        ? AnalyseCore(ContentRect)
                        : new Rect(ContentRect.x, ContentRect.y, 0f, 0f);
                    ContentSize = ContentRect;
                }
            }
        }

        /// <summary>
        /// 光标是否位于控件上方
        /// </summary>
        /// <remarks>对于 <see cref="IsHitTestVisible"/> 为 <see langword="false"/> 的控件，该项永远为 <see langword="false"/>。</remarks>
        [DebugInfoEntry(Priority = 10)]
        public bool IsCursorOver => _isHitTestVisible
            && ReferenceEquals(MouseUtility.HoveredControl, this);

        /// <summary>
        /// 控件是否可拖动
        /// </summary>
        [DebugInfoEntry(Priority = 11)]
        public bool IsDraggable
        {
            get => _isDraggable;
            set => _isDraggable = value;
        }

        /// <summary>
        /// 控件是否正在被拖动
        /// </summary>
        /// <remarks>对于 <see cref="IsHitTestVisible"/> 为 <see langword="false"/> 的控件，该项永远为 <see langword="false"/>。</remarks>
        [DebugInfoEntry(Priority = 12)]
        public bool IsDragging => _isDraggable && MouseUtility.AnyDragging
            && ReferenceEquals(MouseUtility.DraggingControl, this);

        /// <summary>
        /// 正在拖动的控件是否正在该控件上方
        /// </summary>
        /// <remarks>对于 <see cref="IsHitTestVisible"/> 为 <see langword="false"/> 的控件，该项永远为 <see langword="false"/>。</remarks>
        [DebugInfoEntry(Priority = 13)]
        public bool IsDragOwer => _isHitTestVisible && MouseUtility.AnyDragging
            && ReferenceEquals(MouseUtility.HoveredControl, this);

        /// <summary>
        /// 是否在界面中启用该控件
        /// </summary>
        [DebugInfoEntry(Priority = int.MinValue + 2)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;

                    foreach (var child in LogicalChildren)
                    {
                        child.IsEnabled = _isEnabled;
                    }

                    OnIsEnabledChanged(_isEnabled);
                }
            }
        }

        /// <summary>
        /// 控件是否独立于 <see cref="LayoutManager"/>
        /// </summary>
        [DebugInfoEntry(Priority = int.MinValue + 3)]
        public bool IsIndependent => _isIndependent;

        /// <summary>
        /// 控件度量是否有效
        /// </summary>
        [DebugInfoEntry(Priority = 7)]
        public bool IsMeasureValid
        {
            get
            {
                if (!_isMeasureValid || _isIndependent)
                {
                    return false;
                }

                if (LayoutManager.IsMeasureValid(this))
                {
                    return true;
                }

                _isMeasureValid = false;

                return false;
            }
        }

        /// <summary>
        /// 光标是否在控件上方按下
        /// </summary>
        /// <remarks>对于 <see cref="IsHitTestVisible"/> 为 <see langword="false"/> 的控件，该项永远为 <see langword="false"/>。</remarks>
        [DebugInfoEntry(Priority = 14)]
        public bool IsPressing => _isHitTestVisible && MouseUtility.AnyPressing
            && ReferenceEquals(MouseUtility.PressingControl, this);

        /// <summary>
        /// 控件分割是否有效
        /// </summary>
        [DebugInfoEntry(Priority = 8)]
        public bool IsSegmentValid
        {
            get
            {
                if (!_isSegmentValid || _isIndependent)
                {
                    return false;
                }

                if (LayoutManager.IsSegmentValid(this))
                {
                    return true;
                }

                _isSegmentValid = false;

                return false;
            }
        }

        /// <summary>
        /// 控件的布局管理器
        /// </summary>
        public LayoutManager LayoutManager
        {
            get => _layoutManager;
            internal set
            {
                if (value is null)
                {
                    _isIndependent = true;
                    _layoutManager = null;
                    _owner = null;
                }
                else
                {
                    _isIndependent = false;
                    _layoutManager = value;
                    _owner = value.Owner;
                }
            }
        }

        /// <summary>
        /// 控件的所有逻辑子控件
        /// </summary>
        public IEnumerable<Visual> LogicalChildren
        {
            get
            {
                foreach (var child in EnumerateLogicalChildren())
                {
                    yield return child;

                    foreach (var grandchild in child.LogicalChildren)
                    {
                        yield return grandchild;
                    }
                }
            }
        }

        /// <summary>
        /// 控件名称
        /// </summary>
        [DebugInfoEntry(Priority = int.MinValue)]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// 负责呈现控件的窗口
        /// </summary>
        /// <remarks>当 <see cref="IsIndependent"/> 为 <see langword="true"/> 时必定为 <see langword="null"/>。</remarks>
        [DebugInfoEntry(Priority = int.MinValue + 1)]
        public Window Owner
        {
            get
            {
                if (_isIndependent)
                {
                    return null;
                }

                return _owner;
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
        /// 计算控件需要占用的布局区域
        /// </summary>
        /// <param name="availableRect">分配给控件的区域</param>
        /// <returns>控件需要占用的布局区域。</returns>
        /// <remarks>需要保证调用过 <see cref="Measure(Size)"/>。</remarks>
        public Rect Arrange(Rect availableRect)
        {
            if (Visibility is Visibility.Collapsed)
            {
                RenderRect = new Rect(availableRect.x, availableRect.y, 0f, 0f);
                DesiredRect = RenderRect;
            }
            else
            {
                Thickness margin = Margin;
                Thickness padding = Padding;
                RenderRect = ArrangeCore(availableRect - (margin + padding)).Rounded() + padding;
                DesiredRect = RenderRect + margin;
            }

            _isArrangeValid = true;

            return DesiredRect;
        }

        /// <summary>
        /// 绘制调试内容
        /// </summary>
        public void DebugDraw()
        {
            if (_debugContent is DebugContent.Empty)
            {
                return;
            }

            if (_debugContent.HasFlag(DebugContent.RenderRect) && RenderSize != 0f)
            {
                UIUtility.DrawBorder(RenderRect, UIUtility.RederRectBorderBrush);
            }

            if (_debugContent.HasFlag(DebugContent.ContentRect) && ContentSize != 0f)
            {
                UIUtility.DrawBorder(ContentRect, UIUtility.ContentRectBorderBrush);
            }

            if (_debugContent.HasFlag(DebugContent.DesiredRect) && DesiredSize != 0f)
            {
                UIUtility.DrawBorder(DesiredRect, UIUtility.DesiredRectBorderBrush);
            }

            if (_debugContent.HasFlag(DebugContent.ControlRect) && ControlSize != 0f)
            {
                UIUtility.DrawBorder(ControlRect, UIUtility.ControlRectBorderBrush);
                DrawInnerControlRect();
            }
        }

        /// <summary>
        /// 使用当前计算的布局信息绘制控件
        /// </summary>
        public void Draw()
        {
            if (RenderSize < Size.Epsilon)
            {
                return;
            }

            if (PreviewDraw)
            {
                DrawCore();
            }
            else
            {
                if (_isHitTestVisible
                    && MouseUtility.IsHitTesting
                    && ControlRect.Contains(Event.current.mousePosition)
                    && ReferenceEquals(Owner, MouseUtility.HoveredWindow))
                {
                    MouseUtility.CurrentHoveredControl = this;
                }

                Color color = GUI.color;
                Color contentColor = GUI.contentColor;

                GUI.color = _opacityColor * color;
                GUI.contentColor = _opacityColor * contentColor;
                DrawCore();
                GUI.color = color;
                GUI.contentColor = contentColor;

                if (ShouldShowTooltip && !MouseUtility.IsPressing)
                {
                    TooltipHandler.TipRegion(ContentRect, TooltipContent);
                }

                if (Prefs.DevMode && !_isIndependent)
                {
                    DebugDraw();
                }
            }
        }

        /// <summary>
        /// 获取控件的父控件
        /// </summary>
        /// <returns>控件的父控件。</returns>
        public Visual GetParent()
        {
            return Parent;
        }

        /// <summary>
        /// 无效化控件排布
        /// </summary>
        public void InvalidateArrange()
        {
            if (!_isArrangeValid)
            {
                return;
            }

            if (!_isIndependent)
            {
                LayoutManager.InvalidateArrange(this);
            }

            _isArrangeValid = false;
            _isSegmentValid = false;
        }

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        public void InvalidateMeasure()
        {
            if (!_isMeasureValid)
            {
                return;
            }

            if (!_isIndependent)
            {
                LayoutManager.InvalidateMeasure(this);
            }

            _isArrangeValid = false;
            _isMeasureValid = false;
            _isSegmentValid = false;
        }

        /// <summary>
        /// 无效化控件分割
        /// </summary>
        /// <remarks>将会影响 <see cref="ContentRect"/> 和 <see cref="ControlRect"/> 及其对应的 <see cref="Size"/>。</remarks>
        public void InvalidateSegment()
        {
            if (!_isSegmentValid)
            {
                return;
            }

            if (!_isIndependent)
            {
                LayoutManager.InvalidateSegment(this);
            }

            _isSegmentValid = false;
        }

        /// <summary>
        /// 计算控件需要占用的布局尺寸
        /// </summary>
        /// <param name="availableSize">分配给控件的可用空间</param>
        /// <returns>控件需要占用的布局尺寸。</returns>
        public Size Measure(Size availableSize)
        {
            if (Visibility is Visibility.Collapsed)
            {
                RenderSize = Size.Empty;
                DesiredSize = Size.Empty;
            }
            else
            {
                Thickness margin = Margin;
                Thickness padding = Padding;
                RenderSize = MeasureCore(availableSize - (margin + padding)).Round() + padding;
                DesiredSize = RenderSize + margin;
            }

            _isMeasureValid = true;

            return DesiredSize;
        }

        /// <summary>
        /// 计算该控件呈现内容的可见区域
        /// </summary>
        /// <param name="visiableRect">允许控件显示的区域</param>
        /// <returns>该控件呈现内容的可见区域。</returns>
        public Rect Segment(Rect visiableRect)
        {
            if (Visibility is Visibility.Collapsed)
            {
                ContentRect = new Rect(visiableRect.x, visiableRect.y, 0f, 0f);
                ControlRect = ContentRect;
            }
            else
            {
                ContentRect = SegmentCore(RenderRect.IntersectWith(visiableRect));
                ControlRect = _isHitTestVisible
                    ? AnalyseCore(ContentRect)
                    : new Rect(visiableRect.x, visiableRect.y, 0f, 0f);
            }

            ContentSize = ContentRect;
            ControlSize = ControlRect;

            _isSegmentValid = true;

            return ContentRect;
        }

        /// <summary>
        /// 设置控件的父控件
        /// </summary>
        /// <param name="parent">设置给控件的父控件</param>
        public void SetParent(Visual parent)
        {
            if (ReferenceEquals(Parent, parent)
                || ReferenceEquals(this, parent))
            {
                return;
            }

            if (parent is null)
            {
                IsChild = false;
                LayoutManager = null;
                Parent = null;
                Rank = 0;
            }
            else
            {
                IsChild = true;
                LayoutManager = parent._layoutManager;
                Parent = parent;
                Rank = parent.Rank + 1;
            }

            foreach (var child in LogicalChildren)
            {
                child.LayoutManager = _layoutManager;
                child.Rank = child.Parent.Rank + 1;
            }
        }

        /// <summary>
        /// 设置当前控件的父控件
        /// </summary>
        /// <param name="parent">设置给控件的父控件</param>
        /// <remarks>这个方法应只用于初始化控件。</remarks>
        public void SetParentSilently(Visual parent)
        {
            if (ReferenceEquals(Parent, parent)
                || ReferenceEquals(this, parent))
            {
                return;
            }

            if (parent is null)
            {
                IsChild = false;
                LayoutManager = null;
                Parent = null;
            }
            else
            {
                IsChild = true;
                LayoutManager = parent._layoutManager;
                Parent = parent;
            }
        }

        /// <summary>
        /// 获取该控件的字符串表示形式
        /// </summary>
        /// <returns>该控件的字符串表示形式.</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(_name)
                ? Type.ToString()
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
        /// 分析控件结构并计算响应用户交互的区域
        /// </summary>
        /// <param name="contentRect">控件呈现内容的可见区域</param>
        /// <returns>控件响应用户交互的区域。</returns>
        protected virtual Rect AnalyseCore(Rect contentRect)
        {
            return contentRect;
        }

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
        /// 绘制控件内部不是由 <see cref="Visual"/> 控制的可交互区域边界
        /// </summary>
        protected virtual void DrawInnerControlRect()
        {
        }

        /// <summary>
        /// 计算呈现控件内容需要的尺寸
        /// </summary>
        /// <param name="availableSize">分配给控件的可用空间</param>
        /// <returns>呈现控件内容需要的尺寸。</returns>
        protected virtual Size MeasureCore(Size availableSize) => availableSize;

        /// <summary>
        /// 计算该控件呈现内容的可见区域
        /// </summary>
        /// <param name="visiableRect">允许控件显示的区域</param>
        /// <returns>该控件呈现内容的可见区域。</returns>
        protected virtual Rect SegmentCore(Rect visiableRect)
        {
            return visiableRect;
        }

        /// <summary>
        /// 获取逻辑子控件
        /// </summary>
        /// <returns>与该控件直接关联的逻辑子控件。</returns>
        protected internal virtual IEnumerable<Visual> EnumerateLogicalChildren()
        {
            yield break;
        }

        #endregion
    }
}
