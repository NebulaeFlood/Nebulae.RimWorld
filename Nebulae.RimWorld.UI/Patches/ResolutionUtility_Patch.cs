using HarmonyLib;
using RimWorld;
using Verse;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(ResolutionUtility), nameof(ResolutionUtility.SafeSetUIScale))]
    internal static class ResolutionUtility_Patch
    {
        [HarmonyPostfix]
        internal static void SafeSetUIScalePostfix()
        {
            UIPatch.InternalUIEvent.Invoke(UIPatch.HarmonyInstance, UIEventType.ScaleChanged);
        }
    }
}
