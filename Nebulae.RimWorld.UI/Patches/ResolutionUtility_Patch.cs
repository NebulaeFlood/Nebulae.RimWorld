using HarmonyLib;
using RimWorld;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(ResolutionUtility), nameof(ResolutionUtility.SafeSetUIScale))]
    internal static class ResolutionUtility_Patch
    {
        [HarmonyPostfix]
        internal static void SafeSetUIScalePostfix()
        {
            UIPatch.UIEvent.Invoke(UIEventType.ScaleChanged);
        }
    }
}
