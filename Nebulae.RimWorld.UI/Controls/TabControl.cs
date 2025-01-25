using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 选项卡控件
    /// </summary>
    public class TabControl : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private List<TabItem> _tabItems = new List<TabItem>(1);

        private TabItem _selectedTabItem;
        private Control _content;

        private readonly TabPanel _tabPanel;

        private Rect _backgroundRect;
        private static readonly Rect _backgroundUVRect = new Rect(0.49f, 0.49f, 0.02f, 0.02f);

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 选项卡的统一高度
        /// </summary>
        public float TabHeight
        {
            get => _tabPanel.ItemHeight;
            set => _tabPanel.ItemHeight = value;
        }

        /// <summary>
        /// 选项卡的统一宽度
        /// </summary>
        public float TabWidth
        {
            get => _tabPanel.ItemWidth;
            set => _tabPanel.ItemWidth = value;
        }

        #endregion


        static TabControl()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(TabControl),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(TabControl),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
        }


        /// <summary>
        /// 初始化 <see cref="TabControl"/> 的新实例
        /// </summary>
        public TabControl()
        {
            _tabPanel = new TabPanel
            {
                VerticalAlignment = VerticalAlignment.Top
            };
            _tabPanel.SetParent(this);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 追加选项卡
        /// </summary>
        public void Append(TabItem item)
        {
            if (item is null
                || _tabItems.Contains(item))
            {
                return;
            }

            item.Container = this;

            if (_selectedTabItem is null)
            {
                Select(item);
            }

            _tabItems.Add(item);

            InvalidateMeasure();
        }

        /// <summary>
        /// 清空选项卡
        /// </summary>
        public void Clear()
        {
            if (_tabItems.Count < 1)
            {
                return;
            }

            _selectedTabItem = null;
            _content = null;

            for (int i = _tabItems.Count; i >= 0; i--)
            {
                _tabItems[i].Container = null;

                _tabItems.RemoveAt(i);
            }

            _tabPanel.Clear();

            InvalidateMeasure();
        }

        /// <summary>
        /// 移除选项卡
        /// </summary>
        ///<param name="item">要移除的选项卡</param>
        public void Remove(TabItem item)
        {
            if (item is null)
            {
                return;
            }

            item.Container = null;

            _tabItems.Remove(item);
            _tabPanel.Remove(item);

            if (ReferenceEquals(_selectedTabItem, item))
            {
                if (_tabItems.Count > 0)
                {
                    Select(_tabItems[0]);
                }
                else
                {
                    _selectedTabItem = null;
                    _content = null;
                }
            }

            InvalidateMeasure();
        }

        /// <summary>
        /// 选择选项卡
        /// </summary>
        /// <param name="item">要选择的选项卡</param>-->
        public void Select(TabItem item)
        {
            if (item != null
                && _tabItems.Contains(item))
            {
                if (_selectedTabItem != null)
                {
                    _selectedTabItem.Selected = false;
                }

                _selectedTabItem = item;
                _content = item.Content;

                item.Selected = true;

                _tabPanel.InvalidateDrawableChildren();
            }
        }

        /// <summary>
        /// 设置选项卡
        /// </summary>
        /// <param name="tabItems">要设置的选项卡</param>
        /// <returns>该选项卡控件。</returns>
        public TabControl Set(params TabItem[] tabItems)
        {
            Clear();

            _tabItems = new List<TabItem>(tabItems.Where(x => x != null).Distinct());
            _tabPanel.Set(tabItems);

            if (_tabItems.Count > 0)
            {
                _tabItems.ForEach(x => { x.Container = this; });

                Select(_tabItems[0]);
            }

            return this;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect renderRect = base.ArrangeCore(availableRect);

            Rect tabPanelRect = _tabPanel.Arrange(renderRect);

            Rect contentRect = new Rect(
                renderRect.x,
                tabPanelRect.yMax,
                renderRect.width,
                Mathf.Max(0f, renderRect.height - tabPanelRect.height));

            _tabItems.ForEach(x => x.Content?.Arrange(contentRect));

            if (_tabItems.Count > 0)
            {
                _backgroundRect = new Rect(
                    renderRect.x,
                    _tabItems[0].RenderRect.yMax - 1f,
                    renderRect.width,
                    renderRect.height - _tabPanel.ItemHeight + 1f);
            }
            else
            {
                _backgroundRect = renderRect;
            }

            return renderRect;
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            Widgets.DrawTexturePart(_backgroundRect, _backgroundUVRect, TabItem.TabAtlas);

            Color color = GUI.color;
            GUI.color = Color.gray;

            // Left
            GUI.DrawTexture(new Rect(_backgroundRect.x, _backgroundRect.y, 1f, _backgroundRect.height), BaseContent.WhiteTex);
            // Top
            GUI.DrawTexture(new Rect(_backgroundRect.x, _backgroundRect.y, _backgroundRect.width, 1f), BaseContent.WhiteTex);
            // Right
            GUI.DrawTexture(new Rect(_backgroundRect.xMax - 1f, _backgroundRect.y, 1f, _backgroundRect.height), BaseContent.WhiteTex);
            // Bottom
            GUI.DrawTexture(new Rect(_backgroundRect.x, _backgroundRect.yMax - 1f, _backgroundRect.width, 1f), BaseContent.WhiteTex);

            GUI.color = color;

            _tabPanel.Draw();
            _selectedTabItem?.Draw();
            _content?.Draw();
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size renderSize = base.MeasureCore(availableSize);

            Size tabPanelSize = _tabPanel.Measure(renderSize);

            Size contentSize = new Size(
                renderSize.Width,
                Mathf.Max(0f, renderSize.Height - tabPanelSize.Height));

            _tabItems.ForEach(x => x.Content?.Measure(contentSize));

            return renderSize;
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            visiableRect = base.SegmentCore(visiableRect);

            _tabPanel.Segment(visiableRect);
            _tabItems.ForEach(x => x.Content?.Segment(visiableRect));

            return visiableRect;
        }

        #endregion
    }
}
