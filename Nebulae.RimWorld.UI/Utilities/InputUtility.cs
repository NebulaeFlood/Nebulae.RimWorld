using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Events;
using System;
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

        /// <summary>
        /// 键盘
        /// </summary>
        public static readonly KeyBoardTracker KeyBoard = new KeyBoardTracker();

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


        internal static void TraceWindow(List<Window> windowStack)
        {
            var cursorPos = Input.mousePosition / Prefs.UIScale;
            cursorPos.y = Verse.UI.screenHeight - cursorPos.y;

            _cursorPosition = cursorPos;
            _screenRect = new Rect(0f, 0f, Verse.UI.screenWidth, Verse.UI.screenHeight);

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
                    allowIndependentHitTest = false;
                    hoveredWindow = window;
                    return;
                }
            }

            allowIndependentHitTest = true;
            hoveredWindow = null;
        }


        //------------------------------------------------------
        //
        //  Internal Static Fields
        //
        //------------------------------------------------------

        #region Internal Static Fields

        internal static bool allowIndependentHitTest;
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
                anyButtonPressing = false;
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

                var enteredControls = HitTestUtility.Results.Except(HitTestUtility.PreviousResults);
                var mouseEnterArgs = new RoutedEventArgs(_hoveredControl, Control.MouseEnterEvent);

                _hoveredControl.ControlStates |= ControlState.CursorDirectlyOver;

                if (_anyControlDragging)
                {
                    var dragEnterArgs = new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _hoveredControl, Control.DragEnterEvent);

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

                var leavedControls = HitTestUtility.PreviousResults.Except(HitTestUtility.Results);
                var mouseLeaveArgs = new RoutedEventArgs(_hoveredControl, Control.MouseLeaveEvent);

                _hoveredControl.ControlStates &= ~ControlState.CursorDirectlyOver;

                if (_anyControlDragging)
                {
                    var dragLeaveArgs = new DragEventArgs(_pressingControl, MouseButton.LeftMouse, _cursorPosition, _hoveredControl, Control.DragLeaveEvent);

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
                anyButtonPressing = true;

                if (_pressing)
                {
                    if (!_isLeftButton)
                    {
                        return;
                    }

                    if (_anyControlDragging)
                    {
                        _pressingControl.DrawDragEffect(_cursorPosition);

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


            internal static bool anyButtonPressing;


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


        /// <summary>
        /// 键盘状态追踪器
        /// </summary>
        public sealed class KeyBoardTracker
        {
            /// <summary>
            /// <see cref="KeyCode"/> 记录的所有按键代码
            /// </summary>
            public static readonly KeyCode[] KeyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();


            internal KeyBoardTracker()
            {
                _nodePool = new Node[3];

                for (int i = 0; i < 3; i++)
                {
                    _nodePool[i] = new Node();
                }

                _nodeIndex = 0;
            }


            //------------------------------------------------------
            //
            //  Internal Methods
            //
            //------------------------------------------------------

            #region Internal Methods

            internal void Trace()
            {
                for (int i = KeyCodes.Length - 1; i >= 0; i--)
                {
                    var code = KeyCodes[i];

                    if (Input.GetKey(code))
                    {
                        Trace(code);
                    }
                }

                if (_isEmpty)
                {
                    return;
                }

                var node = _head;

                while (node != null)
                {
                    if (!Input.GetKey(node.key))
                    {
                        OnKeyUp(node.key);

                        var next = node.next;
                        Remove(node);
                        node = next;
                    }
                    else
                    {
                        node = node.next;
                    }
                }
            }

            internal void Trace(KeyCode keyCode)
            {
                if (Contains(keyCode))
                {
                    return;
                }

                OnKeyDown(keyCode);

                Node node;

                if (_nodeIndex < 3)
                {
                    node = _nodePool[_nodeIndex++];
                }
                else
                {
                    node = new Node();
                }

                node.key = keyCode;

                AddLast(node);
            }

            #endregion


            //------------------------------------------------------
            //
            //  Private Static Methods
            //
            //------------------------------------------------------

            #region Private Static Methods

            private static void OnKeyDown(KeyCode code)
            {
                if (InputControl.focusedControl is null || InputControl.focusedControl.ControlStates.HasState(ControlState.WillLossFocus))
                {
                    _protentialFocusControl?.RaiseEvent(new KeyEventArgs(code, _protentialFocusControl, Control.KeyDownEvent));
                }
                else
                {
                    InputControl.focusedControl.RaiseEvent(new KeyEventArgs(code, InputControl.focusedControl, Control.KeyDownEvent));
                }
            }

            private static void OnKeyUp(KeyCode code)
            {
                if (InputControl.focusedControl is null || InputControl.focusedControl.ControlStates.HasState(ControlState.WillLossFocus))
                {
                    _protentialFocusControl?.RaiseEvent(new KeyEventArgs(code, _protentialFocusControl, Control.KeyUpEvent));
                }
                else
                {
                    InputControl.focusedControl.RaiseEvent(new KeyEventArgs(code, InputControl.focusedControl, Control.KeyUpEvent));
                }
            }

            #endregion


            //------------------------------------------------------
            //
            //  Private Methods
            //
            //------------------------------------------------------

            #region Private Methods

            private void AddLast(Node node)
            {
                if (_head is null)
                {
                    _head = _tail = node;
                    _isEmpty = false;
                }
                else
                {
                    _tail.next = node;
                    node.prev = _tail;
                    _tail = node;
                }
            }

            private bool Contains(KeyCode keyCode)
            {
                if (_isEmpty)
                {
                    return false;
                }

                var node = _tail;

                while (node != null)
                {
                    if (node.key == keyCode)
                    {
                        return true;
                    }

                    node = node.prev;
                }

                return false;
            }

            private void Remove(Node node)
            {
                if (node.prev != null)
                {
                    node.prev.next = node.next;
                }
                else
                {
                    _head = node.next;
                }

                if (node.next != null)
                {
                    node.next.prev = node.prev;
                }
                else
                {
                    _tail = node.prev;
                }

                node.prev = null;
                node.next = null;

                _isEmpty = _head is null;

                if (_nodeIndex > 0)
                {
                    _nodePool[--_nodeIndex] = node;
                }
            }

            #endregion


            //------------------------------------------------------
            //
            //  Private Fields
            //
            //------------------------------------------------------

            #region Private Fields

            private Node _head;
            private Node _tail;

            private bool _isEmpty;

            private readonly Node[] _nodePool;
            private int _nodeIndex;

            #endregion


            private sealed class Node
            {
                public KeyCode key;

                public Node prev;
                public Node next;
            }
        }
    }
}
