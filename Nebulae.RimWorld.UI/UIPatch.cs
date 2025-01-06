using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
