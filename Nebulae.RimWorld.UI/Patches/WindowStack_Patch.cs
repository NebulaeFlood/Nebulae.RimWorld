﻿using HarmonyLib;
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
            UIUtility.CurrentEvent = Event.current;
            UIUtility.CurrentEventType = UIUtility.CurrentEvent.type;

            InputUtility.TraceWindow(___windows);
        }

        [HarmonyPostfix]
        internal static void WindowStackOnGUIPostfix()
        {
            InputUtility.TraceKeyBoard();

            if (!InputUtility.isHitTesting)
            {
                return;
            }

            InputUtility.MouseTracker.TraceCursor();
            InputUtility.LeftButton.Trace();
            InputUtility.RightButton.Trace();
            InputUtility.MiddleButton.Trace();
            HitTestUtility.Results.TransferTo(HitTestUtility.PreviousResults);
        }
    }
}
