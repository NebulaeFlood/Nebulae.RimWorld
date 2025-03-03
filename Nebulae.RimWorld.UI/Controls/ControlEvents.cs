using Nebulae.RimWorld.UI.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nebulae.RimWorld.UI.Controls
{
    public abstract partial class Control : DependencyObject
    {
        /// <summary>
        /// 当光标进入控件区域时执行的方法
        /// </summary>
        protected internal virtual void OnCursorEnter()
        {
        }

        /// <summary>
        /// 当光标离开控件区域时执行的方法
        /// </summary>
        protected internal virtual void OnCursorLeave()
        {
        }

        /// <summary>
        /// 当控件正在被拖动时执行的方法
        /// </summary>
        /// <param name="cursorPos">光标的坐标（和窗口在同一坐标系）</param>
        protected internal virtual void OnDragging(Vector2 cursorPos)
        {
        }

        /// <summary>
        /// 当拖动的控件在此控件上方时执行的方法
        /// </summary>
        /// <param name="draggingControl">正在拖动的控件</param>
        protected internal virtual void OnDragOver(Control draggingControl)
        {
        }

        /// <summary>
        /// 当控件开始被拖动时执行的方法
        /// </summary>
        protected internal virtual void OnDragStart()
        {
        }

        /// <summary>
        /// 当控件停止被拖动时执行的方法
        /// </summary>
        protected internal virtual void OnDragStop()
        {
        }

        /// <summary>
        /// 当正在拖动的控件在此控件内被放下时执行的方法
        /// </summary>
        /// <param name="droppedControl">被放下的控件</param>
        protected internal virtual void OnDrop(Control droppedControl)
        {
        }

        /// <summary>
        /// 当 <see cref="IsEnabled"/> 变化时执行的方法
        /// </summary>
        /// <param name="isEnabled">是否在界面中启用该控件</param>
        protected virtual void OnIsEnabledChanged(bool isEnabled)
        {
        }

        /// <summary>
        /// 当鼠标按键被按下时执行的方法
        /// </summary>
        /// <param name="button">按下的按钮</param>
        protected internal virtual void OnMouseButtonDown(MouseButton button)
        {
        }

        /// <summary>
        /// 当鼠标按键正被按下时执行的方法
        /// </summary>
        /// <param name="button">按下的按钮</param>
        protected internal virtual void OnMouseButtonPressing(MouseButton button)
        {
        }

        /// <summary>
        /// 当鼠标按键被释放时执行的方法
        /// </summary>
        /// <param name="button">释放的按钮</param>
        protected internal virtual void OnMouseButtonUp(MouseButton button)
        {
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if ((_isArrangeValid || _isMeasureValid)
                && args.Metadata is ControlPropertyMetadata metadata)
            {
                if (metadata.Relation is ControlRelation.Measure)
                {
                    InvalidateMeasure();
                }
                else if (metadata.Relation is ControlRelation.Arrange)
                {
                    InvalidateArrange();
                }
            }
        }
    }
}
