using HarmonyLib;
using LudeonTK;
using Nebulae.RimWorld.UI.Controls;
using System;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(DebugTabMenu_Settings), nameof(DebugTabMenu_Settings.InitActions))]
    internal class DebugTabMenu_Settings_Patch
    {
        [HarmonyPostfix]
        internal static void InitActionsPostfix(DebugTabMenu_Settings __instance)
        {
            Type type = typeof(Control);
            MethodInfo method = AccessTools.Method(typeof(DebugTabMenu_Settings), "AddNode");

            AccessTools.Method(typeof(DebugTabMenu_Settings), "AddNode").Invoke(__instance, new object[]
            {
                AccessTools.Field(type, nameof(Control.ClickForceArrange)),
                "Aether Control"
            });
            AccessTools.Method(typeof(DebugTabMenu_Settings), "AddNode").Invoke(__instance, new object[]
            {
                AccessTools.Field(type, nameof(Control.ClickForceMeasure)),
                "Aether Control"
            });
            AccessTools.Method(typeof(DebugTabMenu_Settings), "AddNode").Invoke(__instance, new object[]
            {
                AccessTools.Field(type, nameof(Control.DrawFullRegion)),
                "Aether Control"
            });
            AccessTools.Method(typeof(DebugTabMenu_Settings), "AddNode").Invoke(__instance, new object[]
            {
                AccessTools.Field(type, nameof(Control.DrawRegion)),
                "Aether Control"
            });
            AccessTools.Method(typeof(DebugTabMenu_Settings), "AddNode").Invoke(__instance, new object[]
            {
                AccessTools.Field(type, nameof(Control.ShowInfo)),
                "Aether Control"
            });
        }
    }
}
