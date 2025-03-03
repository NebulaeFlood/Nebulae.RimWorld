using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 包含多个选项卡的控件
    /// </summary>
    public class TabControl : FrameworkControl
    {
        /// <summary>
        /// 选项卡之间重叠的宽度
        /// </summary>
        public const float IntersectedWidth = 8f;

        /// <summary>
        /// 每行选项卡之间的间隔
        /// </summary>
        public const float RowMargin = 4f;


        #region Private Fields

        private List<TabItem> _tabItems = new List<TabItem>(1);
        private TabItem[] _renderedItems;
        private Rect[] _renderedLines;

        private TabItem _selectedTabItem;
        private Control _selectedTabContent;

        private Rect _backgroundRect;
        private static readonly Rect _backgroundUVRect = new Rect(0.49f, 0.49f, 0.02f, 0.02f);

        private Size _tabPanelSize;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        #region TabHeight
        /// <summary>
        /// 获取或设置选项卡的统一高度
        /// </summary>
        public float TabHeight
        {
            get { return (float)GetValue(TabHeightProperty); }
            set { SetValue(TabHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TabHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TabHeightProperty =
            DependencyProperty.Register(nameof(TabHeight), typeof(float), typeof(TabControl),
                new ControlPropertyMetadata(30f, ControlRelation.Measure));
        #endregion

        #region TabWidth
        /// <summary>
        /// 获取或设置选项卡的统一宽度
        /// </summary>
        public float TabWidth
        {
            get { return (float)GetValue(TabWidthProperty); }
            set { SetValue(TabWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TabWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TabWidthProperty =
            DependencyProperty.Register(nameof(TabWidth), typeof(float), typeof(TabControl),
                 new ControlPropertyMetadata(190f, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="TabControl"/> 的新实例
        /// </summary>
        public TabControl()
        {
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
            if (item is null || _tabItems.Contains(item))
            {
                return;
            }

            item.Container = this;

            if (_selectedTabItem is null)
            {
                Select(item);
            }
            else
            {
                item.Selected = false;
            }

            _tabItems.Add(item);
            InvalidateMeasure();
        }

        /// <summary>
        /// 移除所有选项卡
        /// </summary>
        public void Clear()
        {
            if (_tabItems.Count < 1)
            {
                return;
            }

            _selectedTabItem = null;
            _selectedTabContent = null;

            for (int i = _tabItems.Count; i >= 0; i--)
            {
                _tabItems[i].Container = null;

                _tabItems.RemoveAt(i);
            }

            _tabItems.Clear();

            InvalidateMeasure();
        }

        /// <summary>
        /// 插入选项卡
        /// </summary>
        /// <param name="item">要插入的选项卡</param>
        /// <param name="index">被挤开的选项卡</param>
        public void Insert(TabItem item, TabItem index)
        {
            if (item is null || index is null)
            {
                return;
            }

            int i = _tabItems.IndexOf(index);

            if (i < 0)
            {
                return;
            }

            if (!_tabItems.Remove(item))
            {
                item.Container = this;
            }

            _tabItems.Insert(i, index);

            InvalidateMeasure();
        }

        /// <summary>
        /// 移除指定选项卡
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

            if (ReferenceEquals(_selectedTabItem, item))
            {
                if (_tabItems.Count > 0)
                {
                    Select(_tabItems[0]);
                }
                else
                {
                    _selectedTabItem = null;
                    _selectedTabContent = null;
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
                _selectedTabContent = item.Content;

                item.Selected = true;

                int index = 0;
                int tabCount = _tabItems.Count;
                _renderedItems = new TabItem[tabCount];

                for (int i = 0; i < _tabItems.Count; i++)
                {
                    var tab = _tabItems[i];

                    if (!tab.Selected)
                    {
                        _renderedItems[index++] = tab;
                    }
                }

                _renderedItems[tabCount - 1] = item;
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

            if (_tabItems.Count > 0)
            {
                for (int i = 0; i < _tabItems.Count; i++)
                {
                    var tab = _tabItems[i];
                    tab.Container = this;
                    tab.Selected = false;
                }

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

            int tabCount = _tabItems.Count;

            if (tabCount < 1)
            {
                _backgroundRect = renderRect;
                return renderRect;
            }

            // Prepare

            float tabHeight = TabHeight;
            float tabWidth = TabWidth;

            float currentX = 0f;
            float currentY = 0f;

            Size tabSize = new Size(
                tabWidth > 1f
                    ? Mathf.Min(tabWidth, renderRect.width)
                    : tabWidth * renderRect.width,
                tabHeight > 1f
                    ? tabHeight
                    : tabHeight * renderRect.width);
            Rect tabContentRect = new Rect(
                renderRect.x,
                renderRect.y + _tabPanelSize.Height,
                renderRect.width,
                Mathf.Max(0f, renderRect.height - _tabPanelSize.Height));

            // Arrange tabs and save render order

            _renderedItems = new TabItem[tabCount];
            int index = 0;
            int rowIndex = 0;

            for (int i = 0; i < tabCount; i++)
            {
                var tab = _tabItems[i];

            ArrangeStart:
                if (currentX + tabSize.Width > RenderSize.Width)
                {
                    currentX = 0f;
                    currentY += tabSize.Height + RowMargin;   // 换行

                    _renderedLines[rowIndex++] = new Rect(
                        renderRect.x,
                        renderRect.y + currentY + tabSize.Height - 1f,
                        RenderSize.Width,
                        1f);

                    goto ArrangeStart;  // 重新排列该控件
                }
                else
                {
                    if (currentX > 0f)
                    {
                        currentX -= IntersectedWidth;
                    }

                    tab.Arrange(new Rect(
                        currentX + availableRect.x,
                        currentY + availableRect.y,
                        tabSize.Width,
                        tabSize.Height));

                    currentX += tabSize.Width;
                }

                tab.Content?.Arrange(tabContentRect);

                if (!tab.Selected)
                {
                    _renderedItems[index++] = tab;
                }
            }

            _renderedItems[tabCount - 1] = _selectedTabItem;

            _backgroundRect = new Rect(
                renderRect.x,
                _tabItems[0].RenderRect.yMax - 1f,
                renderRect.width,
                renderRect.height - tabHeight + 1f);

            return renderRect;
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            Widgets.DrawTexturePart(_backgroundRect, _backgroundUVRect, TabItem.TabAtlas);
            UIUtility.DrawBorder(_backgroundRect, BrushUtility.Grey);
            UIUtility.DrawLines(BrushUtility.Grey, _renderedLines);

            for (int i = 0; i < _renderedItems.Length; i++)
            {
                _renderedItems[i].Draw();
            }

            _selectedTabContent?.Draw();
        }

        /// <inheritdoc/>
        protected internal override IEnumerable<Control> EnumerateLogicalChildren()
        {
            for (int i = 0; i < _tabItems.Count; i++)
            {
                yield return _tabItems[i];

                if (_tabItems[i].Content is Control control)
                {
                    yield return control;
                }
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size renderSize = base.MeasureCore(availableSize);

            if (_tabItems.Count < 1)
            {
                _tabPanelSize = Size.Empty;
                return renderSize;
            }

            // Prepare

            float tabHeight = TabHeight;
            float tabWidth = TabWidth;

            tabWidth = tabWidth > 1f
                    ? Mathf.Min(tabWidth, renderSize.Width)
                    : tabWidth * renderSize.Width;

            tabHeight = tabHeight > 1f
                    ? tabHeight
                    : tabHeight * renderSize.Height;

            int tabColumnCount;
            int tabRowCount;

            // Calculate row count and column count

            float restColumnWidth = renderSize.Width - tabWidth;

            if (restColumnWidth > tabWidth - IntersectedWidth)
            {
                tabColumnCount = 1 + (int)(restColumnWidth / (tabWidth - IntersectedWidth));
            }
            else
            {
                tabColumnCount = 1;
            }

            tabRowCount = _tabItems.Count / tabColumnCount;

            if (_tabItems.Count % tabColumnCount > 0)
            {
                tabRowCount++;
            }

            _renderedLines = new Rect[tabColumnCount - 1];

            // Prepare for measure

            Size tabSize = new Size(tabWidth, tabHeight);
            _tabPanelSize = new Size(renderSize.Width, (tabHeight + 2f) * tabRowCount);
            Size tabContentSize = new Size(renderSize.Width,
                Mathf.Max(0f, renderSize.Height - _tabPanelSize.Height));

            for (int i = 0; i < _tabItems.Count; i++)
            {
                var tab = _tabItems[i];

                tab.Measure(tabSize);
                tab.Content?.Measure(tabContentSize);
            }

            return renderSize;
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            for (int i = 0; i < _tabItems.Count; i++)
            {
                var tab = _tabItems[i];

                tab.Segment(visiableRect);
                tab.Content?.Segment(visiableRect);
            }

            return visiableRect;
        }

        #endregion
    }
}
