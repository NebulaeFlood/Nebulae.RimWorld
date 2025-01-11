using HarmonyLib;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// UI 事件的类型
    /// </summary>
    public enum UIEventType
    {
        /// <summary>
        /// 语言被更改
        /// </summary>
        LanguageChanged,

        /// <summary>
        /// 缩放系数被更改
        /// </summary>
        ScaleChanged
    }

    /// <summary>
    /// 原版 UI 补丁
    /// </summary>
    [StaticConstructorOnStartup]
    public static class UIPatch
    {
        internal const string UniqueId = "Nebulae.RimWorld.UI";

        internal static readonly WeakEvent<Harmony, UIEventType> InternalUIEvent = new WeakEvent<Harmony, UIEventType>();


        /// <summary>
        /// 原版 UI 事件
        /// </summary>
        public static event WeakEventHandler<Harmony, UIEventType> UIEvent
        {
            add => InternalUIEvent.Add(value);
            remove => InternalUIEvent.Remove(value);
        }

        /// <summary>
        /// 原版 UI Patch
        /// </summary>
        public static readonly Harmony HarmonyInstance;


        static UIPatch()
        {
            HarmonyInstance = new Harmony(UniqueId);
            HarmonyInstance.PatchAll();
        }
    }
}
