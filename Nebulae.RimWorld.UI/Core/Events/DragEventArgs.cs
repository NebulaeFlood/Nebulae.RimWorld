using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nebulae.RimWorld.UI.Core.Events
{
    /// <summary>
    /// 拖拽事件数据
    /// </summary>
    public class DragEventArgs : MouseButtonEventArgs
    {
        /// <summary>
        /// 被拖拽的控件
        /// </summary>
        public readonly Control Target;


        /// <summary>
        /// 初始化 <see cref="DragEventArgs"/> 的新实例
        /// </summary>
        /// <param name="target">被拖拽的控件</param>
        /// <param name="button">触发事件的鼠标按钮</param>
        /// <param name="position">触发事件时鼠标的坐标</param>
        /// <param name="originalSource">触发事件的源</param>
        /// <param name="routedEvent">关联的路由事件</param>
        public DragEventArgs(Control target, MouseButton button, Vector2 position, object originalSource, RoutedEvent routedEvent) : base(button, position, originalSource, routedEvent)
        {
            Target = target;
        }
    }
}
