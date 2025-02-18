using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.UI.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 布局管理器
    /// </summary>
    public sealed class LayoutManager
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
        public Control Root
        {
            get => _root;
            set
            {
                if (!ReferenceEquals(_root, value))
                {
                    if (!_isEmpty)
                    {
                        _root.SetParent(null);
                    }

                    _isDirty = true;
                    _isEmpty = value is null;
                    _root = value;

                    if (_isEmpty)
                    {
                        return;
                    }

                    _root.IsChild = false;
                    _root.Owner = _owner;
                    _root.Parent = null;
                    _root.Rank = 0;

                    foreach (var child in _root.LogicalChildren)
                    {
                        try
                        {
                            child.Owner = _owner;
                            child.Rank = child.Parent.Rank + 1;
                        }
                        catch (Exception e)
                        {
                            throw new LogicalTreeException(child, "A error occured when setting the root of a layout tree, please check that the parent-child relationship between controls has been set correctly.", e);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 初始化 <see cref="LayoutManager"/> 的新实例
        /// </summary>
        public LayoutManager(ControlWindow owner)
        {
            _owner = owner;
        }


        //------------------------------------------------------
        //
        //  public Methods
        //
        //------------------------------------------------------

        #region public Methods

        /// <summary>
        /// 绘制控件
        /// </summary>
        /// <param name="clientRect">窗口的客户区</param>
        public void Draw(Rect clientRect)
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

                _isArrangeValid = true;
                _isMeasureValid = true;
                _isSegmentValid = true;

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
        public void InvalidateArrange(Control control)
        {
            if (_isDirty || _isEmpty)
            {
                return;
            }

            if (!control.IsChild)
            {
                _arrangeQueue.Clear();
                _segmentQueue.Clear();

                _arrangeQueue.Add(control);
                _segmentQueue.Add(control);

                _isArrangeValid = false;
                _isSegmentValid = false;
            }
            else if (control.Parent is ScrollViewer viewer)
            {
                if (_arrangeQueue.Add(viewer))
                {
                    if (_arrangeQueue.Count > 1)
                    {
                        _arrangeQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                        _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _segmentQueue.Add(viewer);
                }
                else if (_segmentQueue.Add(viewer) && _segmentQueue.Count > 1)
                {
                    _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                }

                _isArrangeValid = false;
                _isSegmentValid = false;
            }
            else
            {
                InvalidateArrange(control.Parent);
            }
        }

        /// <summary>
        /// 无效化控件布局
        /// </summary>
        public void InvalidateLayout()
        {
            _isDirty = true;
        }

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        /// <param name="control">度量被无效化的控件</param>
        public void InvalidateMeasure(Control control)
        {
            if (_isDirty || _isEmpty)
            {
                return;
            }

            if (!control.IsChild)
            {
                _isDirty = true;
            }
            else if (control.Parent is ScrollViewer viewer)
            {
                if (_measureQueue.Add(viewer))
                {
                    if (_measureQueue.Count > 1)
                    {
                        _arrangeQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                        _measureQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                        _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _arrangeQueue.Add(viewer);
                    _segmentQueue.Add(viewer);
                }
                else if (_arrangeQueue.Add(viewer))
                {
                    if (_arrangeQueue.Count > 1)
                    {
                        _arrangeQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                        _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _segmentQueue.Add(viewer);
                }
                else if (_segmentQueue.Add(viewer) && _segmentQueue.Count > 1)
                {
                    _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                }

                _isArrangeValid = false;
                _isMeasureValid = false;
                _isSegmentValid = false;
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
        public void InvalidateSegment(Control control)
        {
            if (_isDirty || _isEmpty)
            {
                return;
            }

            _isSegmentValid = false;

            if (!control.IsChild)
            {
                _segmentQueue.Clear();
                _segmentQueue.Add(control);

                _isSegmentValid = false;
            }
            else if (control.Parent is ScrollViewer viewer)
            {
                if (_segmentQueue.Add(viewer) && _segmentQueue.Count > 1)
                {
                    _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                }

                _isSegmentValid = false;
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
        public bool IsArrangeValid(Control control)
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
                if (item.Rank > control.Rank)
                {
                    return false;
                }
                else if (ReferenceEquals(item, control)
                    || CanInfectLayout(item, control))
                {
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断控件度量是否有效
        /// </summary>
        /// <param name="control">要判断度量是否有效的控件</param>
        /// <returns>控件度量是否有效。</returns>
        public bool IsMeasureValid(Control control)
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
                if (item.Rank > control.Rank)
                {
                    return false;
                }
                else if (ReferenceEquals(item, control)
                    || CanInfectLayout(item, control))
                {
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断控件分割是否有效
        /// </summary>
        /// <param name="control">要判断分割是否有效的控件</param>
        /// <returns>控件分割是否有效。</returns>
        public bool IsSegmentValid(Control control)
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
                if (item.Rank > control.Rank)
                {
                    return false;
                }
                else if (ReferenceEquals(item, control)
                    || CanInfectLayout(item, control))
                {
                    break;
                }
            }

            return true;
        }

        #endregion


        private static bool CanInfectLayout(Control instigator, Control target)
        {
            if (target.IsChild)
            {
                Control parent = target.Parent;

                if (parent is ScrollViewer)
                {
                    return false;
                }
                else if (ReferenceEquals(instigator, parent))
                {
                    return true;
                }
                else
                {
                    return CanInfectLayout(instigator, parent);
                }
            }
            else
            {
                return false;
            }
        }


        private class ControlComparer : IComparer<Control>
        {
            public static readonly ControlComparer Instance = new ControlComparer();


            public int Compare(Control x, Control y)
            {
                if (x.Rank < y.Rank)
                {
                    return -1;
                }
                else if (ReferenceEquals(x, y))
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
