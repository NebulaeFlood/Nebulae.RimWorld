using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 可输入内容的控件基类
    /// </summary>
    public abstract class InputControl : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="InputControl"/> 是否强制获取焦点
        /// </summary>
        /// <remarks><see cref="Control.IsEnabled"/> 为 <see langword="false"/> 的 <see cref="InputControl"/> 无法强制获取焦点。</remarks>
        public bool ForceFocus
        {
            get => ControlStates.HasState(ControlState.ForceFocus);
            set
            {
                if (ControlStates.HasState(ControlState.Disabled))
                {
                    return;
                }

                if (value)
                {
                    ControlStates |= ControlState.ForceFocus;
                }
                else
                {
                    ControlStates &= ~ControlState.ForceFocus;
                }
            }
        }

        /// <summary>
        /// 获取一个值，该值指示 <see cref="InputControl"/> 是否拥有焦点
        /// </summary>
        public bool IsFocused => ControlStates.HasState(ControlState.Focused);

        /// <summary>
        /// 拥有 <see cref="InputControl"/> 的窗口
        /// </summary>
        public Window Owner => LayoutManager.Owner;

        #endregion


        /// <summary>
        /// 为 <see cref="InputControl"/> 派生类实现基本初始化
        /// </summary>
        protected InputControl()
        {
            _focusIndex = $"Nebulae.RimWorld.UI.Controls.FocusableControl_{_globeIndex++}";

            IsHitTestVisible = true;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 使 <see cref="InputControl"/> 丢失焦点
        /// </summary>
        public void Blur()
        {
            ControlStates &= ~ControlState.ForceFocus;
            ControlStates |= ControlState.WillLossFocus;
        }

        /// <summary>
        /// 使 <see cref="InputControl"/> 获取焦点
        /// </summary>
        /// <remarks><see cref="Control.IsEnabled"/> 为 <see langword="false"/> 的 <see cref="InputControl"/> 无法获取焦点。</remarks>
        public void Focus()
        {
            if (ControlStates.HasState(ControlState.Disabled))
            {
                return;
            }

            ControlStates |= ControlState.WillFocus;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override sealed void DrawCore(ControlState states)
        {
            CheckFocus(ref states);

            GUI.SetNextControlName(_focusIndex);
            DrawControl(states);

            if (states.HasState(ControlState.WillLossFocus))
            {
                GUI.FocusControl(null);

                focusedControl = null;

                ControlStates = states & ~(ControlState.Focused | ControlState.ForceFocus | ControlState.WillFocus | ControlState.WillLossFocus);
            }
            else if (states.HasState(ControlState.WillFocus))
            {
                if (Owner is Window owner)
                {
                    Verse.UI.FocusControl(_focusIndex, owner);
                }
                else
                {
                    GUI.FocusControl(_focusIndex);
                }

                focusedControl = this;

                ControlStates = (states & ~ControlState.WillFocus) | ControlState.Focused;
            }
        }

        /// <summary>
        /// 绘制可焦聚的控件
        /// </summary>
        /// <param name="states">当前的状态</param>
        protected abstract void DrawControl(ControlState states);

        /// <inheritdoc/>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Focus update before this event, if focusedControl is not null, means we are forcing system to focus it.
            if (focusedControl is null && !ControlStates.HasState(ControlState.Focused))
            {
                ControlStates |= ControlState.WillFocus;
            }

            e.Handled = true;
        }

        #endregion


        private void CheckFocus(ref ControlState states)
        {
            if (_focusIndex == GUI.GetNameOfFocusedControl())
            {
                focusedControl = this;

                if (states.HasState(ControlState.Disabled)
                    || (Event.current.type is EventType.MouseDown && !states.HasState(ControlState.CursorDirectlyOver | ControlState.ForceFocus)))
                {
                    states |= ControlState.WillLossFocus;
                }
            }
            else if (states.IsState(ControlState.ForceFocus))
            {
                states |= ControlState.WillFocus;
            }
            else if (states.HasState(ControlState.Focused))
            {
                states &= ~ControlState.WillLossFocus;
                ControlStates = states;
            }
        }


        internal static InputControl focusedControl;


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static int _globeIndex = 0;

        private readonly string _focusIndex = string.Empty;

        #endregion
    }
}
