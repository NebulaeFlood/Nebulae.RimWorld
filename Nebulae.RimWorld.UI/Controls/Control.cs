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
        /// 点击控件是否强制重新排布控件
        /// </summary>
        public static bool ClickForceArrange = false;

        /// <summary>
        /// 点击控件是否强制重新度量控件
        /// </summary>
        public static bool ClickForceMeasure = false;

        /// <summary>
        /// 是否绘制控件绘制区域
        /// </summary>
        public static bool DrawRegion = false;

        /// <summary>
        /// 是否绘制控件占用区域
        /// </summary>
        public static bool DrawFullRegion = false;

        /// <summary>
        /// 是否使用提示框显示控件相关信息
        /// </summary>
        public static bool ShowInfo = false;

        #endregion


        internal IFrame Container;
        internal bool IsHolded = false;


        //------------------------------------------------------
        //
        //  Privaet Fields
        //
        //------------------------------------------------------

        #region Privaet Fields

        private Rect _desiredRect;
        private Size _desiredSize = Size.Empty;

        private bool _isArrangeValid = false;
        private bool _isMeasureValid = false;

        private string _name = string.Empty;

        private Rect _renderRect;
        private Size _renderSize = Size.Empty;

        private bool _shouldShowTooltip = false;
        private bool _showTooltip = false;
        private TipSignal _tooltip = string.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 控件需要占用的布局区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Arrange(Rect)"/>。</remarks>
        public Rect DesiredRect => _desiredRect;

        /// <summary>
        /// 控件需要占用的布局尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Measure(Size)"/>。</remarks>
        public Size DesiredSize => _desiredSize;

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
                new ControlPropertyMetadata(HorizontalAlignment.Center, ControlRelation.Measure));
        #endregion

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
        /// 计算的将要绘制的区域
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Arrange(Rect)"/>。</remarks>
        public Rect RenderRect => _renderRect;

        /// <summary>
        /// 计算的将要绘制的尺寸
        /// </summary>
        /// <remarks>使用前需保证已调用过 <see cref="Measure(Size)"/>。</remarks>
        public Size RenderSize => _renderSize;

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
                new ControlPropertyMetadata(VerticalAlignment.Center, ControlRelation.Measure));
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


        //------------------------------------------------------
        //
        //  Protected Properties
        //
        //------------------------------------------------------

        #region Protected Properties

        /// <summary>
        /// 控件布局是否有效
        /// </summary>
        protected bool IsArrangeValid => _isArrangeValid;

        /// <summary>
        /// 控件度量是否有效
        /// </summary>
        protected bool IsMeasureValid => _isMeasureValid;

        #endregion


        /// <summary>
        /// 为 <see cref="Control"/> 派生类实现基本初始化
        /// </summary>
        protected Control()
        {
        }


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
        public Rect Arrange(Rect availableRect)
        {
            if (!_isMeasureValid)
            {
                Measure(availableRect);
            }

            if (_isArrangeValid)
            {
                return _desiredRect;
            }

            if (!(Visibility is Visibility.Collapsed))
            {
                _renderRect = ArrangeCore(availableRect - Margin).Rounded();
                _desiredRect = _renderRect + Margin;
            }
            else
            {
                _desiredRect = new Rect(availableRect.x, availableRect.y, 0f, 0f);
                _renderRect = _desiredRect;
            }

            _isArrangeValid = true;
            return _desiredRect;
        }

        /// <summary>
        /// 使用计算好的区域绘制控件
        /// </summary>
        /// <returns>实际绘制的区域。</returns>
        /// <remarks>必须确保调用过 <see cref="Arrange(Rect)"/>，否则会引发不期望的行为。</remarks>
        public Rect Draw()
        {
            Rect renderRect = _renderRect;

            if (Visibility is Visibility.Collapsed)
            {
                renderRect = new Rect(renderRect.x, renderRect.y, 0f, 0f);
            }
            else if (Visibility is Visibility.Visible)
            {
                renderRect = DrawCore(renderRect);
                if (_showTooltip
                    && _shouldShowTooltip)
                {
                    TooltipHandler.TipRegion(renderRect, _tooltip);
                }
            }

            if (Prefs.DevMode && renderRect.size != Vector2.zero)
            {
                if (DrawRegion)
                {
                    Widgets.DrawBox(renderRect);
                }

                if (DrawFullRegion && (!DrawRegion || Margin != 0f))
                {
                    Widgets.DrawBox(_desiredRect, lineTexture: BaseContent.YellowTex);
                }

                if (ShowInfo)
                {
                    TooltipHandler.TipRegion(DrawFullRegion ? _desiredRect : renderRect,
                        $"{this}\n" +
                        $" - CursorPos = {Event.current.mousePosition}" +
                        $" - DesiredRect = {_desiredRect}\n" +
                        $" - DesiredSize = {_desiredSize}\n" +
                        $" - RenderRect = {renderRect}\n" +
                        $" - Margin = {Margin}");
                }

                if (Event.current.type is EventType.MouseDown
                    && Mouse.IsOver(Container?.Segment().IntersectWith(renderRect) ?? renderRect))
                {
                    if (ClickForceMeasure)
                    {
                        InvalidateMeasure();
                    }
                    else if (ClickForceArrange)
                    {
                        InvalidateArrange();
                    }
                }
            }
            return renderRect;
        }

        /// <summary>
        /// 在指定区域内绘制控件
        /// </summary>
        /// <param name="renderRect">允许绘制的区域</param>
        /// <returns>实际绘制的区域。</returns>
        public Rect Draw(Rect renderRect)
        {
            if (!_isArrangeValid)
            {
                Arrange(renderRect);
            }
            else if (!_isMeasureValid)  // Arrange 方法执行时若 _isMeasureValid 为 false，将会自动调用 Measure 方法
            {
                Measure(renderRect);
            }

            return Draw();
        }

        /// <summary>
        /// 无效化控件布局
        /// </summary>
        public void InvalidateArrange()
        {
            if (_isArrangeValid)
            {
                _isArrangeValid = false;

                // 控件度量无效，就说明已经调用过 Container 的 InvalidateMeasure 方法
                if (_isMeasureValid && IsHolded)
                {
                    Container.InvalidateArrange();
                }

                OnArrangeInvalidated();
            }
        }

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        /// <remarks>这会导致控件布局也被无效化。</remarks>
        public void InvalidateMeasure()
        {
            if (_isMeasureValid)
            {
                _isMeasureValid = false;

                if (IsHolded)
                {
                    Container.InvalidateMeasure();
                }

                OnMeasureInvalidated();
            }

            if (_isArrangeValid)
            {
                InvalidateArrange();
            }
        }

        /// <summary>
        /// 计算控件需要占用的布局尺寸
        /// </summary>
        /// <param name="availableSize">分配给控件的可用空间</param>
        /// <returns>控件需要占用的布局尺寸。</returns>
        public Size Measure(Size availableSize)
        {
            if (_isMeasureValid)
            {
                return _desiredSize;
            }

            if (!(Visibility is Visibility.Collapsed))
            {
                _renderSize = MeasureCore(availableSize - Margin).Round();
                _desiredSize = _renderSize + Margin;
            }
            else
            {
                _desiredSize = Size.Empty;
                _renderSize = _desiredSize;
            }

            _isMeasureValid = true;
            return _desiredSize;
        }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString() => _name.NullOrEmpty() ? base.ToString() : _name;

        #endregion


        //------------------------------------------------------
        //
        //  Protected Method
        //
        //------------------------------------------------------

        #region Protected Method

        /// <summary>
        /// 计算呈现控件内容需要的区域
        /// </summary>
        /// <param name="availableRect">允许排布的区域</param>
        /// <returns>呈现控件内容需要的区域。</returns>
        protected virtual Rect ArrangeCore(Rect availableRect) =>
            (_desiredSize - Margin).AlignRectToArea(availableRect, HorizontalAlignment, VerticalAlignment);

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

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.Metadata is ControlPropertyMetadata metadata)
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
        /// 当控件度量被无效化后执行的操作
        /// </summary>
        protected virtual void OnMeasureInvalidated() { }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal void RemoveContainer()
        {
            Container = null;
            IsHolded = false;

            InvalidateMeasure();
        }

        internal void SetContainer(IFrame container)
        {
            Container = container;
            IsHolded = true;

            InvalidateMeasure();
        }

        #endregion
    }
}
