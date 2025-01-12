using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 可手动获取焦点的控件的基类，定义了其共同行为
    /// </summary>
    public abstract class FocusableControl : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static FocusableControl _focusingControl;
        private static int _globeIndex = 0;


        private Window _owner;

        private readonly string _focusIndex = string.Empty;

        private Status _status = Status.Normal;
        private bool _isSegmentValid = false;

        private Rect _visiableRectCache;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 持有焦点的控件
        /// </summary>
        public static FocusableControl FocusingControl => _focusingControl;


        /// <summary>
        /// 承载该控件的窗口
        /// </summary>
        public Window Owner
        {
            get => _owner;
            set => _owner = value;
        }

        #endregion


        /// <summary>
        /// 为 <see cref="FocusableControl"/> 派生类实现基本初始化
        /// </summary>
        protected FocusableControl()
        {
            _focusIndex = "Nebulae.RimWorld.UI.Controls.FocusableControl" + _globeIndex++.ToString();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 强制获取焦点
        /// </summary>
        public void ForceFocus()
        {
            _status = Status.ForceFocusing;
        }

        /// <summary>
        /// 使控件获取焦点
        /// </summary>
        public void GetFocus()
        {
            _status = Status.WillFocus;
        }

        /// <summary>
        /// 使控件丢失焦点
        /// </summary>
        public void LostFocus()
        {
            _status |= Status.LossingFocus;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect DrawCore(Rect renderRect)
        {
            GUI.SetNextControlName(_focusIndex);
            DrawControl(renderRect);

            bool isFocusing = UpdateStatus(renderRect);
            bool isForceFocusing = _status.HasFlag(Status.ForceFocusing);

            if (_status.HasFlag(Status.LossingFocus))
            {
                GUI.FocusControl(null);

                _focusingControl = null;
                _status = Status.Normal;
            }
            else if (!isFocusing
                && (_status.HasFlag(Status.ForceFocusing) || _status.HasFlag(Status.WillFocus)))
            {
                Verse.UI.FocusControl(_focusIndex, _owner);

                _status = isForceFocusing
                     ? Status.Focusing | Status.ForceFocusing
                     : Status.Focusing;
            }

            return renderRect;
        }

        /// <summary>
        /// 绘制可焦聚的控件
        /// </summary>
        /// <param name="renderRect">控件允许绘制的区域</param>
        protected abstract void DrawControl(Rect renderRect);

        /// <inheritdoc/>
        protected override void OnArrangeInvalidated()
        {
            base.OnArrangeInvalidated();
            _isSegmentValid = false;
        }

        #endregion


        private bool UpdateStatus(Rect renderRect)
        {
            if (_focusIndex == GUI.GetNameOfFocusedControl())
            {
                _focusingControl = this;
                _status |= Status.Focusing;
            }
            else
            {
                return false;
            }

            if (!_status.HasFlag(Status.ForceFocusing))
            {
                if (!_isSegmentValid)
                {
                    _visiableRectCache = this.CalculateVisiableRect(renderRect);
                    _isSegmentValid = true;
                }

                EventType eventType = Event.current.type;

                if (eventType is EventType.MouseDrag)
                {
                    return true;
                }

                if (eventType is EventType.MouseDown
                    && !_visiableRectCache.Contains(Event.current.mousePosition))
                {
                    _status |= Status.LossingFocus;
                    return true;
                }
            }

            return true;
        }


        [Flags]
        private enum Status : int
        {
            Normal = 0x00000000,
            Focusing = 0x00000001,
            ForceFocusing = 0x00000002,
            WillFocus = 0x00000004,
            LossingFocus = 0x00000008
        }
    }
}
