using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 将控件排布在设定好的格子内的表格面板
    /// </summary>
    public sealed class Grid : Panel
    {
        /// <summary>
        /// 表示自动尺寸
        /// </summary>
        public const float Auto = Definition.Auto;

        /// <summary>
        /// 表示自动尺寸
        /// </summary>
        public const float Remain = float.PositiveInfinity;


        //------------------------------------------------------
        //
        //  Attached Properties
        //
        //------------------------------------------------------

        #region Attached Properties

        #region Column
        /// <summary>
        /// 从指定的输入元素获取 <see cref="Grid"/>.Column 附加属性的值
        /// </summary>
        /// <param name="obj">从中读取属性值的输入元素</param>
        /// <returns><see cref="Grid"/>.Column 附加属性的值</returns>
        public static int GetColumn(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnProperty);
        }

        /// <summary>
        /// 设置指定输入元素上的 <see cref="Grid"/>.Column 附加属性的值
        /// </summary>
        /// <param name="obj">要在其上设置 <see cref="Grid"/>.Column 附加属性的元素</param>
        /// <param name="value">要设置的属性值</param>
        public static void SetColumn(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="Grid"/>.Column 附加属性。
        /// </summary>
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached("Column", typeof(int), typeof(Grid),
                new ControlPropertyMetadata(0, ControlRelation.Measure),
                ValidatePositive);
        #endregion

        #region ColumnSpan
        /// <summary>
        /// 从指定的输入元素获取 <see cref="Grid"/>.ColumnSpan 附加属性的值
        /// </summary>
        /// <param name="obj">从中读取属性值的输入元素</param>
        /// <returns><see cref="Grid"/>.ColumnSpan 附加属性的值</returns>
        public static int GetColumnSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnSpanProperty);
        }

        /// <summary>
        /// 设置指定输入元素上的 <see cref="Grid"/>.ColumnSpan 附加属性的值
        /// </summary>
        /// <param name="obj">要在其上设置 <see cref="Grid"/>.ColumnSpan 附加属性的元素</param>
        /// <param name="value">要设置的属性值</param>
        public static void SetColumnSpan(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnSpanProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="Grid"/>.ColumnSpan 附加属性。
        /// </summary>
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(Grid),
                new ControlPropertyMetadata(0, ControlRelation.Measure),
                ValidatePositive);
        #endregion

        #region Row
        /// <summary>
        /// 从指定的输入元素获取 <see cref="Grid"/>.Row 附加属性的值
        /// </summary>
        /// <param name="obj">从中读取属性值的输入元素</param>
        /// <returns><see cref="Grid"/>.Row 附加属性的值</returns>
        public static int GetRow(DependencyObject obj)
        {
            return (int)obj.GetValue(RowProperty);
        }

        /// <summary>
        /// 设置指定输入元素上的 <see cref="Grid"/>.Row 附加属性的值
        /// </summary>
        /// <param name="obj">要在其上设置 <see cref="Grid"/>.Row 附加属性的元素</param>
        /// <param name="value">要设置的属性值</param>
        public static void SetRow(DependencyObject obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="Grid"/>.Row 附加属性。
        /// </summary>
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached("Row", typeof(int), typeof(Grid),
                new ControlPropertyMetadata(0, ControlRelation.Measure),
                ValidatePositive);
        #endregion

        #region RowSpan
        /// <summary>
        /// 从指定的输入元素获取 <see cref="Grid"/>.RowSpan 附加属性的值
        /// </summary>
        /// <param name="obj">从中读取属性值的输入元素</param>
        /// <returns><see cref="Grid"/>.RowSpan 附加属性的值</returns>
        public static int GetRowSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(RowSpanProperty);
        }

        /// <summary>
        /// 设置指定输入元素上的 <see cref="Grid"/>.RowSpan 附加属性的值
        /// </summary>
        /// <param name="obj">要在其上设置 <see cref="Grid"/>.RowSpan 附加属性的元素</param>
        /// <param name="value">要设置的属性值</param>
        public static void SetRowSpan(DependencyObject obj, int value)
        {
            obj.SetValue(RowSpanProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="Grid"/>.RowSpan 附加属性。
        /// </summary>
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(Grid),
                new ControlPropertyMetadata(0, ControlRelation.Measure),
                ValidatePositive);
        #endregion

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 已定义的列数
        /// </summary>
        public int ColumnCount => _columnDefinitions.Length;

        /// <summary>
        /// 已定义的行数
        /// </summary>
        public int RowCount => _rowDefinitions.Length;

        #endregion


        /// <summary>
        /// 初始化 <see cref="Grid"/> 的新实例
        /// </summary>
        public Grid() { }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 定义表格的列的个数和尺寸
        /// </summary>
        /// <param name="columnWidths">每一列的尺寸</param>
        /// <returns>该表格面板。</returns>
        public Grid DefineColumns(params float[] columnWidths)
        {
            if (columnWidths.Length > 0)
            {
                _anyAutoColumn = false;
                _columnDefinitions = new ColumnDefinition[columnWidths.Length];
                _columnsUseRemain.Clear();

                for (int i = 0; i < columnWidths.Length; i++)
                {
                    var column = new ColumnDefinition(columnWidths[i], i);

                    if (column.IsAutoSize)
                    {
                        _anyAutoColumn = true;
                    }
                    else if (column.UseRemain)
                    {
                        _columnsUseRemain.Add(column);
                    }

                    _columnDefinitions[i] = column;
                }

                _columnsUseRemain.TrimExcess();
            }
            else
            {
                throw new ArgumentException("Must have at least one row.", nameof(columnWidths));
            }

            InvalidateMeasure();
            return this;
        }

        /// <summary>
        /// 定义表格的行的个数和尺寸
        /// </summary>
        /// <param name="rowHeights">每一行的尺寸</param>
        /// <returns>该表格面板。</returns>
        public Grid DefineRows(params float[] rowHeights)
        {
            if (rowHeights.Length > 0)
            {
                _anyAutoRow = false;
                _rowDefinitions = new RowDefinition[rowHeights.Length];
                _rowsUseRemain.Clear();

                for (int i = 0; i < rowHeights.Length; i++)
                {
                    var row = new RowDefinition(rowHeights[i], i);

                    if (row.IsAutoSize)
                    {
                        _anyAutoRow = true;
                    }
                    else if (row.UseRemain)
                    {
                        _rowsUseRemain.Add(row);
                    }

                    _rowDefinitions[i] = row;
                }

                _rowsUseRemain.TrimExcess();
            }
            else
            {
                throw new ArgumentException("Must have at least one row.", nameof(rowHeights));
            }

            InvalidateMeasure();
            return this;
        }


        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <remarks>
        /// 必须保证输入的结构能够匹配设置的行数和列数，否则会引发未知的行为。
        /// </remarks>
        public Grid Set(params Control[] controls)
        {
            Children.OverrideCollection(controls.Where(x => x != null).Distinct());

            var children = FilteredChildren;

            Control child;
            int firstIndex;
            int lastIndex;

            for (int i = 0; i < children.Length; i++)
            {
                child = children[i];
                firstIndex = Array.IndexOf(controls, child);
                lastIndex = Array.LastIndexOf(controls, child);

                if (firstIndex == lastIndex)
                {
                    SetColumn(child, firstIndex % ColumnCount);
                    SetRow(child, firstIndex / ColumnCount);
                }
                else
                {
                    int column = firstIndex % ColumnCount;
                    int row = firstIndex / ColumnCount;

                    SetColumn(child, column);
                    SetColumnSpan(child, lastIndex % ColumnCount - column);
                    SetRow(child, row);
                    SetRowSpan(child, lastIndex / ColumnCount - row);
                }
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
        protected override Rect ArrangeOverride(Rect availableRect, Control[] children)
        {
            float x = availableRect.x;
            float y = availableRect.y;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].Arrange(_unitInfos[i].Rect.Offset(x, y));
            }

            return new Rect(availableRect.x, availableRect.y, RenderSize.Width, RenderSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            var renderSize = UpdateDefinitionsSize(availableSize, children);

            UpdateUnitInfos(children);

            return renderSize;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private Size UpdateDefinitionsSize(Size availableSize, Control[] children)
        {
            float height = 0f;
            float width = 0f;

            int columnCount = _columnDefinitions.Length;
            int rowCount = _rowDefinitions.Length;

            if (_anyAutoColumn || _anyAutoRow)
            {
                SizeCache[] cachedSize = new SizeCache[children.Length];
                SizeCache cache;

                for (int i = 0; i < columnCount; i++)
                {
                    var column = _columnDefinitions[i];
                    column.Size = 0f;

                    if (column.IsAutoSize)
                    {
                        for (int j = 0; j < children.Length; j++)
                        {
                            if (GetColumn(children[j]) == i)
                            {
                                cache = cachedSize[j];

                                if (!cache.IsValid)
                                {
                                    cache = new SizeCache(children[j].Measure(new Size(float.PositiveInfinity)));
                                }

                                column.Size = Mathf.Max(column.Size, cache.Size.Width);
                            }
                        }
                    }
                    else if (column.UseRemain)
                    {
                        continue;
                    }
                    else
                    {
                        column.Size = column.LogicalSize > 1f
                            ? column.LogicalSize
                            : availableSize.Width * column.LogicalSize;
                    }

                    width += column.Size;
                }

                for (int i = 0; i < rowCount; i++)
                {
                    var row = _rowDefinitions[i];
                    row.Size = 0f;

                    if (row.IsAutoSize)
                    {
                        for (int j = 0; j < children.Length; j++)
                        {
                            if (GetRow(children[j]) == i)
                            {
                                cache = cachedSize[j];

                                if (!cache.IsValid)
                                {
                                    cache = new SizeCache(children[j].Measure(
                                        new Size(_columnDefinitions[GetColumn(children[j])].Size,
                                            float.PositiveInfinity)));
                                }

                                row.Size = Mathf.Max(row.Size, cache.Size.Height);
                            }
                        }
                    }
                    else if (row.UseRemain)
                    {
                        continue;
                    }
                    else
                    {
                        row.Size = row.LogicalSize > 1f
                            ? row.LogicalSize
                            : availableSize.Height * row.LogicalSize;
                    }

                    height += row.Size;
                }
            }
            else
            {
                for (int i = 0; i < columnCount; i++)
                {
                    var column = _columnDefinitions[i];
                    column.Size = 0f;

                    if (column.UseRemain)
                    {
                        continue;
                    }

                    column.Size = column.LogicalSize > 1f
                        ? column.LogicalSize
                        : availableSize.Width * column.LogicalSize;
                    width += column.Size;
                }

                for (int i = 0; i < rowCount; i++)
                {
                    var row = _rowDefinitions[i];
                    row.Size = 0f;

                    if (row.UseRemain)
                    {
                        continue;
                    }

                    row.Size = row.LogicalSize > 1f
                        ? row.LogicalSize
                        : availableSize.Height * row.LogicalSize;
                    height += row.Size;
                }
            }

            if (_columnsUseRemain.Count > 0)
            {
                float remainWidth = Mathf.Max(0f, availableSize.Width - width);

                if (remainWidth > 0f)
                {
                    width += remainWidth;
                    remainWidth /= _columnsUseRemain.Count;

                    for (int i = _columnsUseRemain.Count - 1; i >= 0; i--)
                    {
                        _columnsUseRemain[i].Size = remainWidth;
                    }
                }
            }

            if (_rowsUseRemain.Count > 0)
            {
                float remainHeight = Mathf.Max(0f, availableSize.Height - height);

                if (remainHeight > 0f)
                {
                    height += remainHeight;
                    remainHeight /= _rowsUseRemain.Count;

                    for (int i = _rowsUseRemain.Count - 1; i >= 0; i--)
                    {
                        _rowsUseRemain[i].Size = remainHeight;
                    }
                }
            }

            return new Size(width, height);
        }

        private void UpdateUnitInfos(Control[] children)
        {
            _unitInfos = new UnitInfo[children.Length];

            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                _unitInfos[i] = new UnitInfo(
                    GetColumn(child),
                    GetRow(child),
                    GetColumnSpan(child),
                    GetRowSpan(child));
            }

            UnitInfo info;

            for (int i = 0; i < _unitInfos.Length; i++)
            {
                info = _unitInfos[i];

                for (int j = 0; j < _columnDefinitions.Length; j++)
                {
                    if (j < info.Column)
                    {
                        info.X += _columnDefinitions[j].Size;
                    }
                    else if (j <= info.Column + info.ColumnSpan)
                    {
                        info.Width += _columnDefinitions[j].Size;
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = 0; j < _rowDefinitions.Length; j++)
                {
                    if (j < info.Row)
                    {
                        info.Y += _rowDefinitions[j].Size;
                    }
                    else if (j <= info.Row + info.RowSpan)
                    {
                        info.Height += _rowDefinitions[j].Size;
                    }
                    else
                    {
                        break;
                    }
                }

                children[i].Measure(new Size(info.Width, info.Height));
                _unitInfos[i] = info;
            }
        }

        private static bool ValidatePositive(object value)
        {
            return (int)value > -1;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private UnitInfo[] _unitInfos;

        private ColumnDefinition[] _columnDefinitions = new ColumnDefinition[] { new ColumnDefinition(1f, 0) };
        private RowDefinition[] _rowDefinitions = new RowDefinition[] { new RowDefinition(1f, 0) };

        private bool _anyAutoColumn;
        private bool _anyAutoRow;

        private bool _unitInfoChanged = true;

        private readonly List<ColumnDefinition> _columnsUseRemain = new List<ColumnDefinition>();
        private readonly List<RowDefinition> _rowsUseRemain = new List<RowDefinition>();

        #endregion


        //------------------------------------------------------
        //
        //  Definitions
        //
        //------------------------------------------------------

        #region Definitions

        /// <summary>
        /// 表格的列定义
        /// </summary>
        private class ColumnDefinition : Definition
        {
            public readonly bool UseRemain;

            /// <summary>
            /// 初始化 <see cref="ColumnDefinition"/> 的新实例
            /// </summary>
            /// <param name="logicalWidth">列的逻辑高度</param>
            /// <param name="index">列索引</param>
            public ColumnDefinition(float logicalWidth, int index) : base(logicalWidth, index)
            {
                UseRemain = float.IsPositiveInfinity(logicalWidth);
            }
        }

        /// <summary>
        /// 表格的行定义
        /// </summary>
        private class RowDefinition : Definition
        {
            public readonly bool UseRemain;

            /// <summary>
            /// 初始化 <see cref="RowDefinition"/> 的新实例
            /// </summary>
            /// <param name="logicalHeight">行的逻辑高度</param>
            /// <param name="index">行索引</param>
            public RowDefinition(float logicalHeight, int index) : base(logicalHeight, index)
            {
                UseRemain = float.IsPositiveInfinity(logicalHeight);
            }
        }

        #endregion


        private readonly struct SizeCache
        {
            public readonly Size Size;
            public readonly bool IsValid;

            public SizeCache(Size size)
            {
                Size = size;
                IsValid = true;
            }
        }

        /// <summary>
        /// 控件所在单元的信息
        /// </summary>
        private struct UnitInfo
        {
            /// <summary>
            /// 单元所在列
            /// </summary>
            public int Column;

            /// <summary>
            /// 单元跨越的列总数
            /// </summary>
            public int ColumnSpan;

            /// <summary>
            /// 单元的高度
            /// </summary>
            public float Height;

            /// <summary>
            /// 单元所在行
            /// </summary>
            public int Row;

            /// <summary>
            /// 单元跨越的行总数
            /// </summary>
            public int RowSpan;

            /// <summary>
            /// 单元的宽度
            /// </summary>
            public float Width;

            /// <summary>
            /// 单元相对于网格左上角的 X 坐标
            /// </summary>
            public float X;

            /// <summary>
            /// 单元相对于网格左上角的 Y 坐标
            /// </summary>
            public float Y;

            /// <summary>
            /// 单元所在区域
            /// </summary>
            /// <remarks>坐标相对于网格左上角。</remarks>
            public Rect Rect => new Rect(X, Y, Width, Height);

            /// <summary>
            /// 单元的大小
            /// </summary>
            public Size Size => new Size(Width, Height);

            /// <summary>
            /// 初始化 <see cref="UnitInfo"/> 的新实例
            /// </summary>
            /// <param name="column">单元所在列</param>
            /// <param name="row">单元所在行</param>
            /// <param name="columnSpan">单元跨越的列总数</param>
            /// <param name="rowSpan">单元跨越的行总数</param>
            public UnitInfo(int column, int row, int columnSpan, int rowSpan)
            {
                Row = row;
                Column = column;

                RowSpan = rowSpan;
                ColumnSpan = columnSpan;

                X = 0f;
                Y = 0f;

                Width = 0f;
                Height = 0f;
            }
        }
    }
}
