using Nebulae.RimWorld.WeakEventManagers;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 键盘事件监听器接口
    /// </summary>
    public interface IKeyboardEventListener
    {
        /// <summary>
        /// 当按键按下或松开时执行的方法
        /// </summary>
        /// <param name="currentEvent">当前的事件</param>
        /// <param name="eventType">当前事件的类型</param>
        /// <param name="handled">是否已处理事件</param>
        void HandleKeyboardEvent(Event currentEvent, EventType eventType, ref bool handled);
    }


    /// <summary>
    /// 键盘事件帮助类
    /// </summary>
    public static class KeyboardEventUtility
    {
        private static readonly KeyboardEvent _event = new KeyboardEvent();


        /// <summary>
        /// 添加键盘事件监听器
        /// </summary>
        /// <param name="listener">要监听事件的对象</param>
        public static void AddListener(IKeyboardEventListener listener)
        {
            _event.Manage(listener);
        }

        /// <summary>
        /// 移除键盘事件监听器
        /// </summary>
        /// <param name="listener">要取消监听事件的对象</param>
        public static void RemoveListener(IKeyboardEventListener listener)
        {
            _event.Remove(listener);
        }


        internal static void CheckEvent(Event currentEvent, EventType type)
        {
            if (type is EventType.KeyDown
                || type is EventType.KeyUp)
            {
                _event.Invoke(currentEvent, type);
            }
        }


        internal sealed class KeyboardEvent : WeakEventManager<IKeyboardEventListener>
        {
            internal void Invoke(Event currentEvent, EventType eventType)
            {
                bool handled = false;

                var subscribers = GetSubcribers();

                for (int i = subscribers.Count - 1; i >= 0; i--)
                {
                    subscribers[i].HandleKeyboardEvent(currentEvent, eventType, ref handled);

                    if (handled)
                    {
                        return;
                    }
                }
            }
        }
    }
}
