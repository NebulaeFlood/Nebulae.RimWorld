using HarmonyLib;
using Nebulae.RimWorld.WeakEventManagers;
using System.Reflection;
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
    public static class UIPatch
    {
        internal const string UniqueId = "Nebulae.RimWorld.UI";

        /// <summary>
        /// 原版 UI Patch
        /// </summary>
        internal static readonly Harmony HarmonyInstance;

        /// <summary>
        /// 原版 UI 事件管理器
        /// </summary>
        public static readonly UIEventManager UIEvent = new UIEventManager();


        static UIPatch()
        {
            HarmonyInstance = new Harmony(UniqueId);
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            HarmonyInstance.Patch(AccessTools.Method(typeof(Root), nameof(Root.Update)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Dispatcher), nameof(Dispatcher.Update))));
        }
    }


    /// <summary>
    /// 通过 <see cref="UIPatch"/> 创建的原版 UI 事件的订阅者
    /// </summary>
    public interface IUIEventListener
    {
        /// <summary>
        /// 原版 UI 事件的处理器
        /// </summary>
        /// <param name="type">UI 事件的类型</param>
        void HandleUIEvent(UIEventType type);
    }


    /// <summary>
    /// UI 事件管理器
    /// </summary>
    public sealed class UIEventManager : WeakEventManager<IUIEventListener>
    {
        internal UIEventManager()
        {
        }

        /// <summary>
        /// 调用所有订阅者的事件处理方法
        /// </summary>
        /// <param name="eventType">事件类型</param>
        internal void Invoke(UIEventType eventType)
        {
            var subscribers = GetSubcribers();

            for (int i = subscribers.Count - 1; i >= 0; i--)
            {
                subscribers[i].HandleUIEvent(eventType);
            }
        }
    }
}
