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
    /// 表示一个原版 UI 事件的处理器
    /// </summary>
    /// <param name="harmony">通过 Patch 获取到事件的 <see cref="Harmony"/> 实例</param>
    /// <param name="type">UI 事件的类型</param>
    public delegate void UIEventHandler(Harmony harmony, UIEventType type);


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
        public static event UIEventHandler UIEvent
        {
            add => InternalUIEvent.Add(value, value.Invoke);
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
