using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(StaticConstructorOnStartupUtility), nameof(StaticConstructorOnStartupUtility.CallAll))]
    internal static class StaticConstructorOnStartupUtility_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {

        }
    }
}
