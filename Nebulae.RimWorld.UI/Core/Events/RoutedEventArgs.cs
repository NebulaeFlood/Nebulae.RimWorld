using System;
using System.Diagnostics;

namespace Nebulae.RimWorld.UI.Core.Events
{
    /// <summary>
    /// 与路由事件相关联的状态信息和事件数据
    /// </summary>
    [DebuggerStepThrough]
    public class RoutedEventArgs : EventArgs
    {
        /// <summary>
        /// 事件是否已被处理
        /// </summary>
        public bool Handled;

        /// <summary>
        /// 触发事件的源
        /// </summary>
        public readonly object OriginalSource;

        /// <summary>
        /// 关联的路由事件
        /// </summary>
        public readonly RoutedEvent RoutedEvent;


        /// <summary>
        /// 初始化 <see cref="RoutedEventArgs"/> 的新实例
        /// </summary>
        /// <param name="originalSource">触发事件的源</param>
        /// <param name="routedEvent">关联的路由事件</param>
        public RoutedEventArgs(object originalSource, RoutedEvent routedEvent)
        {
            OriginalSource = originalSource;
            RoutedEvent = routedEvent;
        }
    }
}
