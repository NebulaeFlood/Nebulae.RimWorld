using Nebulae.RimWorld.UI.Controls.Resources;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// <see cref="TabControl"/> 的可选项
    /// </summary>
    public sealed class TabItem : ContentControl
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置 <see cref="TabItem"/> 的标题
        /// </summary>
        public string Header
        {
            get => _header;
            set
            {
                if (_header != value)
                {
                    _header = value ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// 获取一个值，该值表示 <see cref="TabItem"/> 是否已被选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            internal set => _isSelected = value;
        }

        #endregion


        static TabItem()
        {
            ButtonBase.ClickEvent.RegisterClassHandler(typeof(TabItem), new RoutedEventHandler(OnClick));
        }

        /// <summary>
        /// 初始化 <see cref="TabItem"/> 的新实例
        /// </summary>
        public TabItem()
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
        protected override Rect ArrangeOverride(Rect availableRect)
        {
            _leftRect = new Rect(availableRect.x, availableRect.y, 30f, RenderSize.Height);

            _rightRect = new Rect(availableRect.xMax - 30f, availableRect.y, 30f, RenderSize.Height);

            _midRect = new Rect(availableRect.x + 29f, availableRect.y, RenderSize.Width - 58f, RenderSize.Height);
            _midUVRect = new Rect(30f, 0f, 4f, TabItemResources.HeaderAtlas.height)
                .ToUVRect(new Vector2(TabItemResources.HeaderAtlas.width, TabItemResources.HeaderAtlas.height));

            _bottomRect = new Rect(availableRect.x, availableRect.yMax - 1f, RenderSize.Width, 1f);

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize) => availableSize;

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect) => visiableRect.IntersectWith(RenderRect);

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(Vector2 hitPoint) => HitTestResult.HitTest(this, hitPoint);

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            Rect labelRect;

            if (states.HasState(ControlState.Disabled))
            {
                labelRect = RenderRect;
                GUI.contentColor *= Widgets.InactiveColor;
            }
            else if (_isSelected || states.HasState(ControlState.Pressing))
            {
                labelRect = RenderRect;
                GUI.contentColor *= Color.yellow;
            }
            else if (states.HasState(ControlState.CursorDirectlyOver))
            {
                labelRect = new Rect(RenderRect.x + 1f, RenderRect.y, RenderSize.Width, RenderSize.Height);
                GUI.contentColor *= Color.yellow;
            }
            else
            {
                labelRect = RenderRect;
            }

            Widgets.DrawTexturePart(_leftRect, LeftUVRect, TabItemResources.HeaderAtlas);
            Widgets.DrawTexturePart(_midRect, _midUVRect, TabItemResources.HeaderAtlas);
            Widgets.DrawTexturePart(_rightRect, RightUVRect, TabItemResources.HeaderAtlas);

            if (!_isSelected)
            {
                Widgets.DrawTexturePart(_bottomRect, BottomUVRect, TabItemResources.HeaderAtlas);
            }

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;

            Widgets.Label(labelRect, _header);

            Text.Anchor = anchor;
        }

        /// <inheritdoc/>
        protected override void OnMouseEnter(RoutedEventArgs e)
        {
            if (!_isSelected)
            {
                SoundDefOf.Mouseover_Tab.PlayOneShotOnCamera();
            }
        }

        #endregion


        private static void OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is TabItem tabItem && !tabItem._isSelected)
            {
                SoundDefOf.RowTabSelect.PlayOneShotOnCamera();

                if (tabItem.Parent is TabControl tabControl)
                {
                    tabControl.Select(tabItem);
                }

                e.Handled = true;
            }
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly Rect LeftUVRect = new Rect(0f, 0f, 0.46875f, 1f);
        private static readonly Rect RightUVRect = new Rect(0.53125f, 0f, 0.46875f, 1f);
        private static readonly Rect BottomUVRect = new Rect(0.5f, 0.01f, 0.01f, 0.01f);

        // Cache

        private Rect _leftRect;

        private Rect _midRect;
        private Rect _midUVRect;

        private Rect _rightRect;

        private Rect _bottomRect;


        private string _header = string.Empty;
        private bool _isSelected;

        #endregion
    }
}
