﻿using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 拓展器控件
    /// </summary>
    public class Expander : ButtonBase
    {
        #region Collapsed 

        private readonly WeakEvent<Expander, Control> _collapsed = new WeakEvent<Expander, Control>();

        /// <summary>
        /// 收起拓展内容时发生的弱事件
        /// </summary>
        public event Action<Expander, Control> Collapsed
        {
            add => _collapsed.Add(value, value.Invoke);
            remove => _collapsed.Remove(value);
        }

        #endregion

        #region Expanded 

        private readonly WeakEvent<Expander, Control> _expanded = new WeakEvent<Expander, Control>();

        /// <summary>
        /// 展开拓展内容时发生的弱事件
        /// </summary>
        public event Action<Expander, Control> Expanded
        {
            add => _expanded.Add(value, value.Invoke);
            remove => _expanded.Remove(value);
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
        private bool _isExpanded = false;

        private Rect _expandButtonRect;
        private Rect _labelRect;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 内容控件
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
                        _content.SetParent(null);
                    }

                    _content = value;
                    _isEmpty = value is null;

                    if (_isEmpty)
                    {
                        return;
                    }

                    _content.SetParent(this);
                    _content.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 内容是否可见
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    InvalidateMeasure();
                }
            }
        }

        #endregion


        static Expander()
        {
            HeightProperty.OverrideMetadata(typeof(Expander),
                new ControlPropertyMetadata(20f, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(Expander),
                new ControlPropertyMetadata(VerticalAlignment.Top, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="Expander"/> 的新实例
        /// </summary>
        public Expander()
        {
            ClickSound = SoundDefOf.Tick_Tiny;
            IsHitTestVisible = true;
            PlayCursorOverSound = false;
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
            float buttonHeight = Height;
            Rect renderRect = base.ArrangeCore(availableRect);

            if (_isEmpty)
            {
                _labelRect = new Rect(renderRect.x + 6f, renderRect.y, renderRect.width, buttonHeight);

                return renderRect;
            }

            _expandButtonRect = new Size(18f).AlignToArea(new Rect(renderRect.x, renderRect.y, renderRect.width, buttonHeight), HorizontalAlignment.Left, VerticalAlignment.Center);
            _labelRect = new Rect(_expandButtonRect.xMax + 2f, renderRect.y, renderRect.width - 20f, buttonHeight);

            if (!_isExpanded)
            {
                renderRect.height = buttonHeight;
                return renderRect;
            }

            renderRect.height = _content.Arrange(new Rect(
                renderRect.x + 20f,
                renderRect.y + buttonHeight,
                renderRect.width - 20f,
               float.PositiveInfinity)).height + buttonHeight;

            return renderRect;
        }


        /// <inheritdoc/>
        protected override void DrawButton(ButtonStatus status)
        {
            Color color = GUI.color;

            bool isEnabled = !status.HasFlag(ButtonStatus.Disabled);

            if (!isEnabled)
            {
                GUI.color = Widgets.InactiveColor * color;
            }
            else if (status.HasFlag(ButtonStatus.Hovered))
            {
                GUI.DrawTexture(_labelRect, TexUI.HighlightTex);
            }

            TextAnchor anchor = GameText.Anchor;
            GameFont font = GameText.Font;

            GameText.Anchor = TextAnchor.MiddleLeft;
            GameText.Font = FontSize;
            GUI.Label(_labelRect, Text);
            GameText.Anchor = anchor;
            GameText.Font = font;

            if (_isEmpty)
            {
                return;
            }

            if (_isExpanded)
            {
                if (Widgets.ButtonImage(_expandButtonRect, TexButton.Collapse)
                    && isEnabled)
                {
                    IsExpanded = false;
                    _collapsed.Invoke(this, _content);
                    SoundDefOf.TabClose.PlayOneShotOnCamera();
                }

                _content.Draw();
            }
            else
            {
                if (Widgets.ButtonImage(_expandButtonRect, TexButton.Reveal)
                    && isEnabled)
                {
                    IsExpanded = true;
                    _expanded.Invoke(this, _content);
                    SoundDefOf.TabOpen.PlayOneShotOnCamera();
                }
            }
        }

        /// <inheritdoc/>
        protected override void DrawInnerControlRect()
        {
            UIUtility.DrawBorder(_expandButtonRect, UIUtility.ControlRectBorderBrush);
        }

        /// <inheritdoc/>
        protected internal override IEnumerable<Control> EnumerateLogicalChildren()
        {
            if (_isEmpty)
            {
                yield break;
            }

            yield return _content;
        }

        /// <inheritdoc/>
        protected override Rect HitTestCore(Rect contentRect)
        {
            return _labelRect.IntersectWith(contentRect);
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size renderSize = base.MeasureCore(availableSize);
            float height = Height;

            if (_isEmpty || !_isExpanded)
            {
                return new Size(renderSize.Width, height);
            }

            return new Size(renderSize.Width, _content.Measure(new Size(renderSize.Width - 20f, float.PositiveInfinity)).Height + height);
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            if (!_isEmpty)
            {
                _content.Segment(visiableRect);
            }

            return visiableRect;
        }

        #endregion
    }
}
