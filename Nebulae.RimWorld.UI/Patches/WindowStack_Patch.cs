using HarmonyLib;
using Nebulae.RimWorld.UI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using static Nebulae.RimWorld.UI.Utilities.MouseUtility;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(WindowStack), nameof(WindowStack.WindowStackOnGUI))]
    internal static class WindowStack_Patch
    {
        [HarmonyPrefix]
        internal static void WindowStackOnGUIPrefix(WindowStack __instance)
        {
            Vector2 cursorPos = Input.mousePosition / Prefs.UIScale;
            cursorPos.y = Verse.UI.screenHeight - cursorPos.y;

            CursorPosition = cursorPos;
            HoveredWindow = __instance.GetWindowAt(cursorPos);

            CurrentEvent = Event.current.type;
            IsHitTesting = CurrentEvent is EventType.Repaint;
        }

        [HarmonyPostfix]
        internal static void WindowStackOnGUIPostfix()
        {
            if (!IsHitTesting)
            {
                return;
            }

            if (!ReferenceEquals(CurrentHoveredControl, HoveredControl))
            {
                if (AnyHovered)
                {
                    HoveredControl.OnCursorLeave();
                }

                AnyHovered = CurrentHoveredControl != null;
                HoveredControl = CurrentHoveredControl;
                IsHitAvailable = AnyHovered && HoveredControl.IsEnabled;

                if (AnyHovered)
                {
                    HoveredControl.OnCursorEnter();
                }
            }


            if (Input.GetMouseButton(0))
            {
                if (!LeftButtonPressing)
                {
                    IsPressing = true;
                    LeftButtonDown = true;
                    LeftButtonPressing = true;
                }

                if (IsHitAvailable)
                {
                    if (LeftButtonDown)
                    {
                        LeftButtonDown = false;
                        HoveredControl.OnMouseButtonDown(MouseButton.LeftMouse);

                        if (HoveredWindow != null)
                        {
                            AnyPressing = true;
                            IsDraggable = HoveredControl.IsDraggable;
                            PressingControl = HoveredControl;
                            PressStartPos = CursorPosition;
                            PressingWindow = HoveredWindow;
                            PressingWindowDraggable = HoveredWindow.draggable;
                            PressingWindow.draggable = false;
                        }
                    }

                    HoveredControl.OnMouseButtonPressing(MouseButton.LeftMouse);
                }
                else if (LeftButtonDown)
                {
                    LeftButtonDown = false;
                }
            }
            else if (LeftButtonPressing)
            {
                if (IsHitAvailable)
                {
                    HoveredControl.OnMouseButtonUp(MouseButton.LeftMouse);
                }

                if (AnyPressing
                    && ReferenceEquals(HoveredControl, PressingControl)
                    && PressingControl.IsEnabled
                    && PressingControl is ButtonBase button)
                {
                    button.Click();
                }

                if (AnyDragging)
                {
                    DraggingControl.OnDragStop();

                    if (IsHitAvailable)
                    {
                        HoveredControl.OnDrop(DraggingControl);
                    }
                }

                DraggingControl = null;
                PressingControl = null;

                if (PressingWindowDraggable)
                {
                    PressingWindow.draggable = true;
                    PressingWindowDraggable = false;
                }

                PressingWindow = null;

                AnyDragging = false;
                AnyPressing = false;
                IsDraggable = false;
                LeftButtonDown = false;
                LeftButtonPressing = false;
            }

            if (Input.GetMouseButton(2))
            {
                if (!MiddleButtonPressing)
                {
                    IsPressing = true;
                    MiddleButtonDown = true;
                    MiddleButtonPressing = true;
                }

                if (IsHitAvailable)
                {
                    if (MiddleButtonDown)
                    {
                        MiddleButtonDown = false;
                        HoveredControl.OnMouseButtonDown(MouseButton.MiddleMouse);
                    }

                    HoveredControl.OnMouseButtonPressing(MouseButton.MiddleMouse);
                }
                else if (MiddleButtonDown)
                {
                    MiddleButtonDown = false;
                }
            }
            else if (MiddleButtonPressing)
            {
                if (IsHitAvailable)
                {
                    HoveredControl.OnMouseButtonUp(MouseButton.MiddleMouse);
                }

                MiddleButtonDown = false;
                MiddleButtonPressing = false;
            }

            if (Input.GetMouseButton(1))
            {
                if (!RightButtonPressing)
                {
                    IsPressing = true;
                    RightButtonDown = true;
                    RightButtonPressing = true;
                }

                if (IsHitAvailable)
                {
                    if (RightButtonDown)
                    {
                        RightButtonDown = false;
                        HoveredControl.OnMouseButtonDown(MouseButton.RightMouse);
                    }

                    HoveredControl.OnMouseButtonPressing(MouseButton.RightMouse);
                }
                else if (RightButtonDown)
                {
                    RightButtonDown = false;
                }
            }
            else if (RightButtonPressing)
            {
                if (IsHitAvailable)
                {
                    HoveredControl.OnMouseButtonUp(MouseButton.RightMouse);
                }

                RightButtonDown = false;
                RightButtonPressing = false;
            }

            if (!LeftButtonPressing && !MiddleButtonPressing && !RightButtonPressing)
            {
                IsPressing = false;
            }

            if (AnyDragging)
            {
                DraggingControl.OnDragging(CursorPosition);

                if (AnyHovered)
                {
                    HoveredControl.OnDragOver(DraggingControl);
                }
            }
            else if (IsDraggable && CanDrag())
            {
                AnyDragging = true;
                DraggingControl = PressingControl;
                DraggingControl.OnDragStart();
            }

            CurrentHoveredControl = null;
        }


        private static bool CanDrag()
        {
            return !ReferenceEquals(HoveredControl, PressingControl)
                || (CursorPosition - PressStartPos).SqrMagnitude() > 2500f;
        }
    }
}
