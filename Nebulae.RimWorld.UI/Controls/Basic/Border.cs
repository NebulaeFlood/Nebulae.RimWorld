using Nebulae.RimWorld.UI.Utilities;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 在子控件四周绘制边框和背景
    /// </summary>
    public sealed class Border : ContentControl
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置背景画刷
        /// </summary>
        public Texture2D Background
        {
            get => _background;
            set
            {
                if (value is null)
                {
                    value = BrushUtility.Transparent;
                }

                _background = value;
            }
        }

        /// <summary>
        /// 获取或设置边框画刷
        /// </summary>
        public Texture2D BorderBrush
        {
            get => _borderBrush;
            set
            {
                if (value is null)
                {
                    value = BrushUtility.Transparent;
                }

                _borderBrush = value;
            }
        }

        /// <summary>
        /// 获取或设置边框粗细
        /// </summary>
        public Thickness BorderThickness
        {
            get => _borderThickness;
            set
            {
                value = value.Normalize();

                if (_borderThickness != value)
                {
                    _borderThickness = value;

                    InvalidateMeasure();
                }
            }
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="Border"/> 的新实例
        /// </summary>
        public Border() { }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeOverride(Rect availableRect)
        {
            if (IsEmpty)
            {
                return availableRect;
            }

            return Content.Arrange(availableRect - _borderThickness) + _borderThickness;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (IsEmpty)
            {
                return availableSize;
            }

            return Content.Measure(availableSize - _borderThickness) + _borderThickness;
        }

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            visiableRect = visiableRect.IntersectWith(RenderRect);

            if (!IsEmpty)
            {
                Content.Segment(visiableRect);
            }

            return visiableRect;
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            if (!ReferenceEquals(_background, BrushUtility.Transparent))
            {
                GUI.DrawTexture(RegionRect, _background, ScaleMode.StretchToFill);
            }

            float x = RegionRect.x;
            float y = RegionRect.y;
            float width = RegionSize.Width;
            float height = RegionSize.Height;

            if (_borderThickness != 0f)
            {
                // Left
                GUI.DrawTexture(new Rect(x, y, _borderThickness.Left, height), _borderBrush, ScaleMode.StretchToFill);
                // Top
                GUI.DrawTexture(new Rect(x, y, width, _borderThickness.Top), _borderBrush, ScaleMode.StretchToFill);
                // Right
                GUI.DrawTexture(new Rect(x + width - _borderThickness.Right, y, _borderThickness.Right, height), _borderBrush, ScaleMode.StretchToFill);
                // Bottom
                GUI.DrawTexture(new Rect(x, y + height - _borderThickness.Bottom, width, _borderThickness.Bottom), _borderBrush, ScaleMode.StretchToFill);
            }

            base.DrawCore(states);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Texture2D _background = BrushUtility.Transparent;
        private Texture2D _borderBrush = BrushUtility.Transparent;

        private Thickness _borderThickness = Thickness.Empty;

        #endregion
    }
}
