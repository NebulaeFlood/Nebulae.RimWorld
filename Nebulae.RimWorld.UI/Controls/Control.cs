using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.UI.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 调试内容
    /// </summary>
    [Flags]
    public enum DebugContent : int
    {
        /// <summary>
        /// 不绘制内容
        /// </summary>
        Empty = 0b0000,

        /// <summary>
        /// 绘制 <see cref="Control.ContentRect"/>
        /// </summary>
        ContentRect = 0b0001,

        /// <summary>
        /// 绘制 <see cref="Control.DesiredRect"/>
        /// </summary>
        DesiredRect = 0b0010,

        /// <summary>
        /// 绘制 <see cref="Control.RenderRect"/>
        /// </summary>
        RenderRect = 0b0100,

        /// <summary>
        /// 绘制按钮的可交互区域
        /// </summary>
        HitTestRect = 0b1000
    }


    /// <summary>
    /// 所有控件的基类，定义了控件的共同特性
    /// </summary>
    public abstract class Control : DependencyObject
    {
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
        /// 控件是否为其他控件的逻辑子控件
        /// </summary>
        internal bool IsChild = false;

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

        private bool _isIndependent = true;

        private bool _isMeasureValid = false;
        private bool _isSegmentValid = false;

        private bool _shouldShowTooltip = false;
        private bool _showTooltip = false;
        private TipSignal _tooltip = string.Empty;

        private ControlWindow _owner;

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
                if (!_isArrangeValid || _isIndependent)
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
            set
            {
                if (_isIndependent)
                {
                    _isIndependent = value;
                }
                else
                {
                    SetParent(null);
                }
            }
        }

        /// <summary>
        /// 控件度量是否有效
        /// </summary>
        public bool IsMeasureValid
        {
            get
            {
                if (!_isMeasureValid || _isIndependent)
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
                if (!_isSegmentValid || _isIndependent)
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

        /// <summary>
        /// 控件的所有逻辑子控件
        /// </summary>
        public IEnumerable<Control> LogicalChildren
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
        /// 负责呈现控件的 <see cref="ControlWindow"/>
        /// </summary>
        /// <remarks>当 <see cref="IsIndependent"/> 为 <see langword="true"/> 时必定为 <see langword="null"/>。</remarks>
        public ControlWindow Owner
        {
            get
            {
                if (_isIndependent)
                {
                    return null;
                }

                if (_owner != null)
                {
                    return _owner;
                }

                if (!IsChild)
                {
                    new LogicalTreeException(this, $"Cannot find this control in any logical tree while {Type}.IsIndependent is false.");
                }

                _owner = Parent.Owner;
                return _owner;
            }
            internal set
            {
                _isIndependent = value is null;
                _owner = value;
            }
        }

        #region Padding
        /// <summary>
        /// 获取或设置控件内容的内部边距
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Padding"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(Control),
                new ControlPropertyMetadata(Thickness.Empty, ControlRelation.Measure));
        #endregion

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


#if DEBUG
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        ~Control()
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                System.Diagnostics.Debug.WriteLine($"[NebulaeFlood's Lib] A control of type {Type} has been collected.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[NebulaeFlood's Lib] A control named {_name} of type {Type} has been collected.");
            }
        }
#endif


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
                Thickness padding = Padding;
                RenderRect = ArrangeCore(availableRect - (margin + padding)).Rounded() + padding;
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
        /// 绘制调试内容
        /// </summary>
        /// <param name="content">要绘制的调试内容</param>
        public void DebugDraw(DebugContent content)
        {
            if (content is DebugContent.Empty)
            {
                return;
            }

            if (content.HasFlag(DebugContent.RenderRect) && RenderSize != 0f)
            {
                UIUtility.DrawBorder(RenderRect, UIUtility.RederRectBorderColor);
            }

            if (content.HasFlag(DebugContent.ContentRect) && ContentSize != 0f)
            {
                UIUtility.DrawBorder(ContentRect, UIUtility.ContentRectBorderColor);
            }

            if (content.HasFlag(DebugContent.DesiredRect) && DesiredSize != 0f)
            {
                UIUtility.DrawBorder(DesiredRect, UIUtility.DesiredRectBorderColor);
            }

            OnDebugDraw(content);
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
                TooltipHandler.TipRegion(ContentRect, _tooltip);
            }

            if (Prefs.DevMode && !_isIndependent)
            {
                DebugDraw(Owner.GetDebugContent());
            }
        }

        /// <summary>
        /// 获取控件的父控件
        /// </summary>
        /// <returns>控件的父控件。</returns>
        public Control GetParent()
        {
            return Parent;
        }

        /// <summary>
        /// 无效化控件排布
        /// </summary>
        /// <exception cref="InvalidOperationException">当一个不是 <see cref="ControlWindow"/> 的 <see cref="ControlWindow.Content"/> 的控件或其子控件尝试无效化排布时发生。</exception>
        public void InvalidateArrange()
        {
            if (!_isArrangeValid)
            {
                return;
            }

            _isArrangeValid = false;
            _isSegmentValid = false;

            if (_isIndependent)
            {
                return;
            }

            Owner.LayoutManager.InvalidateArrange(this);
        }

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        /// <exception cref="InvalidOperationException">当一个不是 <see cref="ControlWindow"/> 的 <see cref="ControlWindow.Content"/> 的控件或其子控件尝试无效化度量时发生。</exception>
        public void InvalidateMeasure()
        {
            if (!_isMeasureValid)
            {
                return;
            }

            _isArrangeValid = false;
            _isMeasureValid = false;
            _isSegmentValid = false;

            if (_isIndependent)
            {
                return;
            }

            Owner.LayoutManager.InvalidateMeasure(this);
        }

        /// <summary>
        /// 无效化控件分割
        /// </summary>
        /// <remarks>控件分割：由该控件控制呈现的可见区域。</remarks>
        /// <exception cref="InvalidOperationException">当一个不是 <see cref="ControlWindow"/> 的 <see cref="ControlWindow.Content"/> 的控件或其子控件尝试无效化分割时发生。</exception>
        public void InvalidateSegment()
        {
            if (!_isSegmentValid)
            {
                return;
            }

            _isSegmentValid = false;

            if (_isIndependent)
            {
                return;
            }

            Owner.LayoutManager.InvalidateSegment(this);
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
                Thickness padding = Padding;
                RenderSize = MeasureCore(availableSize - (margin + padding)).Round() + padding;
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
        /// 设置控件的父控件
        /// </summary>
        /// <param name="parent">设置给控件的父控件</param>
        public void SetParent(Control parent)
        {
            if (ReferenceEquals(Parent, parent))
            {
                return;
            }

            if (parent is null)
            {
                IsChild = false;
                Owner = null;
                Parent = null;
                Rank = 0;
            }
            else
            {
                IsChild = true;
                Owner = parent.Owner;
                Parent = parent;
                Rank = parent.Rank + 1;
            }

            foreach (var child in LogicalChildren)
            {
                child.Owner = _owner;
                child.Rank = child.Parent.Rank + 1;
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
        /// 获取逻辑子控件
        /// </summary>
        /// <returns>与该控件直接关联的逻辑子控件。</returns>
        protected internal virtual IEnumerable<Control> EnumerateLogicalChildren()
        {
            yield break;
        }

        /// <summary>
        /// 计算呈现控件内容需要的尺寸
        /// </summary>
        /// <param name="availableSize">分配给控件的可用空间</param>
        /// <returns>呈现控件内容需要的尺寸。</returns>
        protected virtual Size MeasureCore(Size availableSize) => availableSize;

        /// <summary>
        /// 当 <see cref="DebugDraw"/> 被调用时执行的函数
        /// </summary>
        /// <param name="content">要绘制的调试内容</param>
        protected virtual void OnDebugDraw(DebugContent content)
        {
        }

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
    }
}
