using HarmonyLib;
using LudeonTK;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Utilities;
using System;
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
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="LayoutManager"/> 是否绘制调试用按钮
        /// </summary>
        public bool DrawDebugButtons
        {
            get => _drawDebugButtons;
            set => _drawDebugButtons = value;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="LayoutManager"/> 中的控件是否参与布局命中测试
        /// </summary>
        public bool IsHitTestVisible
        {
            get => _isHitTestVisible;
            set => _isHitTestVisible = value;
        }

        /// <summary>
        /// 获取拥有该 <see cref="LayoutManager"/> 的窗口
        /// </summary>
        public Window Owner => _owner;

        /// <summary>
        /// 获取或设置 <see cref="LayoutManager"/> 的根控件
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
                        _root.Parent = null;
                    }

                    _isEmpty = value is null;
                    _root = value;

                    InvalidateLayout();

                    if (!_isEmpty)
                    {
                        _root.LayoutManager = this;
                    }
                }
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="LayoutManager"/> 的新实例
        /// </summary>
        public LayoutManager()
        {
            _isIndependent = true;
            _layer = WindowLayer.GameUI;
            UIPatch.ScaleChanged += OnScaleChanged;
        }

        /// <summary>
        /// 初始化 <see cref="LayoutManager"/> 的新实例
        /// </summary>
        /// <param name="layer">管理器所在的层</param>
        public LayoutManager(WindowLayer layer)
        {
            _isIndependent = true;
            _layer = layer;
            UIPatch.ScaleChanged += OnScaleChanged;
        }

        /// <summary>
        /// 初始化 <see cref="LayoutManager"/> 的新实例
        /// </summary>
        /// <param name="owner">拥有管理器的窗口</param>
        public LayoutManager(Window owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _isIndependent = false;

            UIPatch.ScaleChanged += OnScaleChanged;
        }

        #endregion


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

            if (InputUtility.isHitTesting)
            {
                if (_isMeasureDirty || _layoutInfo.IsMeasureDirty(layoutInfo))
                {
                    Rect availableRect = layoutInfo;

                    _root.Measure(layoutInfo);
                    _root.Arrange(availableRect);
                    _root.Segment(availableRect);

                    _isArrangeDirty = false;
                    _isMeasureDirty = false;
                    _isSegmentDirty = false;

                    _layoutInfo = layoutInfo;
                }
                else if (_isArrangeDirty || _layoutInfo.IsArrangeDirty(layoutInfo))
                {
                    Rect availableRect = layoutInfo;

                    _root.Arrange(availableRect);
                    _root.Segment(availableRect);

                    _isArrangeDirty = false;
                    _isSegmentDirty = false;

                    _layoutInfo = layoutInfo;
                }
                else if (_isSegmentDirty)
                {
                    _root.Segment(layoutInfo);

                    _isSegmentDirty = false;
                }
                else
                {
                    if (!_measureQueue.IsEmpty)
                    {
                        _measureQueue.Measure();
                    }

                    if (!_arrangeQueue.IsEmpty)
                    {
                        _arrangeQueue.Arrange();
                    }

                    if (!_segmentQueue.IsEmpty)
                    {
                        _segmentQueue.Segment();
                    }
                }

                if (_isHitTestVisible)
                {
                    if (ReferenceEquals(_owner, InputUtility.hoveredWindow))
                    {
                        HitTestUtility.InputHitTest(_root);
                    }
                    else if (_isIndependent && (InputUtility.allowIndependentHitTest || _layer >= InputUtility.hoveredWindow.layer))
                    {
                        HitTestUtility.InputHitTestIndependently(_root);
                    }
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
            if (DevGUI.ButtonText(new Rect(
                nonClientRect.x,
                nonClientRect.y,
                150f,
                24f), "Debug: ForceLayout"))
            {
                InvalidateLayout();
            }

            if (DevGUI.ButtonText(new Rect(
                nonClientRect.x + 154f,
                nonClientRect.y,
                130f,
                24f), "Debug: ShowTree")
                && !_isEmpty)
            {
                _root.Debug();
            }
        }

        /// <summary>
        /// 无效化控件排布
        /// </summary>
        /// <param name="control">排布被无效化的控件</param>
        public void InvalidateArrange(Control control)
        {
            if (_isArrangeDirty || _isEmpty)
            {
                return;
            }

            var parent = control.Parent;

            while (parent != null)
            {
                if (parent.IsSolid)
                {
                    _arrangeQueue.Enqueue(parent);
                    _segmentQueue.Enqueue(parent);

                    return;
                }

                control = parent;
                parent = control.Parent;
            }

            _arrangeQueue.Clear();
            _segmentQueue.Clear();

            _isArrangeDirty = true;
            _isSegmentDirty = true;
        }

        /// <summary>
        /// 无效化控件度量
        /// </summary>
        /// <param name="control">度量被无效化的控件</param>
        public void InvalidateMeasure(Control control)
        {
            if (_isMeasureDirty || _isEmpty)
            {
                return;
            }

            var parent = control.Parent;

            while (parent != null)
            {
                if (parent.IsSolid)
                {
                    _arrangeQueue.Enqueue(parent);
                    _measureQueue.Enqueue(parent);
                    _segmentQueue.Enqueue(parent);

                    return;
                }

                control = parent;
                parent = control.Parent;
            }

            _arrangeQueue.Clear();
            _measureQueue.Clear();
            _segmentQueue.Clear();

            _isArrangeDirty = true;
            _isMeasureDirty = true;
            _isSegmentDirty = true;
        }

        /// <summary>
        /// 无效化控件分割
        /// </summary>
        /// <param name="control">分割被无效化的控件</param>
        public void InvalidateSegment(Control control)
        {
            if (_isSegmentDirty || _isEmpty)
            {
                return;
            }

            var parent = control.Parent;

            while (parent != null)
            {
                if (parent.IsSolid)
                {
                    _segmentQueue.Enqueue(parent);

                    return;
                }

                control = parent;
                parent = control.Parent;
            }

            _segmentQueue.Clear();

            _isSegmentDirty = true;
        }

        /// <summary>
        /// 无效化控件布局
        /// </summary>
        public void InvalidateLayout()
        {
            _isArrangeDirty = true;
            _isMeasureDirty = true;
            _isSegmentDirty = true;

            _arrangeQueue.Clear();
            _measureQueue.Clear();
            _segmentQueue.Clear();
        }

        /// <summary>
        /// 判断控件排布是否有效
        /// </summary>
        /// <param name="control">要判断排布是否有效的控件</param>
        /// <returns>控件排布是否有效。</returns>
        public bool IsArrangeValid(Control control)
        {
            if (_isArrangeDirty || _isEmpty)
            {
                return false;
            }

            if (_arrangeQueue.IsEmpty)
            {
                return true;
            }

            return !_arrangeQueue.Exist(control);
        }

        /// <summary>
        /// 判断控件度量是否有效
        /// </summary>
        /// <param name="control">要判断度量是否有效的控件</param>
        /// <returns>控件度量是否有效。</returns>
        public bool IsMeasureValid(Control control)
        {
            if (_isMeasureDirty || _isEmpty)
            {
                return false;
            }

            if (_measureQueue.IsEmpty)
            {
                return true;
            }

            return !_measureQueue.Exist(control);
        }

        /// <summary>
        /// 判断控件分割是否有效
        /// </summary>
        /// <param name="control">要判断分割是否有效的控件</param>
        /// <returns>控件分割是否有效。</returns>
        public bool IsSegmentValid(Control control)
        {
            if (_isSegmentDirty || _isEmpty)
            {
                return false;
            }

            if (_segmentQueue.IsEmpty)
            {
                return true;
            }

            return !_segmentQueue.Exist(control);
        }

        #endregion


        private void OnScaleChanged(Harmony sender, ScaleChangedEventArgs args)
        {
            _isArrangeDirty = true;
            _isMeasureDirty = true;
            _isSegmentDirty = true;

            _arrangeQueue.Clear();
            _measureQueue.Clear();
            _segmentQueue.Clear();
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private bool _isEmpty = true;
        private bool _isHitTestVisible = true;

        private bool _isArrangeDirty = true;
        private readonly LayoutQueue _arrangeQueue = new LayoutQueue();

        private bool _isMeasureDirty = true;
        private readonly LayoutQueue _measureQueue = new LayoutQueue();

        private bool _isSegmentDirty = true;
        private readonly LayoutQueue _segmentQueue = new LayoutQueue();

        private readonly bool _isIndependent;
        private readonly Window _owner;
        private Control _root;

        private WindowLayer _layer;
        private LayoutInfo _layoutInfo;

        private bool _drawDebugButtons = true;

        #endregion
    }
}
