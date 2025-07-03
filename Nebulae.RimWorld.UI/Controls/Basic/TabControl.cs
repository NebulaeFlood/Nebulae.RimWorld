using Nebulae.RimWorld.UI.Controls.Resources;
using Nebulae.RimWorld.UI.Core;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 包含多个项的控件
    /// </summary>
    public sealed class TabControl : FrameworkControl
    {
        /// <summary>
        ///  <see cref="TabControl"/> 的标题高度
        /// </summary>
        public const float HeaderHeight = 30f;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <inheritdoc/>
        public override IEnumerable<Control> LogicalChildren => _items;

        /// <summary>
        /// 获取 <see cref="TabControl"/> 选中的项
        /// </summary>
        public TabItem SelectedItem => _selectedItem;

        #endregion


        #region HeaderWidth
        /// <summary>
        /// 获取或设置 <see cref="TabControl"/> 的标题宽度
        /// </summary>
        /// <remarks>当值为小于等于 1 的正数时，则表示所占可用空间的百分比。</remarks>
        public float HeaderWidth
        {
            get { return (float)GetValue(HeaderWidthProperty); }
            set { SetValue(HeaderWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HeaderWidth"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty HeaderWidthProperty =
            DependencyProperty.Register(nameof(HeaderWidth), typeof(float), typeof(TabControl),
                new ControlPropertyMetadata(0.2f, CoerceHeaderWidth, ControlRelation.Measure));

        private static object CoerceHeaderWidth(DependencyObject d, object baseValue)
        {
            return Mathf.Max((float)baseValue, 0f);
        }
        #endregion


        /// <summary>
        /// 初始化 <see cref="TabControl"/> 的新实例
        /// </summary>
        public TabControl()
        {
            _items = new TabItemGroup(this);

            IsSolid = true;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 向 <see cref="TabControl"/> 添加可选项到末尾
        /// </summary>
        /// <param name="item">要添加的可选项</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 <see langword="null"/> 时发生。</exception>
        public void Append(TabItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_items.Contains(item))
            {
                return;
            }

            _items.Append(item);

            if (_selectedItem is null)
            {
                Select(item);
            }
            else
            {
                UpdateRenderQueue();
            }
        }

        /// <summary>
        /// 向 <see cref="TabControl"/> 插入可选项
        /// </summary>
        /// <param name="index">作为索引的可选项</param>
        /// <param name="item">要插入的可选项</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="index"/> 或 <paramref name="item"/> 为 <see langword="null"/> 时发生。</exception>
        public void Insert(TabItem index, TabItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (index is null)
            {
                throw new ArgumentNullException(nameof(index));
            }

            _items.Insert(index, item);
            item.InvalidateArrange();

            if (_selectedItem is null)
            {
                Select(item);
            }
            else
            {
                UpdateRenderQueue();
            }
        }

        /// <summary>
        /// 移除 <see cref="TabControl"/> 的可选项
        /// </summary>
        /// <param name="item">要移除的可选项</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当 <paramref name="item"/> 不是该 <see cref="TabControl"/> 的子控件时发生。</exception>
        public void Remove(TabItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!ReferenceEquals(item.Parent, this))
            {
                throw new InvalidOperationException($"Cannot remove {item} since it is not child of {this}.");
            }

            if (_items.IsEmpty)
            {
                return;
            }

            item.InvalidateArrange();
            _items.Remove(item);
            _items.TrySelectHead();

            if (ReferenceEquals(item, _selectedItem))
            {
                _selectedItem.IsSelected = false;
                _selectedItem = null;

                UpdateRenderQueue();
            }
        }

        /// <summary>
        /// 选中指定的可选项
        /// </summary>
        /// <param name="item">要选中的可选项</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当 <paramref name="item"/> 不是该 <see cref="TabControl"/> 的子控件时发生。</exception>
        public void Select(TabItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!ReferenceEquals(this, item.Parent))
            {
                throw new InvalidOperationException($"Cannot select {item} since it is not child of {this}.");
            }

            if (ReferenceEquals(_selectedItem, item))
            {
                return;
            }

            if (_selectedItem != null)
            {
                _selectedItem.IsSelected = false;
            }

            _selectedItem = item;
            _selectedItem.IsSelected = true;

            UpdateRenderQueue();
        }

        /// <summary>
        /// 设置 <see cref="TabControl"/> 的可选项
        /// </summary>
        /// <param name="items">要添加的可选项，应保证所有选项不重复且不为 <see langword="null"/></param>
        /// <returns>设置可选项的 <see cref="TabControl"/>。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="items"/> 为 <see langword="null"/> 时发生。</exception>
        /// <remarks>会自动选择第一个可选项。</remarks>
        public TabControl Set(params TabItem[] items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items.Set(items);
            _items.TrySelectHead();

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
        protected override Rect ArrangeOverride(Rect availableRect)
        {
            if (_items.IsEmpty)
            {
                _backgroundRect = availableRect;
                return availableRect;
            }

            float currentX = availableRect.x;
            float currentY = availableRect.y;

            _backgroundRect = new Rect(currentX, currentY + HeaderHeight - 1f, RenderSize.Width, RenderSize.Height - (HeaderHeight + 1f));

            var contentRect = new Rect(
                currentX,
                currentY + _headerPanelSize.Height + 1f,
                RenderSize.Width,
                RenderSize.Height - (_headerPanelSize.Height + 1f));

            int columnIndex = 0;

            foreach (var item in _items)
            {
            ArrangeStart:
                if (columnIndex < _columnCount)
                {
                    columnIndex++;
                    item.Arrange(new Rect(currentX, currentY, _headerWidth, HeaderHeight));
                    item.Content?.Arrange(contentRect);
                    currentX += _headerWidth - 8f;
                }
                else
                {
                    currentX = availableRect.x;
                    currentY += HeaderHeight + 2f;

                    columnIndex = 0;

                    goto ArrangeStart;
                }
            }

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_items.IsEmpty)
            {
                return availableSize;
            }

            var rawHeaderWidth = HeaderWidth;

            if (rawHeaderWidth > 1f)
            {
                _headerWidth = Mathf.Clamp(rawHeaderWidth, 2f, availableSize.Width);
                _columnCount = Mathf.FloorToInt(availableSize.Width / _headerWidth);
            }
            else
            {
                _headerWidth = rawHeaderWidth * availableSize.Width;
                _columnCount = Mathf.FloorToInt(1f / rawHeaderWidth);
            }

            _headerPanelSize = new Size(availableSize.Width, Mathf.CeilToInt(_items.Count * rawHeaderWidth) * (HeaderHeight + 2f));

            var contentSize = new Size(availableSize.Width, Mathf.Max(0f, availableSize.Height - (_headerPanelSize.Height + 1f)));
            var headerSize = new Size(_headerWidth, HeaderHeight);

            var node = _items.Head;

            while (node != null)
            {
                node.Data.Measure(headerSize);
                node.Data.Content?.Measure(contentSize);

                node = node.next;
            }

            return availableSize;
        }

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            visiableRect = visiableRect.IntersectWith(RenderRect);

            if (!_isEmpty)
            {
                for (int i = _renderQueue.Length - 1; i >= 0; i--)
                {
                    _renderQueue[i].Segment(visiableRect);
                }
            }

            return visiableRect;
        }

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(Vector2 hitPoint)
        {
            if (!ControlRect.Contains(hitPoint))
            {
                return HitTestResult.Empty;
            }

            if (!_isEmpty)
            {
                for (int i = _renderQueue.Length - 1; i >= 0; i--)
                {
                    var result = _renderQueue[i].HitTest(hitPoint);

                    if (result.IsHit)
                    {
                        return result;
                    }
                }
            }

            return HitTestResult.HitTest(this, true);
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            Widgets.DrawTexturePart(_backgroundRect, BackgroundUVRect, TabItemResources.HeaderAtlas);

            float x = _backgroundRect.x;
            float y = _backgroundRect.y;
            float width = _backgroundRect.width;
            float height = _backgroundRect.height;

            // Left
            Widgets.DrawTexturePart(new Rect(x, y, 1f, height), BottomUVRect, TabItemResources.HeaderAtlas);
            // Top
            Widgets.DrawTexturePart(new Rect(x, y, width, 1f), BottomUVRect, TabItemResources.HeaderAtlas);
            // Right
            Widgets.DrawTexturePart(new Rect(x + width - 1f, y, 1f, height), BottomUVRect, TabItemResources.HeaderAtlas);
            // Bottom
            Widgets.DrawTexturePart(new Rect(x, y + height - 1f, width, 1f), BottomUVRect, TabItemResources.HeaderAtlas);

            if (_isEmpty)
            {
                return;
            }

            for (int i = 0; i < _renderQueue.Length; i++)
            {
                _renderQueue[i].Draw();
            }
        }

        #endregion


        private void UpdateRenderQueue()
        {
            if (_items.IsEmpty)
            {
                _isEmpty = true;
                _renderQueue = Array.Empty<TabItem>();
                return;
            }

            _isEmpty = false;

            if (_selectedItem is null)
            {
                _renderQueue = _items.ToArray();
                return;
            }

            bool IsUnselected(TabItem item)
            {
                return !ReferenceEquals(item, _selectedItem);
            }

            if (_selectedItem.IsEmpty)
            {
                _renderQueue = _items
                    .Where(IsUnselected)
                    .Append(_selectedItem)
                    .ToArray();
                return;
            }

            _renderQueue = _items
                .Where(IsUnselected)
                .Append(_selectedItem)
                .Append(_selectedItem.Content)
                .ToArray();
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly Rect BackgroundUVRect = new Rect(0.49f, 0.49f, 0.02f, 0.02f);
        private static readonly Rect BottomUVRect = new Rect(0.5f, 0.01f, 0.01f, 0.01f);

        private bool _isEmpty = true;

        private readonly TabItemGroup _items;

        // Cache

        private TabItem _selectedItem;

        private int _columnCount;
        private float _headerWidth;

        private Control[] _renderQueue;

        private Rect _backgroundRect;
        private Size _headerPanelSize;

        #endregion


        private sealed class TabItemGroup : LinkedListThin<TabItem>
        {
            public TabItemGroup(TabControl owner)
            {
                _owner = owner;
            }

            public void Append(TabItem item)
            {
                item.Parent = _owner;
                AddLast(item);
            }

            public void Insert(TabItem index, TabItem item)
            {
                item.Parent = _owner;

                var inserted = false;
                var node = head;

                while (node != null)
                {
                    if (ReferenceEquals(node.Data, item))
                    {
                        inserted = true;

                        var nextNode = node.next;
                        Remove(node);
                        node = nextNode;
                    }
                    else if (!inserted && ReferenceEquals(node.Data, index))
                    {
                        Insert(node, item);
                    }
                }

                if (!inserted)
                {
                    AddLast(item);
                }
            }

            public void Remove(TabItem item)
            {
                item.Parent = null;

                var node = head;

                while (node != null)
                {
                    if (ReferenceEquals(node.Data, item))
                    {
                        Remove(node);
                        return;
                    }

                    node = node.next;
                }
            }

            public void Set(IEnumerable<TabItem> items)
            {
                var node = head;

                while (node != null)
                {
                    node.Data.Parent = null;
                }

                Clear();

                foreach (var item in items)
                {
                    if (item != null)
                    {
                        item.Parent = _owner;
                        AddLast(item);
                    }
                }
            }

            public void TrySelectHead()
            {
                if (count > 0)
                {
                    _owner.Select(head.Data);
                }
            }


            private readonly TabControl _owner;
        }
    }
}
