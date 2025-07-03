using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Core.Events
{
    /// <summary>
    /// 键盘按键事件数据
    /// </summary>
    public class KeyEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 与此事件关联的键
        /// </summary>
        public readonly KeyCode Key;


        /// <summary>
        /// 初始化 <see cref="KeyEventArgs"/> 的新实例
        /// </summary>
        /// <param name="key">与此事件关联的键</param>
        /// <param name="originalSource">触发事件的源</param>
        /// <param name="routedEvent">关联的路由事件</param>
        public KeyEventArgs(KeyCode key, object originalSource, RoutedEvent routedEvent) : base(originalSource, routedEvent)
        {
            Key = key;
        }
    }
}
