using HarmonyLib;
using RimWorld;
using Verse;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(ResolutionUtility), nameof(ResolutionUtility.SafeSetUIScale))]
    internal static class ResolutionUtility_Patch
    {
        private static WeakEvent<object, float> _scaleChanged = new WeakEvent<object, float>();

        /// <summary>
        /// 当界面缩放 <see cref="Prefs.UIScale"/> 变化时触发的弱事件
        /// </summary>
        internal static event WeakEventHandler<object, float> ScaleChanged
        {
            add => _scaleChanged.Add(value);
            remove => _scaleChanged.Remove(value);
        }

        [HarmonyPostfix]
        internal static void SafeSetUIScalePostfix(float newScale)
        {
            _scaleChanged.Invoke(null, newScale);
        }
    }
}
