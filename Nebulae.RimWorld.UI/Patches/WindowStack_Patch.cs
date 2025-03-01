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
                if (!CursorUtility.IsPressing)
                {
                    CursorUtility.IsPressing = true;
                    CursorUtility.PressStart = true;
                }
            }
            else if (CursorUtility.IsPressing)
            {
                CursorUtility.ReleaseButton();
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
