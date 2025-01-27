using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.UI.Windows;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 布局管理器
    /// </summary>
    internal sealed class LayoutManager
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private bool _isDirty = true;
        private bool _isEmpty = true;

        private bool _isArrangeValid = false;
        private readonly SortedSet<Control> _arrangeQueue = new SortedSet<Control>(ControlComparer.Instance);

        private bool _isMeasureValid = false;
        private readonly SortedSet<Control> _measureQueue = new SortedSet<Control>(ControlComparer.Instance);

        private bool _isSegmentValid = false;
        private readonly SortedSet<Control> _segmentQueue = new SortedSet<Control>(ControlComparer.Instance);

        private ControlWindow _owner;
        private Control _root;

        #endregion


        /// <summary>
        /// 获取或设置根控件
        /// </summary>
        internal Control Root
        {
            get => _root;
            set
            {
                if (!ReferenceEquals(_root, value))
                {
                    _root = value;
                    _root.Owner = _owner;
                    _root.Rank = 0;

                    _isDirty = true;
                    _isEmpty = value is null;
                }
            }
        }


        /// <summary>
        /// 初始化 <see cref="LayoutManager"/> 的新实例
        /// </summary>
        internal LayoutManager(ControlWindow owner)
        {
            _owner = owner;
        }


        //------------------------------------------------------
        //
        //  internal Methods
        //
        //------------------------------------------------------

        #region internal Methods

        /// <summary>
        /// 绘制控件
        /// </summary>
        /// <param name="clientRect">窗口的客户区</param>
        internal void Draw(Rect clientRect)
        {
            if (_isEmpty)
            {
                return;
            }

            if (_isDirty)
            {
                _root.Measure(clientRect);
                _root.Arrange(clientRect);
                _root.Segment(clientRect);

                _isDirty = false;

                _isArrangeValid = false;
                _isMeasureValid = false;
                _isSegmentValid = false;

                _arrangeQueue.Clear();
                _measureQueue.Clear();
                _segmentQueue.Clear();

                _root.Draw();

                return;
            }

            if (!_isMeasureValid)
            {
                foreach (var control in _measureQueue)
                {
                    if (control.IsChild)
                    {
                        control.Parent.Measure(control.Parent.DesiredSize);
                    }
                    else
                    {
                        control.Measure(clientRect);
                    }
                }

                _measureQueue.Clear();
                _isMeasureValid = true;
            }

            if (!_isArrangeValid)
            {
                foreach (var control in _arrangeQueue)
                {
                    if (control.IsChild)
                    {
                        control.Parent.Arrange(control.Parent.DesiredRect);
                    }
                    else
                    {
                        control.Arrange(clientRect);
                    }
                }

                _arrangeQueue.Clear();
                _isArrangeValid = true;
            }

            if (!_isSegmentValid)
            {
                foreach (var control in _segmentQueue)
                {
                    if (control.IsChild)
                    {
                        control.Parent.Segment(control.Parent.ContentRect);
                    }
                    else
                    {
                        control.Segment(clientRect);
                    }
                }

                _segmentQueue.Clear();
                _isSegmentValid = true;
            }

            _root.Draw();
        }

        /// <summary>
        /// 无效化控件排布
        /// </summary>
        /// <param name="control">排布被无效化的控件</param>
        internal void InvalidateArrange(Control control)
        {
            if (_isDirty || _isEmpty)
            {
                return;
            }

            if (_arrangeQueue.Count < 1)
            {
                _arrangeQueue.Add(control);
                _isArrangeValid = false;
            }
            else if (!control.IsChild || control.Parent is ScrollViewer)
            {
                if (_isArrangeValid)
                {
                    _arrangeQueue.Add(control);
                    _isArrangeValid = false;
                }
                else
                {
                    foreach (var item in _arrangeQueue)
                    {
                        if (control.IsParent(item))
                        {
                            _arrangeQueue.Remove(item);
                            break;
                        }
                        else if (ReferenceEquals(control, item))
                        {
                            return;
                        }
                        else if (item.Rank > control.Rank)
                        {
                            break;
                        }
                    }

                    _arrangeQueue.Add(control);
                }
            }
            else
            {
                InvalidateArrange(control.Parent);
            }
        }

        /// <summary>
        /// 无效化控件布局
        /// </summary>
        internal void InvalidateLayout()
        {
            if (_isDirty || _isEmpty)
            {
                return;
            }

            _isDirty = true;
        }

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        /// <param name="control">度量被无效化的控件</param>
        internal void InvalidateMeasure(Control control)
        {
            if (_isDirty || _isEmpty)
            {
                return;
            }

            if (_measureQueue.Count < 1)
            {
                _measureQueue.Add(control);
                _isMeasureValid = false;
            }
            else if (!control.IsChild || control.Parent is ScrollViewer)
            {
                if (_isMeasureValid)
                {
                    _measureQueue.Add(control);
                    _isMeasureValid = false;
                }
                else
                {
                    foreach (var item in _measureQueue)
                    {
                        if (control.IsParent(item))
                        {
                            _measureQueue.Remove(item);
                            break;
                        }
                        else if (ReferenceEquals(control, item))
                        {
                            return;
                        }
                        else if (item.Rank > control.Rank)
                        {
                            break;
                        }
                    }

                    _measureQueue.Add(control);
                }
            }
            else
            {
                InvalidateMeasure(control.Parent);
            }
        }

        /// <summary>
        /// 无效化控件分割
        /// </summary>
        /// <param name="control">分割被无效化的控件</param>
        internal void InvalidateSegment(Control control)
        {
            if (_isDirty || _isEmpty)
            {
                return;
            }

            if (_segmentQueue.Count < 1)
            {
                _segmentQueue.Add(control);
                _isSegmentValid = false;
            }
            else if (!control.IsChild || control.Parent is ScrollViewer)
            {
                if (_isSegmentValid)
                {
                    _segmentQueue.Add(control);
                    _isSegmentValid = false;
                }
                else
                {
                    foreach (var item in _segmentQueue)
                    {
                        if (control.IsParent(item))
                        {
                            _segmentQueue.Remove(item);
                            break;
                        }
                        else if (ReferenceEquals(control, item))
                        {
                            return;
                        }
                        else if (item.Rank > control.Rank)
                        {
                            break;
                        }
                    }

                    _segmentQueue.Add(control);
                }
            }
            else
            {
                InvalidateSegment(control.Parent);
            }
        }

        /// <summary>
        /// 判断控件排布是否有效
        /// </summary>
        /// <param name="control">要判断排布是否有效的控件</param>
        /// <returns>控件排布是否有效。</returns>
        internal bool IsArrangeValid(Control control)
        {
            if (_isDirty)
            {
                return false;
            }

            if (_isArrangeValid)
            {
                return true;
            }

            foreach (var item in _arrangeQueue)
            {
                if (control.IsParent(item)
                    || ReferenceEquals(control, item))
                {
                    return false;
                }
                else if (item.Rank > control.Rank)
                {
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断控件度量是否有效
        /// </summary>
        /// <param name="control">要判断度量是否有效的控件</param>
        /// <returns>控件度量是否有效。</returns>
        internal bool IsMeasureValid(Control control)
        {
            if (_isDirty)
            {
                return false;
            }

            if (_isMeasureValid)
            {
                return true;
            }

            foreach (var item in _measureQueue)
            {
                if (control.IsParent(item)
                    || ReferenceEquals(control, item))
                {
                    return false;
                }
                else if (item.Rank > control.Rank)
                {
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断控件分割是否有效
        /// </summary>
        /// <param name="control">要判断分割是否有效的控件</param>
        /// <returns>控件分割是否有效。</returns>
        internal bool IsSegmentValid(Control control)
        {
            if (_isDirty)
            {
                return false;
            }

            if (_isSegmentValid)
            {
                return true;
            }

            foreach (var item in _segmentQueue)
            {
                if (control.IsParent(item)
                    || ReferenceEquals(control, item))
                {
                    return false;
                }
                else if (item.Rank > control.Rank)
                {
                    return true;
                }
            }

            return true;
        }

        #endregion


        private class ControlComparer : IComparer<Control>
        {
            public static readonly ControlComparer Instance = new ControlComparer();


            public int Compare(Control x, Control y)
            {
                return x.Rank < y.Rank ? -1 : 1;
            }
        }
    }
}
