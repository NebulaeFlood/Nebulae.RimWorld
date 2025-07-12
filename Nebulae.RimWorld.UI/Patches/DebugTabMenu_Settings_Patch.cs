using HarmonyLib;
using LudeonTK;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using System;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(DebugTabMenu_Settings), nameof(DebugTabMenu_Settings.InitActions))]
    internal static class DebugTabMenu_Settings_Patch
    {
        [HarmonyPostfix]
        internal static void InitActionsPostfix(DebugTabMenu_Settings __instance)
        {
            try
            {
                typeof(DebugTabMenu_Settings).GetMethod("AddNode", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(__instance, new object[] { typeof(UIUtility).GetField(nameof(UIUtility.DebugMode), BindingFlags.Static | BindingFlags.Public), StartUp.Lib });
            }
            catch (Exception e)
            {
                StartUp.Lib.Error($"Cannot add debug mode switch to {typeof(DebugTabMenu_Settings)}.\n---> {e}");
            }
        }
    }
}
