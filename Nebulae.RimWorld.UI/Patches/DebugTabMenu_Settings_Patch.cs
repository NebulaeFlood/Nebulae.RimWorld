using HarmonyLib;
using LudeonTK;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(DebugTabMenu_Settings), nameof(DebugTabMenu_Settings.InitActions))]
    internal static class DebugTabMenu_Settings_Patch
    {
        [HarmonyPostfix]
        internal static void InitActionsPostfix(DebugTabMenu_Settings __instance, ref DebugActionNode __result)
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
