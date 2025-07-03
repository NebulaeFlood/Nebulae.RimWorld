using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (!IsEmpty)
            {
                Content.Arrange(availableRect - _borderThickness);
            }

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!IsEmpty)
            {
                Content.Measure(availableSize - _borderThickness);
            }

            return availableSize;
        }

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            if (!IsEmpty)
            {
                Content.Segment(visiableRect);
            }

            return visiableRect.IntersectWith(RenderRect);
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            if (!ReferenceEquals(_background, BrushUtility.Transparent))
            {
                GUI.DrawTexture(RenderRect, _background, ScaleMode.StretchToFill);
            }

            float x = RenderRect.x;
            float y = RenderRect.y;
            float width = RenderSize.Width;
            float height = RenderSize.Height;

            if (_borderThickness != 0f)
            {
                // Left
                GUI.DrawTexture(new Rect(x, y, _borderThickness.Left, height), _borderBrush);
                // Top
                GUI.DrawTexture(new Rect(x, y, width, _borderThickness.Top), _borderBrush);
                // Right
                GUI.DrawTexture(new Rect(x + width - _borderThickness.Right, y, _borderThickness.Right, height), _borderBrush);
                // Bottom
                GUI.DrawTexture(new Rect(x, y + height - _borderThickness.Bottom, width, _borderThickness.Bottom), _borderBrush);
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
