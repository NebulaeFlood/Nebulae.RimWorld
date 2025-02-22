using HarmonyLib;
using Nebulae.RimWorld.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(Dialog_Options), "DoModOptions")]
    internal static class Dialog_Options_Patch
    {
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> DoModOptionsTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool patched = false;

            MethodInfo method = AccessTools.Method(typeof(WindowStack), nameof(WindowStack.Add));

            ConstructorInfo constructorInfo = AccessTools.Constructor(typeof(Dialog_ModSettings), parameters: new Type[] { typeof(Mod) });

            foreach (var code in instructions)
            {
                if (patched)
                {
                    yield return code;
                }
                else if (code.opcode == OpCodes.Callvirt && (MethodInfo)code.operand == method)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Dialog_Options_Patch), nameof(ReplaceModSettingWindow)));
                    yield return code;

                    patched = true;
                }
                else
                {
                    yield return code;
                }
            }

            if (!patched)
            {
                "NebulaeFlood's Lib".Error($"Failed to patch method: {typeof(Dialog_Options)}.DoModOptions");
            }
        }

        private static Window ReplaceModSettingWindow(Window window, Mod mod)
        {
            if (mod is INebulaeMod nebulaeMod)
            {
                return nebulaeMod.GetSettingWindow();
            }
            else
            {
                return window;
            }
        }
    }
}
