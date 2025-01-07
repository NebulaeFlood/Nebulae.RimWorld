using HarmonyLib;
using Verse;

namespace Nebulae.RimWorld.UI
{
    [StaticConstructorOnStartup]
    internal static class UIPatch
    {
        internal const string UniqueId = "Nebulae.RimWorld.UI";

        internal static readonly Harmony HarmonyInstance;

        static UIPatch()
        {
            HarmonyInstance = new Harmony(UniqueId);
            HarmonyInstance.PatchAll();
        }
    }
}
