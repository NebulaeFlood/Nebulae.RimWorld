using UnityEngine;
using UnityEngine.UIElements;

namespace Nebulae.RimWorld.UI.Core.Events
{
    /// <summary>
    /// 鼠标按钮事件数据
    /// </summary>
    public class MouseButtonEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 触发事件的鼠标按钮
        /// </summary>
        public readonly MouseButton Button;

        /// <summary>
        /// 触发事件时鼠标的坐标
        /// </summary>
        public readonly Vector2 Position;


        /// <summary>
        /// 初始化 <see cref="MouseButtonEventArgs"/> 的新实例
        /// </summary>
        /// <param name="button">触发事件的鼠标按钮</param>
        /// <param name="position">触发事件时鼠标的坐标</param>
        /// <param name="originalSource">触发事件的源</param>
        /// <param name="routedEvent">关联的路由事件</param>
        public MouseButtonEventArgs(MouseButton button, Vector2 position, object originalSource, RoutedEvent routedEvent) : base(originalSource, routedEvent)
        {
            Button = button;
            Position = position;
        }
    }
}
