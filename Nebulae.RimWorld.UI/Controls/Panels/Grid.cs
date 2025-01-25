using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 将控件排布在设定好的格子内的面板
    /// </summary>
    public class Grid : Panel
    {
        /// <summary>
        /// 表示单元格剩余尺寸
        /// </summary>
        public const float Remain = float.NaN;

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private int _autoSizeColumn = -1;
        private int _autoSizeRow = -1;

        private float[] _columnWidths;
        private float[] _rowHeights;

        private float[] _logicalColumnWidths = new float[] { 1f };
        private float[] _logicalRowHeights = new float[] { 1f };

        private UnitInfo[] _unitInfos;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount => _logicalColumnWidths.Length;

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount => _logicalRowHeights.Length;

        #endregion


        /// <summary>
        /// 初始化 <see cref="Grid"/> 的新实例
        /// </summary>
        public Grid()
        {
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <remarks>
        /// 必须保证输入的结构能够匹配设置的行数和列数，否则会引发未知的行为。
        /// </remarks>
        public Grid Set(params Control[] controls)
        {
            Control[] controlList = controls.Cast<Control>().ToArray();
            Control[] cleanList = controlList.Where(x => x != null).Distinct().ToArray();
            Children.OverrideCollection(cleanList);

            cleanList = FilteredChildren;

            _unitInfos = new UnitInfo[cleanList.Length];

            int firstIndex;
            int lastIndex;
            for (int i = 0; i < cleanList.Length; i++)
            {
                firstIndex = Array.IndexOf(controlList, cleanList[i]);
                lastIndex = Array.LastIndexOf(controlList, cleanList[i]);

                if (firstIndex == lastIndex)
                {
                    _unitInfos[i] = new UnitInfo(
                        firstIndex / ColumnCount,
                        firstIndex % ColumnCount,
                        0,
                        0);
                }
                else
                {
                    int column = firstIndex % ColumnCount;
                    int row = firstIndex / ColumnCount;

                    _unitInfos[i] = new UnitInfo(
                        row,
                        column,
                        lastIndex / ColumnCount - row,
                        lastIndex % ColumnCount - column);
                }
            }

            return this;
        }

        /// <summary>
        /// 设置方格的尺寸
        /// </summary>
        /// <param name="columnWidths">每列的宽度</param>
        /// <param name="rowHeights">每行的高度</param>
        /// <returns>该面板控件</returns>
        /// <exception cref="ArgumentException">当行或列的数量小于 1，<see cref="Remain"/> 多于一个或尺寸为负时发生。</exception>
        /// <remarks>
        /// 通过此方法设置面板的行数与列数。<para/>
        /// 行或列的数量不能小于 1。<para/>
        /// 使用 <see cref="Remain"/> 自动剩余尺寸，一行或一列中只能有一个 <see cref="Remain"/> 尺寸。<para/>
        /// </remarks>
        public Grid SetSize(float[] columnWidths, float[] rowHeights)
        {
            if (columnWidths.Length < 1)
            {
                throw new ArgumentException("Must have at least one column.", nameof(columnWidths));
            }

            if (rowHeights.Length < 1)
            {
                throw new ArgumentException("Must have at least one row.", nameof(rowHeights));
            }

            if (Array.Exists(columnWidths, x => x < 0))
            {
                throw new ArgumentException("Column width can not be negative.", nameof(columnWidths));
            }

            if (Array.Exists(rowHeights, x => x < 0))
            {
                throw new ArgumentException("Row height can not be negative.", nameof(rowHeights));
            }

            _autoSizeColumn = -1;
            _autoSizeRow = -1;

            for (int i = 0; i < columnWidths.Length; i++)
            {
                if (float.IsNaN(columnWidths[i]))
                {
                    if (_autoSizeColumn == -1)
                    {
                        _autoSizeColumn = i;
                    }
                    else
                    {
                        throw new ArgumentException("Only one auto column width is allowed but found two more.", nameof(columnWidths));
                    }
                }
            }

            for (int i = 0; i < rowHeights.Length; i++)
            {
                if (float.IsNaN(rowHeights[i]))
                {
                    if (_autoSizeRow == -1)
                    {
                        _autoSizeRow = i;
                    }
                    else
                    {
                        throw new ArgumentException("Only one auto row height is allowed but found two more.", nameof(rowHeights));
                    }
                }
            }

            _logicalColumnWidths = columnWidths;
            _logicalRowHeights = rowHeights;

            InvalidateMeasure();

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
            Control[] filteredChildren = FilteredChildren;
            for (int i = 0; i < filteredChildren.Length; i++)
            {
                filteredChildren[i].Arrange(_unitInfos[i].Rect.Offset(availableRect.x, availableRect.y));
            }

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < _unitInfos.Length; i++)
            {
                _unitInfos[i] = new UnitInfo(_unitInfos[i]);
            }

            _columnWidths = new float[_logicalColumnWidths.Length];
            _rowHeights = new float[_logicalRowHeights.Length];

            _logicalColumnWidths.CopyTo(_columnWidths, 0);
            _logicalRowHeights.CopyTo(_rowHeights, 0);

            float calculatedWidth = 0f;
            for (int i = 0; i < _columnWidths.Length; i++)
            {
                if (_autoSizeColumn != i)
                {
                    if (_columnWidths[i] <= 1f)
                    {
                        _columnWidths[i] *= availableSize.Width;
                    }
                    calculatedWidth += _columnWidths[i];
                }
            }

            if (_autoSizeColumn > -1)
            {
                _columnWidths[_autoSizeColumn] = (availableSize.Width - calculatedWidth)
                    .Clamp(0f, availableSize.Width);
                calculatedWidth += _columnWidths[_autoSizeColumn];
            }

            float calculatedHeight = 0f;
            for (int i = 0; i < _rowHeights.Length; i++)
            {
                if (_autoSizeRow != i)
                {
                    if (_rowHeights[i] <= 1f)
                    {
                        _rowHeights[i] *= availableSize.Height;
                    }
                    calculatedHeight += _rowHeights[i];
                }
            }

            if (_autoSizeRow > -1)
            {
                _rowHeights[_autoSizeRow] = (availableSize.Height - calculatedHeight)
                    .Clamp(0f, availableSize.Height);
                calculatedHeight += _rowHeights[_autoSizeRow];
            }

            UpdateUnitInfo();

            Control[] filteredChildren = FilteredChildren;
            for (int i = 0; i < filteredChildren.Length; i++)
            {
                filteredChildren[i].Measure(_unitInfos[i].Size);
            }

            return new Size(calculatedWidth, calculatedHeight);
        }

        #endregion


        private void UpdateUnitInfo()
        {
            UnitInfo info;
            for (int i = 0; i < _unitInfos.Length; i++)
            {
                info = _unitInfos[i];

                for (int j = 0; j < _columnWidths.Length; j++)
                {
                    if (j < info.Column)
                    {
                        info.X += _columnWidths[j];
                    }
                    else if (j <= info.Column + info.ColumnSpan)
                    {
                        info.Width += _columnWidths[j];
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = 0; j < _rowHeights.Length; j++)
                {
                    if (j < info.Row)
                    {
                        info.Y += _rowHeights[j];
                    }
                    else if (j <= info.Row + info.RowSpan)
                    {
                        info.Height += _rowHeights[j];
                    }
                    else
                    {
                        break;
                    }
                }

                _unitInfos[i] = info;
            }
        }


        /// <summary>
        /// 控件所在单元的信息
        /// </summary>
        protected struct UnitInfo
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
            /// <param name="row">单元所在行</param>
            /// <param name="column">单元所在列</param>
            /// <param name="rowSpan">单元跨越的行总数</param>
            /// <param name="columnSpan">单元跨越的列总数</param>
            public UnitInfo(int row, int column, int rowSpan, int columnSpan)
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

            /// <summary>
            /// 初始化仅继承所占用行列的信息的 <see cref="UnitInfo"/> 的新实例
            /// </summary>
            /// <param name="info">将被继承信息的 <see cref="UnitInfo"/> 实例</param>
            public UnitInfo(UnitInfo info)
            {
                Row = info.Row;
                Column = info.Column;

                RowSpan = info.RowSpan;
                ColumnSpan = info.ColumnSpan;

                X = 0f;
                Y = 0f;

                Width = 0f;
                Height = 0f;
            }
        }
    }
}
