using Nebulae.RimWorld.UI.Controls.Composites;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// <see cref="Expander"/> 的收放按钮
    /// </summary>
    internal class ExpandButton : Control
    {
        private static readonly Size _uniformSize = new Size(18f);
        private readonly Expander _parent;


        internal bool IsExpanded;


        /// <summary>
        /// 初始化 <see cref="ExpandButton"/> 的新实例
        /// </summary>
        /// <param name="parent">作为父控件的 <see cref="Expander"/></param>
        internal ExpandButton(Expander parent)
        {
            if (parent is null)
            {
                throw new ArgumentNullException("parent");
            }

            _parent = parent;

            SetParentSilently(parent);
            IsHitTestVisible = true;
        }


        protected override Rect ArrangeCore(Rect availableRect)
        {
            return RenderSize.AlignToArea(availableRect,
                HorizontalAlignment.Left, VerticalAlignment.Center);
        }

        protected override void DrawCore()
        {
            bool isEnabled = IsEnabled;

            if (!isEnabled)
            {
                GUI.color *= Widgets.InactiveColor;
            }
            else if (IsPressing || IsCursorOver)
            {
                GUI.color *= GenUI.MouseoverColor;
            }

            if (isEnabled && IsExpanded)
            {
                GUI.DrawTexture(RenderRect, TexButton.Collapse);
            }
            else
            {
                GUI.DrawTexture(RenderRect, TexButton.Reveal);
            }
        }

        protected override Size MeasureCore(Size availableSize)
        {
            return _uniformSize;
        }

        protected internal override void OnClick()
        {
            if (IsExpanded)
            {
                IsExpanded = false;
                SoundDefOf.TabClose.PlayOneShotOnCamera();
                _parent.IsExpanded = false;
            }
            else
            {
                IsExpanded = true;
                SoundDefOf.TabOpen.PlayOneShotOnCamera();
                _parent.IsExpanded = true;
            }
        }
    }
}
