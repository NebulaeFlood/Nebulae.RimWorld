using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(LanguageDatabase), nameof(LanguageDatabase.SelectLanguage))]
    internal static class LanguageDatabase_Patch
    {
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> SelectLanguageTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(LoadedLanguage), nameof(LoadedLanguage.folderName)));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertySetter(typeof(Prefs), nameof(Prefs.LangFolderName)));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LanguageDatabase_Patch), nameof(SelectLanguageWithNotification)));

        }

        private static void SelectLanguageWithNotification()
        {
            LongEventHandler.QueueLongEvent(() =>
            {
                PlayDataLoader.ClearAllPlayData();
                PlayDataLoader.LoadAllPlayData();
                UIPatch.InternalUIEvent.Invoke(UIPatch.HarmonyInstance, UIEventType.LanguageChanged);
            }, "LoadingLongEvent", doAsynchronously: true, exceptionHandler: null);
        }
    }
}
