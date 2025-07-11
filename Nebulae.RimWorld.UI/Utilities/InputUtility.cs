﻿using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 输入帮助类
    /// </summary>
    public static class InputUtility
    {
        //------------------------------------------------------
        //
        //  Public Static Fields
        //
        //------------------------------------------------------

        #region Public Static Fields

        /// <summary>
        /// 鼠标左键
        /// </summary>
        public static readonly MouseTracker LeftButton = new MouseTracker(MouseButton.LeftMouse);

        /// <summary>
        /// 鼠标中键
        /// </summary>
        public static readonly MouseTracker MiddleButton = new MouseTracker(MouseButton.MiddleMouse);

        /// <summary>
        /// 鼠标右键
        /// </summary>
        public static readonly MouseTracker RightButton = new MouseTracker(MouseButton.RightMouse);

        #endregion


        //------------------------------------------------------
        //
        //  Public Static Properties
        //
        //------------------------------------------------------

        #region Public Static Properties

        /// <summary>
        /// 光标在 UI 系统中的坐标（和窗口在同一坐标系）
        /// </summary>
        public static Vector2 CursorPosition => _cursorPosition;

        /// <summary>
        /// 游戏窗口的尺寸
        /// </summary>
        public static Rect ScreenRect => _screenRect;

        #endregion


        //------------------------------------------------------
        //
        //  Internal Static Methods
        //
        //------------------------------------------------------

        #region Internal Static Methods

        internal static void TraceKeyBoard()
        {
            if ((!_isKeyDown && !_isKeyUp) || _keyCode is KeyCode.None)
            {
                return;
            }

            if (InputControl._focusedControl is null || InputControl._focusedControl.ControlStates.HasState(ControlState.WillLossFocus))
            {
                if (_protentialFocusControl is null)
                {
                    return;
                }

                if (_isKeyDown)
                {
                    _protentialFocusControl.RaiseEvent(new KeyEventArgs(_keyCode, _protentialFocusControl, Control.KeyDownEvent));
                }
                else
                {
                    _protentialFocusControl.RaiseEvent(new KeyEventArgs(_keyCode, _protentialFocusControl, Control.KeyUpEvent));
                }
            }
            else
            {
                if (_isKeyDown)
                {
                    InputControl._focusedControl.RaiseEvent(new KeyEventArgs(_keyCode, InputControl._focusedControl, Control.KeyDownEvent));
                }
                else
                {
                    InputControl._focusedControl.RaiseEvent(new KeyEventArgs(_keyCode, InputControl._focusedControl, Control.KeyUpEvent));
                }
            }
        }

        internal static void TraceWindow(List<Window> windowStack)
        {
            isHitTesting = UIUtility.CurrentEventType is EventType.Layout;

            var cursorPos = Input.mousePosition / Prefs.UIScale;
            cursorPos.y = Verse.UI.screenHeight - cursorPos.y;

            _cursorPosition = cursorPos;
            _screenRect = new Rect(0f, 0f, Verse.UI.screenWidth, Verse.UI.screenHeight);

            _isKeyDown = UIUtility.CurrentEventType is EventType.KeyDown;
            _isKeyUp = UIUtility.CurrentEventType is EventType.KeyUp;
            _keyCode = UIUtility.CurrentEvent.keyCode;

            if (!isHitTesting)
            {
                return;
            }

            Window window;

            for (int i = windowStack.Count - 1; i >= 0; i--)
            {
                window = windowStack[i];

                if (window.ID >= 0  // ImmediateWindow's ID < 0
                    && (window.absorbInputAroundWindow
                        || window.windowRect.Contains(cursorPos)))
                {
                    hoveredWindow = window;
                    return;
                }
            }

            hoveredWindow = null;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Static Fields
        //
        //------------------------------------------------------

        #region Internal Static Fields

        internal static Window hoveredWindow;
        internal static bool isHitTesting;

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static Control _protentialFocusControl;

        private static Rect _screenRect;

        // Mouse

        private static Vector2 _cursorPosition;

        // KeyBoard

        private static bool _isKeyDown;
        private static bool _isKeyUp;
        private static KeyCode _keyCode;

        #endregion


        /// <summary>
        /// 鼠标状态追踪器
        /// </summary>
        public sealed class MouseTracker
        {
            /// <summary>
            /// 该按钮是否已经按下
            /// </summary>
            public bool Pressing => _pressing;


            internal MouseTracker(MouseButton button)
            {
                _button = button;
                _buttonId = (int)button;
                _isLeftButton = button is MouseButton.LeftMouse;
            }


            internal static void TraceCursor()
            {
                anyPressing = false;
                var currentHoveredControl = HitTestUtility.Results.HoveredControl;

                if (ReferenceEquals(_hoveredControl, currentHoveredControl))
                {
                    return;
                }

                CursorLeave();

                _anyControlHovered = currentHoveredControl != null;
                _hoveredControl = currentHoveredControl;

                CursorEnter();
                HitTestUtility.Results.TransferTo(HitTestUtility.PreviousResults);
            }


            internal void Trace()
            {
                if (Input.GetMouseButton(_buttonId))
                {
                    TracePressing();
                }
                else
                {
                    TraceCommon();
                }
            }


            //------------------------------------------------------
            //
            //  Private Static Methods
            //
            //------------------------------------------------------

            #region Private Static Methods

            private static void CursorEnter()
            {
                if (!_anyControlHovered)
                {
                    return;
                }

                // Skip _hoveredControl
                var enteredControls = HitTestUtility.Results.Skip(1).Except(HitTestUtility.PreviousResults);
                var mouseEnterArgs = new RoutedEventArgs(_hoveredControl, Control.MouseEnterEvent);

                _hoveredControl.ControlStates |= ControlState.CursorDirectlyOver | ControlState.CursorOver;
                _hoveredControl.RaiseEvent(mouseEnterArgs);

                if (_anyControlDragging)
                {
                    var dragEnterArgs = new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _hoveredControl, Control.DragEnterEvent);

                    _hoveredControl.RaiseEvent(dragEnterArgs);

                    foreach (var control in enteredControls)
                    {
                        control.ControlStates |= ControlState.CursorOver;
                        mouseEnterArgs.Handled = false;
                        dragEnterArgs.Handled = false;

                        control.RaiseEvent(mouseEnterArgs);
                        control.RaiseEvent(dragEnterArgs);
                    }
                }
                else
                {
                    foreach (var control in enteredControls)
                    {
                        control.ControlStates |= ControlState.CursorOver;
                        mouseEnterArgs.Handled = false;

                        control.RaiseEvent(mouseEnterArgs);
                    }
                }
            }

            private static void CursorLeave()
            {
                if (!_anyControlHovered)
                {
                    return;
                }

                // Skip _hoveredControl witch cursor is directly leaved 
                var leavedControls = HitTestUtility.PreviousResults.Skip(1).Except(HitTestUtility.Results);
                var mouseLeaveArgs = new RoutedEventArgs(_hoveredControl, Control.MouseLeaveEvent);

                _hoveredControl.ControlStates &= ~(ControlState.CursorDirectlyOver | ControlState.CursorOver);
                _hoveredControl.RaiseEvent(mouseLeaveArgs);

                if (_anyControlDragging)
                {
                    var dragLeaveArgs = new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _hoveredControl, Control.DragLeaveEvent);
                    _hoveredControl.RaiseEvent(dragLeaveArgs);

                    foreach (var control in leavedControls)
                    {
                        control.ControlStates &= ~ControlState.CursorOver;

                        mouseLeaveArgs.Handled = false;
                        dragLeaveArgs.Handled = false;

                        control.RaiseEvent(mouseLeaveArgs);
                        control.RaiseEvent(dragLeaveArgs);
                    }
                }
                else
                {
                    foreach (var control in leavedControls)
                    {
                        control.ControlStates &= ~ControlState.CursorOver;
                        mouseLeaveArgs.Handled = false;

                        control.RaiseEvent(mouseLeaveArgs);
                    }
                }
            }

            #endregion


            //------------------------------------------------------
            //
            //  Private Methods
            //
            //------------------------------------------------------

            #region Private Methods

            private bool CanDrag()
            {
                return !ReferenceEquals(_pressingControl, _hoveredControl)
                    || (_cursorPosition - _pressingStartPos).SqrMagnitude() > 2500f;
            }

            private void TraceCommon()
            {
                if (!_pressing)
                {
                    return;
                }

                _pressing = false;

                if (_pressingWindowDraggable)
                {
                    _pressingWindow.draggable = true;
                    _pressingWindowDraggable = false;
                    _pressingWindow = null;
                }

                if (_anyControlHovered)
                {
                    _hoveredControl.RaiseEvent(new MouseButtonEventArgs(_button, _cursorPosition, _hoveredControl, Control.MouseUpEvent));
                }

                if (!_isLeftButton)
                {
                    return;
                }

                if (_anyControlDragging)
                {
                    _anyControlDragging = false;

                    _pressingControl.RaiseEvent(new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _pressingControl, Control.DragStopEvent));

                    if (_anyControlHovered && !ReferenceEquals(_pressingControl, _hoveredControl))
                    {
                        _hoveredControl.RaiseEvent(new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _hoveredControl, Control.DropEvent));
                    }

                    _pressingControl.ControlStates &= ~(ControlState.Dragging | ControlState.Pressing);
                    _pressingControl = null;
                }
                else if (_anyControlPressing)
                {
                    _anyControlPressing = false;

                    if (_anyControlHovered && ReferenceEquals(_pressingControl, _hoveredControl))
                    {
                        _pressingControl.RaiseEvent(new RoutedEventArgs(_pressingControl, ButtonBase.ClickEvent));
                    }

                    _pressingControl.ControlStates &= ~ControlState.Pressing;
                    _pressingControl = null;
                }

                _isControlAllowDrag = false;
            }

            private void TracePressing()
            {
                if (_pressing)
                {
                    anyPressing = true;

                    if (!_isLeftButton)
                    {
                        return;
                    }

                    if (_anyControlDragging)
                    {
                        if (_anyControlHovered && !ReferenceEquals(_pressingControl, _hoveredControl))
                        {
                            _hoveredControl.RaiseEvent(new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _pressingControl, Control.DragOverEvent));
                        }
                    }
                    else if (_isControlAllowDrag && CanDrag())
                    {
                        _anyControlDragging = true;
                        _pressingControl.ControlStates |= ControlState.Dragging;

                        _pressingControl.RaiseEvent(new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _pressingControl, Control.DragStartEvent));

                        if (_anyControlHovered && !ReferenceEquals(_pressingControl, _hoveredControl))
                        {
                            _hoveredControl.RaiseEvent(new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _pressingControl, Control.DragEnterEvent));
                        }
                    }

                    return;
                }

                _pressing = true;

                if (!_anyControlHovered)
                {
                    _protentialFocusControl = null;
                    return;
                }

                _hoveredControl.RaiseEvent(new MouseButtonEventArgs(_button, _cursorPosition, _hoveredControl, Control.MouseDownEvent));
                _protentialFocusControl = _hoveredControl;

                if (_isLeftButton)
                {
                    _anyControlPressing = true;
                    _isControlAllowDrag = _hoveredControl.AllowDrag;

                    _pressingControl = _hoveredControl;
                    _pressingControl.ControlStates |= ControlState.Pressing;

                    _pressingStartPos = _cursorPosition;
                }

                if (!_pressingWindowDraggable && hoveredWindow != null && hoveredWindow.draggable)
                {
                    _pressingWindow = hoveredWindow;
                    _pressingWindowDraggable = true;
                    _pressingWindow.draggable = false;
                }
            }

            #endregion


            internal static bool anyPressing;


            //------------------------------------------------------
            //
            //  Private Static Fields
            //
            //------------------------------------------------------

            #region Private Static Fields

            private static bool _anyControlHovered;
            private static bool _anyControlPressing;
            private static bool _anyControlDragging;

            private static bool _isControlAllowDrag;

            private static Control _hoveredControl;
            private static Control _pressingControl;

            private static Window _pressingWindow;
            private static bool _pressingWindowDraggable;

            #endregion


            //------------------------------------------------------
            //
            //  Private Fields
            //
            //------------------------------------------------------

            #region Private Fields

            private readonly MouseButton _button;
            private readonly int _buttonId;
            private readonly bool _isLeftButton;

            private bool _pressing;
            private Vector2 _pressingStartPos;

            #endregion
        }
    }
}
