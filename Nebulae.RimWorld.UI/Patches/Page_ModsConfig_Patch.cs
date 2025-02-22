﻿using HarmonyLib;
using Nebulae.RimWorld.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(Page_ModsConfig), "DoModInfo")]
    internal class Page_ModsConfig_Patch
    {
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> DoModInfoTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool patched = false;

            CodeInstruction[] codes = instructions.ToArray();

            for (int i = 0; i < codes.Length; i++)
            {
                var code = codes[i];

                if (!patched
                    && code.opcode == OpCodes.Ldstr 
                    && code.operand is "ModOptions")
                {
                    yield return codes[i++];
                    yield return codes[i++];
                    yield return codes[i++];

                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    i++;

                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Page_ModsConfig), "primaryModHandle"));
                    i++;

                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Page_ModsConfig_Patch), nameof(CreateDelegate)));

                    patched = true;
                }
                else
                {
                    yield return code;
                }
            }

            if (!patched)
            {
                "NebulaeFlood's Lib".Error($"Failed to patch method: {typeof(Page_ModsConfig)}.DoModInfo");
            }
        }

        private static Action CreateDelegate(Mod mod)
        {
            return () =>
            {
                Window settingWindow;

                if (mod is INebulaeMod nebulaeMod)
                {
                    settingWindow = nebulaeMod.GetSettingWindow();
                }
                else
                {
                    settingWindow = new Dialog_ModSettings(mod);
                }

                Find.WindowStack.Add(settingWindow);
            };
        }
    }
}
