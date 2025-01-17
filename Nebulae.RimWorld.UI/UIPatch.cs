﻿using HarmonyLib;
using Nebulae.RimWorld.WeakEventManagers;
using RimWorld;
using System.Collections.Generic;
using Verse;
using CollectionWeakReference = System.WeakReference<Nebulae.RimWorld.IWeakCollection>;

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
            HarmonyInstance.PatchAll();
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
        void UIEventHandler(UIEventType type);
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
        public void Invoke(UIEventType eventType)
        {
            Purge();

            Subscribers.ForEach(x =>
            {
                if (x.TryGetTarget(out var listener))
                {
                    listener.UIEventHandler(eventType);
                }
            });
        }
    }
}
