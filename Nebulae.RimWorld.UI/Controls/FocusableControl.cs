using Nebulae.RimWorld.UI.Utilities;
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
        internal static FocusableControl FocusingControl;

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static int _globeIndex = 0;


        private readonly string _focusIndex = string.Empty;
        private Status _status = Status.Normal;

        private Window _associatedWindow;

        #endregion


        /// <summary>
        /// 该控件关联的窗口
        /// </summary>
        public Window AssociatedWindow
        {
            get
            {
                if (_associatedWindow != null)
                {
                    return _associatedWindow;
                }

                if (IsIndependent)
                {
                    throw new Exception($"{this}.AssociatedWindow cannot be null.");
                }

                _associatedWindow = Owner;

                if (_associatedWindow is null)
                {
                    throw new Exception($"{this}.AssociatedWindow cannot be null.");
                }

                return _associatedWindow;
            }
            set
            {
                if (value is null)
                {
                    throw new InvalidOperationException($"{this}.AssociatedWindow cannot set to be null.");
                }

                if (IsIndependent)
                {
                    _associatedWindow = value;
                }
                else
                {
                    throw new InvalidOperationException($"Cannot set {this}.AssociatedWindow while cotrol is in a logical tree.");
                }
            }
        }

        /// <summary>
        /// 控件是否已经获取焦点
        /// </summary>
        public bool IsFocusing => _status.HasFlag(Status.Focusing);


        /// <summary>
        /// 为 <see cref="FocusableControl"/> 派生类实现基本初始化
        /// </summary>
        protected FocusableControl()
        {
            _focusIndex = "Nebulae.RimWorld.UI.Controls.FocusableControl" + _globeIndex++.ToString();

            IsHitTestVisible = true;
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
            _status &= ~Status.ForceFocusing;
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
        protected override void DrawCore()
        {
            GUI.SetNextControlName(_focusIndex);
            DrawControl();

            bool isFocusing = UpdateStatus();
            bool isForceFocusing = _status.HasFlag(Status.ForceFocusing);

            if (_status.HasFlag(Status.LossingFocus))
            {
                GUI.FocusControl(null);
                FocusingControl = null;

                _status = Status.Normal;
            }
            else if (!isFocusing
                && _status.HasFlag(Status.WillFocus))
            {
                Verse.UI.FocusControl(_focusIndex, AssociatedWindow);

                _status = isForceFocusing
                     ? Status.Focusing | Status.ForceFocusing
                     : Status.Focusing;
            }
        }

        /// <summary>
        /// 绘制可焦聚的控件
        /// </summary>
        protected abstract void DrawControl();

        #endregion


        private bool UpdateStatus()
        {
            if (_focusIndex == GUI.GetNameOfFocusedControl())
            {
                FocusingControl = this;

                _status |= Status.Focusing;
            }
            else
            {
                return false;
            }

            if (!_status.HasFlag(Status.ForceFocusing))
            {
                if (Event.current.type is EventType.MouseDown
                    && !IsCursorOver)
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
            Normal = 0b0000,
            Focusing = 0b0001,
            ForceFocusing = 0b0110,
            WillFocus = 0b0100,
            LossingFocus = 0b1000
        }
    }
}
