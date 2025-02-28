using HarmonyLib;
using Nebulae.RimWorld.UI.Utilities;
using UnityEngine;
using Verse;

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

            CursorUtility.CursorPosition = cursorPos;
            CursorUtility.HoveredWindow = __instance.GetWindowAt(cursorPos);

            if (Input.GetMouseButton(0))
            {
                CursorUtility.IsPressing = true;
            }
            else if (CursorUtility.IsPressing)
            {
                CursorUtility.Click();
                CursorUtility.Drop();

                CursorUtility.DraggingControl = null;
                CursorUtility.PressingControl = null;

                if (CursorUtility.PressingWindowDraggable)
                {
                    CursorUtility.PressingWindow.draggable = true;
                    CursorUtility.PressingWindowDraggable = false;
                }

                CursorUtility.PressingWindow = null;

                CursorUtility.AnyDragging = false;
                CursorUtility.AnyPressing = false;
                CursorUtility.IsPressing = false;
            }
        }

        [HarmonyPostfix]
        internal static void WindowStackOnGUIPostfix()
        {
            CursorUtility.Press();
            CursorUtility.Drag();

            CursorUtility.HoveredControl = null;
        }
    }
}
