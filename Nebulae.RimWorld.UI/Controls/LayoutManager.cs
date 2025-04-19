using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Patches;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

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
        private readonly SortedSet<Visual> _arrangeQueue = new SortedSet<Visual>(VisualComparer.Instance);

        private bool _isMeasureValid = false;
        private readonly SortedSet<Visual> _measureQueue = new SortedSet<Visual>(VisualComparer.Instance);

        private bool _isSegmentValid = false;
        private readonly SortedSet<Visual> _segmentQueue = new SortedSet<Visual>(VisualComparer.Instance);

        private readonly Window _owner;
        private Visual _root;

        private LayoutInfo _layoutInfo;

        private bool _drawDebugButtons = true;

        #endregion


        /// <summary>
        /// 布局管理器是否独立于窗口
        /// </summary>
        public readonly bool IsIndependent;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 是否绘制调试用按钮
        /// </summary>
        public bool DrawDebugButtons
        {
            get => _drawDebugButtons;
            set => _drawDebugButtons = value;
        }

        /// <summary>
        /// 拥有该控件管理器的窗口
        /// </summary>
        public Window Owner => _owner;

        /// <summary>
        /// 获取或设置根控件
        /// </summary>
        public Visual Root
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
                    _root.LayoutManager = this;
                    _root.Parent = null;
                    _root.Rank = 0;

                    foreach (var child in _root.LogicalChildren)
                    {
                        try
                        {
                            child.LayoutManager = this;
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

        #endregion


        /// <summary>
        /// 初始化 <see cref="LayoutManager"/> 的新实例
        /// </summary>
        public LayoutManager()
        {
            IsIndependent = true;
        }

        /// <summary>
        /// 初始化 <see cref="LayoutManager"/> 的新实例
        /// </summary>
        /// <param name="owner">拥有管理器的窗口</param>
        public LayoutManager(Window owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            _owner = owner;

            IsIndependent = false;
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
        /// <param name="layoutInfo">允许绘制的区域</param>
        public void Draw(LayoutInfo layoutInfo)
        {
            if (_isEmpty)
            {
                return;
            }

            Rect availableRect = layoutInfo;
            Size availableSize = layoutInfo;

            if (_isDirty || _layoutInfo.IsMeasureDirty(layoutInfo))
            {
                _layoutInfo = layoutInfo;

                _root.Measure(availableSize);
                _root.Arrange(availableRect);
                _root.Segment(availableRect);

                _isDirty = false;

                _isArrangeValid = true;
                _isMeasureValid = true;
                _isSegmentValid = true;

                _arrangeQueue.Clear();
                _measureQueue.Clear();
                _segmentQueue.Clear();
            }
            else if (_layoutInfo.IsArrangeDirty(layoutInfo))
            {
                _layoutInfo = layoutInfo;

                _root.Arrange(availableRect);
                _root.Segment(availableRect);

                _isArrangeValid = true;
                _isSegmentValid = true;

                _arrangeQueue.Clear();
                _segmentQueue.Clear();
            }
            else
            {
                if (!_isMeasureValid)
                {
                    foreach (var control in _measureQueue)
                    {
                        // Means that the control is a scroll viewer.
                        if (control.IsChild)
                        {
                            control.Measure(control.DesiredSize);
                        }
                        else
                        {
                            control.Measure(availableSize);
                        }
                    }

                    _measureQueue.Clear();
                    _isMeasureValid = true;
                }

                if (!_isArrangeValid)
                {
                    foreach (var control in _arrangeQueue)
                    {
                        // Means that the control is a scroll viewer.
                        if (control.IsChild)
                        {
                            control.Arrange(control.DesiredRect);
                        }
                        else
                        {
                            control.Arrange(availableRect);
                        }
                    }

                    _arrangeQueue.Clear();
                    _isArrangeValid = true;
                }

                if (!_isSegmentValid)
                {
                    foreach (var control in _segmentQueue)
                    {
                        // Means that the control is a scroll viewer.
                        if (control.IsChild)
                        {
                            control.Segment(control.ContentRect);
                        }
                        else
                        {
                            control.Segment(availableRect);
                        }
                    }

                    _segmentQueue.Clear();
                    _isSegmentValid = true;
                }
            }

            _root.Draw();
        }

        /// <summary>
        /// 绘制窗口调试按钮
        /// </summary>
        /// <param name="nonClientRect">窗口的非客户区</param>
        public void DrawWindowDebugButtons(Rect nonClientRect)
        {
            if (Widgets.ButtonText(new Rect(
                nonClientRect.x,
                nonClientRect.y,
                150f,
                24f), "Debug: ForceLayout"))
            {
                InvalidateLayout();
            }

            if (Widgets.ButtonText(new Rect(
                nonClientRect.x + 154f,
                nonClientRect.y,
                130f,
                24f), "Debug: ShowInfo")
                && !_isEmpty)
            {
                _root.ShowInfo();
            }
        }

        /// <summary>
        /// 无效化控件排布
        /// </summary>
        /// <param name="control">排布被无效化的控件</param>
        public void InvalidateArrange(Visual control)
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
                if (!IsInfected(viewer, _arrangeQueue))
                {
                    if (_arrangeQueue.Count > 0)
                    {
                        _arrangeQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _arrangeQueue.Add(viewer);
                }

                if (!IsInfected(viewer, _segmentQueue))
                {
                    if (_segmentQueue.Count > 0)
                    {
                        _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _segmentQueue.Add(viewer);
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
        public void InvalidateMeasure(Visual control)
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
                if (!IsInfected(viewer, _measureQueue))
                {
                    if (_measureQueue.Count > 0)
                    {
                        _measureQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _measureQueue.Add(viewer);
                }

                if (!IsInfected(viewer, _arrangeQueue))
                {
                    if (_arrangeQueue.Count > 0)
                    {
                        _arrangeQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _arrangeQueue.Add(viewer);
                }

                if (!IsInfected(viewer, _segmentQueue))
                {
                    if (_segmentQueue.Count > 0)
                    {
                        _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _segmentQueue.Add(viewer);
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
        public void InvalidateSegment(Visual control)
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
                if (!IsInfected(viewer, _segmentQueue))
                {
                    if (_segmentQueue.Count > 0)
                    {
                        _segmentQueue.RemoveWhere(x => CanInfectLayout(viewer, x));
                    }

                    _segmentQueue.Add(viewer);
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
        public bool IsArrangeValid(Visual control)
        {
            if (_isDirty || _isEmpty)
            {
                return false;
            }

            if (_isArrangeValid)
            {
                return true;
            }

            return !IsInfected(control, _arrangeQueue);
        }

        /// <summary>
        /// 判断控件度量是否有效
        /// </summary>
        /// <param name="control">要判断度量是否有效的控件</param>
        /// <returns>控件度量是否有效。</returns>
        public bool IsMeasureValid(Visual control)
        {
            if (_isDirty || _isEmpty)
            {
                return false;
            }

            if (_isMeasureValid)
            {
                return true;
            }

            return !IsInfected(control, _measureQueue);
        }

        /// <summary>
        /// 判断控件分割是否有效
        /// </summary>
        /// <param name="control">要判断分割是否有效的控件</param>
        /// <returns>控件分割是否有效。</returns>
        public bool IsSegmentValid(Visual control)
        {
            if (_isDirty || _isEmpty)
            {
                return false;
            }

            if (_isSegmentValid)
            {
                return true;
            }

            return !IsInfected(control, _segmentQueue);
        }

        #endregion


        private static bool CanInfectLayout(Visual instigator, Visual target)
        {
            if (target.IsChild)
            {
                Visual parent = target.Parent;

                if (ReferenceEquals(instigator, parent))
                {
                    return true;
                }
                else if (parent is ScrollViewer)
                {
                    return false;
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

        private static bool IsInfected(Visual control, SortedSet<Visual> layoutQueue)
        {
            foreach (var item in layoutQueue)
            {
                if (item.Rank > control.Rank)
                {
                    return false;
                }

                if (ReferenceEquals(item, control)
                    || CanInfectLayout(item, control))
                {
                    return true;
                }
            }

            return false;
        }


        private class VisualComparer : IComparer<Visual>
        {
            public static readonly VisualComparer Instance = new VisualComparer();


            public int Compare(Visual x, Visual y)
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
