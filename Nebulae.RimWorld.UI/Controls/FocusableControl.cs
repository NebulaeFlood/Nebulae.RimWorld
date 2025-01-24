using System;
using UnityEngine;

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
                && (_status.HasFlag(Status.ForceFocusing)
                    || _status.HasFlag(Status.WillFocus)))
            {
                Verse.UI.FocusControl(_focusIndex, Owner);

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
                EventType eventType = Event.current.type;

                if (eventType is EventType.MouseDrag)
                {
                    return true;
                }

                if (eventType is EventType.MouseDown
                    && !ContentRect.Contains(Event.current.mousePosition))
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
            ForceFocusing = 0b0010,
            WillFocus = 0b0100,
            LossingFocus = 0b1000
        }
    }
}
