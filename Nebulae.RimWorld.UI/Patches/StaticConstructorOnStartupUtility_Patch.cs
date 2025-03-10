using HarmonyLib;
using Verse;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(StaticConstructorOnStartupUtility), nameof(StaticConstructorOnStartupUtility.CallAll))]
    internal static class StaticConstructorOnStartupUtility_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {
            StartUp.FinishQuests();
        }
    }
}
