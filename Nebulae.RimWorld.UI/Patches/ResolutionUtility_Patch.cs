using HarmonyLib;
using RimWorld;
using static Nebulae.RimWorld.UI.UIPatch;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(ResolutionUtility), nameof(ResolutionUtility.SafeSetUIScale))]
    internal static class ResolutionUtility_Patch
    {
        [HarmonyPostfix]
        internal static void SafeSetUIScalePostfix(float newScale)
        {
            ScaleChangedEvent.Invoke(HarmonyInstance, new ScaleChangedEventArgs(newScale));
        }
    }
}
