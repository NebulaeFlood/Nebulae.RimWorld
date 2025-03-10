using HarmonyLib;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(WindowStack), nameof(WindowStack.WindowStackOnGUI))]
    internal static class WindowStack_Patch
    {
        internal static Event CurrentEvent;
        internal static EventType CurrentEventType;
        internal static List<Window> Windows;


        [HarmonyPrefix]
        internal static void WindowStackOnGUIPrefix(List<Window> ___windows)
        {
            CurrentEvent = Event.current;
            CurrentEventType = CurrentEvent.type;
            Windows = ___windows;

            GameStateEventUtility.CheckState(Current.ProgramState);
            MouseUtility.CheckState(Windows);
            KeyboardEventUtility.CheckEvent(CurrentEvent, CurrentEventType);
        }

        [HarmonyPostfix]
        internal static void WindowStackOnGUIPostfix()
        {
            MouseUtility.Update();
            PopupWindowUtility.CheckState(Windows);
        }
    }
}
