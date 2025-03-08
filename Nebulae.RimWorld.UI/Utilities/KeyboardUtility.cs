using Nebulae.RimWorld.WeakEventManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="handled">是否已处理事件</param>
        void HandleKeyboardEvent(ref bool handled);
    }


    /// <summary>
    /// 键盘帮助类
    /// </summary>
    public static class KeyboardUtility
    {
        internal static readonly KeyboardEvent Event = new KeyboardEvent();

        /// <summary>
        /// 添加键盘事件处理器
        /// </summary>
        /// <param name="listener">要订阅事件的对象</param>
        public static void AddHandler(IKeyboardEventListener listener)
        {
            Event.Manage(listener);
        }

        /// <summary>
        /// 移除键盘事件处理器
        /// </summary>
        /// <param name="listener">要取消订阅事件的对象</param>
        public static void RemoveHandler(IKeyboardEventListener listener)
        {
            Event.Remove(listener);
        }


        internal sealed class KeyboardEvent : WeakEventManager<IKeyboardEventListener>
        {
            internal void Invoke()
            {
                bool handled = false;

                var subscribers = GetSubcribers();

                for (int i = subscribers.Count - 1; i >= 0; i--)
                {
                    subscribers[i].HandleKeyboardEvent(ref handled);

                    if (handled)
                    {
                        return;
                    }
                }
            }
        }
    }
}
