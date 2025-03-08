using Nebulae.RimWorld.UI.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 边框控件，用于装饰控件
    /// </summary>
    public class Border : Control
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Texture2D _background = BrushUtility.Transparent;
        private Texture2D _borderBrush = BrushUtility.Transparent;

        private Thickness _borderThickness = Thickness.Empty;

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
        /// 背景画刷
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
        /// 边框画刷
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

        /// <summary>
        /// 内容控件
        /// </summary>
        public Visual Content
        {
            get => _content;
            set
            {
                if (!ReferenceEquals(_content, value))
                {
                    InvalidateMeasure();

                    if (!_isEmpty)
                    {
                        _content.SetParent(null);
                    }

                    _content = value;
                    _isEmpty = value is null;

                    if (_isEmpty)
                    {
                        return;
                    }

                    _content.SetParent(this);
                }
            }
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="Border"/> 的新实例
        /// </summary>
        public Border()
        {
            IsHitTestVisible = true;
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
                return availableRect;
            }

            return _content.Arrange(availableRect - _borderThickness) + _borderThickness;
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            GUI.DrawTexture(RenderRect, _background);

            float x = RenderRect.x;
            float y = RenderRect.y;
            float width = RenderRect.width;
            float height = RenderRect.height;

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

            if (!_isEmpty)
            {
                _content.Draw();
            }
        }

        /// <inheritdoc/>
        protected internal override IEnumerable<Visual> EnumerateLogicalChildren()
        {
            if (_isEmpty)
            {
                yield break;
            }

            yield return _content;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            if (_isEmpty)
            {
                return availableSize;
            }

            return _content.Measure(availableSize - _borderThickness) + _borderThickness;
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            if (_isEmpty)
            {
                return visiableRect;
            }

            return _content.Segment(visiableRect - _borderThickness) + _borderThickness;
        }

        #endregion
    }
}
