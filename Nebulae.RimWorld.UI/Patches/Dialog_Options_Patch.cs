﻿using HarmonyLib;
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
            ConstructorInfo constructorInfo = AccessTools.Constructor(typeof(Dialog_ModSettings), parameters: new Type[] { typeof(Mod) });
            foreach (var code in instructions)
            {
                yield return code.opcode == OpCodes.Newobj && (ConstructorInfo)code.operand == constructorInfo
                    ? new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Dialog_Options_Patch), nameof(CreateModSettingWindow)))
                    : code;
            }
        }

        private static Window CreateModSettingWindow(Mod mod)
        {
            Window settingWindow;

            if (mod is NebulaeModBase nebulaeMod)
            {
                settingWindow = nebulaeMod.SettingWindow;
            }
            else
            {
                settingWindow = new Dialog_ModSettings(mod);
            }

            return settingWindow;
        }
    }
}
