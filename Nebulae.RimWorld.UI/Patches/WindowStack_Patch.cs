using HarmonyLib;
using Nebulae.RimWorld.UI.Utilities;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(WindowStack), nameof(WindowStack.WindowStackOnGUI))]
    internal static class WindowStack_Patch
    {
        [HarmonyPrefix]
        internal static void WindowStackOnGUIPrefix(List<Window> ___windows)
        {
            InputUtility.isHitTesting = Event.current.type is EventType.Layout;
            InputUtility.TraceWindow(___windows);
        }

        [HarmonyPostfix]
        internal static void WindowStackOnGUIPostfix()
        {
            if (InputUtility.isHitTesting)
            {
                InputUtility.MouseTracker.TraceCursor();
                InputUtility.LeftButton.Trace();
                InputUtility.RightButton.Trace();
                InputUtility.MiddleButton.Trace();
                HitTestUtility.Results.Clear();

                InputUtility.KeyBoard.Trace();
            }
            else
            {
                InputUtility.MouseTracker.DrawEffect();
            }
        }
    }
}
