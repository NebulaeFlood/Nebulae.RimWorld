using Nebulae.RimWorld.UI.Data;
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
        /// 是否绘制控件绘制区域
        /// </summary>
        public static bool drawRegion = false;

        /// <summary>
        /// 是否绘制控件占用区域
        /// </summary>
        public static bool drawFullRegion = false;

        /// <summary>
        /// 是否使用提示框显示控件相关信息
        /// </summary>
        public static bool showInfo = false;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 排布控件后控件需要占用的布局区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Arrange(Rect)"/>。</remarks>
        public Rect DesiredRect { get; private set; }

        /// <summary>
        /// 控件需要占用的布局尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Measure(Size)"/>。</remarks>
        public Size DesiredSize { get; private set; }

        #region HorizontalAlignment
        /// <summary>
        /// 获取或设置控件水平对齐方式
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HorizontalAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(HorizontalAlignment), typeof(HorizontalAlignment), typeof(Control),
                new ControlPropertyMetadata(HorizontalAlignment.Center, ControlRelation.Arrange));
        #endregion

        /// <summary>
        /// 控件布局是否有效
        /// </summary>
        public bool IsArrangeValid { get; private set; }

        /// <summary>
        /// 当前布局测量是否有效
        /// </summary>
        public bool IsMeasureValid { get; private set; }

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
                new ControlPropertyMetadata(Thickness.Empty));
        #endregion

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 控件最终呈现区域
        /// </summary>
        public Rect RenderedRect { get; private set; }

        /// <summary>
        /// 控件最终呈现尺寸
        /// </summary>
        public Size RenderedSize { get; private set; }

        /// <summary>
        /// 光标悬浮于控件上时是否显示提示框
        /// </summary>
        public bool ShowTooltip { get; set; }

        /// <summary>
        /// 提示框文字
        /// </summary>
        public string Tooltip { get; set; }

        #region VerticalAlignment
        /// <summary>
        /// 获取或设置控件垂直对齐方式
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="VerticalAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(nameof(VerticalAlignment), typeof(VerticalAlignment), typeof(Control),
                new ControlPropertyMetadata(VerticalAlignment.Center, ControlRelation.Arrange));
        #endregion

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


        /// <summary>
        /// 为 <see cref="Control"/> 派生类实现基本初始化
        /// </summary>
        protected Control()
        {
            IsArrangeValid = false;
            IsMeasureValid = false;
            Name = string.Empty;
            ShowTooltip = false;
            Tooltip = string.Empty;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 排布控件
        /// </summary>
        /// <param name="availableRect">分配给控件的区域</param>
        /// <returns>控件需要占用的布局区域。</returns>
        public Rect Arrange(Rect availableRect)
        {
            if (!IsMeasureValid) { Measure(availableRect); }
            if (IsArrangeValid) { return DesiredRect; }
            if (!(Visibility is Visibility.Collapsed))
            {
                DesiredRect = (ArrangeCore(availableRect - Margin) + Margin).Rounded();
            }
            else
            {
                DesiredRect = new Rect(availableRect.x, availableRect.y, 0f, 0f);
            }
            IsArrangeValid = true;
            return DesiredRect;
        }

        /// <summary>
        /// 使用计算好的区域绘制控件
        /// </summary>
        /// <returns>实际绘制的区域。</returns>
        public Rect Draw()
        {
            Rect renderRect = DesiredRect - Margin;
            if (Visibility is Visibility.Collapsed) { renderRect = new Rect(renderRect.x, renderRect.y, 0f, 0f); }
            else if (Visibility is Visibility.Visible)
            {
                renderRect = DrawCore(renderRect);
                if (ShowTooltip && !Tooltip.NullOrEmpty())
                {
                    TooltipHandler.TipRegion(renderRect, Tooltip);
                }
            }

            if (Prefs.DevMode && renderRect.size != Vector2.zero)
            {
                if (drawRegion)
                {
                    Widgets.DrawBox(renderRect);
                }
                if (drawFullRegion && (!drawRegion || Margin != 0f))
                {
                    Widgets.DrawBox(DesiredRect, lineTexture: BaseContent.YellowTex);
                }
                if (showInfo)
                {
                    TooltipHandler.TipRegion(drawFullRegion ? DesiredRect : renderRect,
                        $"{this}\n" +
                        $" - DesiredRect = {DesiredRect}\n" +
                        $" - DesiredSize = {DesiredSize}\n" +
                        $" - RenderedRect = {renderRect}\n" +
                        $" - Margin = {Margin}");
                }
            }

            RenderedRect = renderRect;
            RenderedSize = renderRect;
            return renderRect;
        }

        /// <summary>
        /// 在指定区域内绘制控件
        /// </summary>
        /// <param name="renderRect">允许绘制的区域</param>
        /// <returns>实际绘制的区域。</returns>
        public Rect Draw(Rect renderRect)
        {
            if (!IsArrangeValid) { Arrange(renderRect); }
            else if (!IsMeasureValid) { Measure(renderRect); }
            return Draw();
        }

        /// <summary>
        /// 无效化控件布局
        /// </summary>
        public void InvalidateArrange()
        {
            IsArrangeValid = false;
            OnArrangeInvalidated();
        }

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        /// <remarks>这会导致控件布局也被无效化。</remarks>
        public void InvalidateMeasure()
        {
            IsMeasureValid = false;
            OnMeasureInvalidated();
        }

        /// <summary>
        /// 计算控件需要占用的布局尺寸
        /// </summary>
        /// <param name="availableSize">分配给控件的可用空间</param>
        /// <returns>控件需要占用的布局尺寸。</returns>
        public Size Measure(Size availableSize)
        {
            if (IsMeasureValid) { return DesiredSize; }
            if (!(Visibility is Visibility.Collapsed))
            {
                DesiredSize = (MeasureCore(availableSize - Margin) + Margin).Round();
            }
            else
            {
                DesiredSize = Size.Empty;
            }
            IsMeasureValid = true;
            return DesiredSize;
        }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString() => Name.NullOrEmpty() ? base.ToString() : Name;

        #endregion


        //------------------------------------------------------
        //
        //  Protected Method
        //
        //------------------------------------------------------

        #region Protected Method

        /// <summary>
        /// 计算控件需要占用的布局区域
        /// </summary>
        /// <param name="availableRect">允许排布的区域</param>
        /// <returns>控件需要占用的布局区域。</returns>
        protected virtual Rect ArrangeCore(Rect availableRect) =>
            (DesiredSize - Margin).AlignRectToArea(availableRect, HorizontalAlignment, VerticalAlignment);

        /// <summary>
        /// 绘制控件
        /// </summary>
        /// <param name="renderRect">允许绘制的区域</param>
        /// <returns>实际绘制的区域。</returns>
        protected abstract Rect DrawCore(Rect renderRect);

        /// <summary>
        /// 计算呈现控件内容需要的尺寸
        /// </summary>
        /// <param name="availableSize">分配给控件的可用空间</param>
        /// <returns>呈现控件内容需要的尺寸。</returns>
        protected virtual Size MeasureCore(Size availableSize) => availableSize;

        /// <summary>
        /// 当控件布局被无效化后执行的操作
        /// </summary>
        protected virtual void OnArrangeInvalidated() { }

        /// <summary>
        /// 当控件度量被无效化后执行的操作
        /// </summary>
        protected virtual void OnMeasureInvalidated()
        {
            InvalidateArrange();
        }

        #endregion
    }
}
