using Nebulae.RimWorld.UI.Core;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    public partial class Control
    {
        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------

        #region Public Events

        #region DragEnter
        /// <summary>
        /// 当控件被拖拽到此控件范围内时发生
        /// </summary>
        public event DragEventHandler DragEnter
        {
            add { AddHandler(DragEnterEvent, value); }
            remove { RemoveHandler(DragEnterEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="DragEnter"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent DragEnterEvent =
            RoutedEvent.Register(nameof(DragEnter), RoutingStrategy.Direct, typeof(Control), typeof(DragEventArgs));
        #endregion

        #region DragLeave
        /// <summary>
        /// 当控件被拖拽从此控件范围外时发生
        /// </summary>
        public event DragEventHandler DragLeave
        {
            add { AddHandler(DragLeaveEvent, value); }
            remove { RemoveHandler(DragLeaveEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="DragLeave"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent DragLeaveEvent =
            RoutedEvent.Register(nameof(DragLeave), RoutingStrategy.Direct, typeof(Control), typeof(DragEventArgs));
        #endregion

        #region DragOver
        /// <summary>
        /// 当被拖拽的控件在此控件范围内时发生
        /// </summary>
        public event DragEventHandler DragOver
        {
            add { AddHandler(DragOverEvent, value); }
            remove { RemoveHandler(DragOverEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="DragOver"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent DragOverEvent =
            RoutedEvent.Register(nameof(DragOver), RoutingStrategy.TopHit, typeof(Control), typeof(DragEventArgs));
        #endregion

        #region DragStart
        /// <summary>
        /// 当控件开始被拖拽时发生
        /// </summary>
        public event DragEventHandler DragStart
        {
            add { AddHandler(DragStartEvent, value); }
            remove { RemoveHandler(DragStartEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="DragStart"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent DragStartEvent =
            RoutedEvent.Register(nameof(DragStart), RoutingStrategy.Direct, typeof(Control), typeof(DragEventArgs));
        #endregion

        #region DragStop
        /// <summary>
        /// 当控件不再被拖拽时发生
        /// </summary>
        public event DragEventHandler DragStop
        {
            add { AddHandler(DragStopEvent, value); }
            remove { RemoveHandler(DragStopEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="DragStop"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent DragStopEvent =
            RoutedEvent.Register(nameof(DragStop), RoutingStrategy.Direct, typeof(Control), typeof(DragEventArgs));
        #endregion

        #region Drop
        /// <summary>
        /// 当被拖拽的控件在此控件范围内释放时发生
        /// </summary>
        public event DragEventHandler Drop
        {
            add { AddHandler(DropEvent, value); }
            remove { RemoveHandler(DropEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="Drop"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent DropEvent =
            RoutedEvent.Register(nameof(Drop), RoutingStrategy.TopHit, typeof(Control), typeof(DragEventArgs));
        #endregion

        #region KeyDown
        /// <summary>
        /// 当焦点在该控件上时按下键盘按键后发生
        /// </summary>
        public event KeyEventHandler KeyDown
        {
            add { AddHandler(KeyDownEvent, value); }
            remove { RemoveHandler(KeyDownEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="KeyDown"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent KeyDownEvent =
            RoutedEvent.Register(nameof(KeyDown), RoutingStrategy.Bubble, typeof(Control), typeof(KeyEventArgs));
        #endregion

        #region KeyUp
        /// <summary>
        /// 当焦点在该控件上时松开键盘按键后发生
        /// </summary>
        public event KeyEventHandler KeyUp
        {
            add { AddHandler(KeyUpEvent, value); }
            remove { RemoveHandler(KeyUpEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="KeyUp"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent KeyUpEvent =
            RoutedEvent.Register(nameof(KeyUp), RoutingStrategy.Bubble, typeof(Control), typeof(KeyEventArgs));
        #endregion

        #region MouseDown
        /// <summary>
        /// 当鼠标按钮在控件上按下时发生
        /// </summary>
        public event MouseButtonEventHandler MouseDown
        {
            add { AddHandler(MouseDownEvent, value); }
            remove { RemoveHandler(MouseDownEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="MouseDown"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent MouseDownEvent =
            RoutedEvent.Register(nameof(MouseDown), RoutingStrategy.TopHit, typeof(Control), typeof(MouseButtonEventArgs));
        #endregion

        #region MouseEnter
        /// <summary>
        /// 当鼠标进入控件时发生
        /// </summary>
        public event RoutedEventHandler MouseEnter
        {
            add { AddHandler(MouseEnterEvent, value); }
            remove { RemoveHandler(MouseEnterEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="MouseEnter"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent MouseEnterEvent =
            RoutedEvent.Register(nameof(MouseEnter), RoutingStrategy.Direct, typeof(Control), typeof(RoutedEventArgs));
        #endregion

        #region MouseLeave
        /// <summary>
        /// 当鼠标离开控件时发生
        /// </summary>
        public event RoutedEventHandler MouseLeave
        {
            add { AddHandler(MouseLeaveEvent, value); }
            remove { RemoveHandler(MouseLeaveEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="MouseLeave"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent MouseLeaveEvent =
            RoutedEvent.Register(nameof(MouseLeave), RoutingStrategy.Direct, typeof(Control), typeof(RoutedEventArgs));
        #endregion

        #region MouseUp
        /// <summary>
        /// 当鼠标按钮在控件上释放时发生
        /// </summary>
        public event MouseButtonEventHandler MouseUp
        {
            add { AddHandler(MouseUpEvent, value); }
            remove { RemoveHandler(MouseUpEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="MouseUp"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent MouseUpEvent =
            RoutedEvent.Register(nameof(MouseUp), RoutingStrategy.TopHit, typeof(Control), typeof(MouseButtonEventArgs));
        #endregion

        #endregion


        static Control()
        {
            DragEnterEvent.RegisterClassHandler(typeof(Control), new DragEventHandler(OnDragEnterThunk));
            DragLeaveEvent.RegisterClassHandler(typeof(Control), new DragEventHandler(OnDragLeaveThunk));
            DragOverEvent.RegisterClassHandler(typeof(Control), new DragEventHandler(OnDragOverThunk));
            DropEvent.RegisterClassHandler(typeof(Control), new DragEventHandler(OnDropThunk));
            DragStartEvent.RegisterClassHandler(typeof(Control), new DragEventHandler(OnDragStartThunk));
            DragStopEvent.RegisterClassHandler(typeof(Control), new DragEventHandler(OnDragStopThunk));
            KeyDownEvent.RegisterClassHandler(typeof(Control), new KeyEventHandler(OnKeyDownThunk));
            KeyUpEvent.RegisterClassHandler(typeof(Control), new KeyEventHandler(OnKeyUpThunk));
            MouseDownEvent.RegisterClassHandler(typeof(Control), new MouseButtonEventHandler(OnMouseDownThunk));
            MouseEnterEvent.RegisterClassHandler(typeof(Control), new RoutedEventHandler(OnMouseEnterThunk));
            MouseLeaveEvent.RegisterClassHandler(typeof(Control), new RoutedEventHandler(OnMouseLeaveThunk));
            MouseUpEvent.RegisterClassHandler(typeof(Control), new MouseButtonEventHandler(OnMouseUpThunk));
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 为指定的路由事件添加处理程序
        /// </summary>
        /// <param name="routedEvent">要添加处理程序的路由事件</param>
        /// <param name="handler">路由事件的处理程序</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="routedEvent"/> 或 <paramref name="handler"/> 为 <see langword="null"/> 时发生。</exception>
        public void AddHandler(RoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent is null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (_handlerCollections.TryGetValue(routedEvent, out var handlers))
            {
                handlers.Add(RoutedEventHandlerFactory.Convert(handler, routedEvent.ArgType));
            }
            else
            {
                _handlerCollections[routedEvent] = new ControlRoutedEventHandlerCollection(RoutedEventHandlerFactory.Convert(handler, routedEvent.ArgType));
            }
        }

        /// <summary>
        /// 引发指定的路由事件，并传递事件数据
        /// </summary>
        /// <param name="args">包含事件数据并标识要引发的事件的 <see cref="RoutedEventArgs"/></param>
        public void RaiseEvent(RoutedEventArgs args)
        {
            switch (args.RoutedEvent.Strategy)
            {
                case RoutingStrategy.Direct:
                    InvokeEvent(this, args);
                    break;
                case RoutingStrategy.TopHit:
                    TopHitEvent(args);
                    break;
                case RoutingStrategy.Bubble:
                    BubbleEvent(this, args);
                    break;
                case RoutingStrategy.Tunnel:
                    TunnelEvent(this, args);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 从指定的路由事件中移除处理程序
        /// </summary>
        /// <param name="routedEvent">要移除处理程序的路由事件</param>
        /// <param name="handler">要移除的处理程序</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="routedEvent"/> 或 <paramref name="handler"/> 为 <see langword="null"/> 时发生。</exception>
        public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent is null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!_handlerCollections.TryGetValue(routedEvent, out var handlers))
            {
                return;
            }

            handlers.Remove(handler);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 当 <see cref="DragEnter"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnDragEnter(DragEventArgs e) { }

        /// <summary>
        /// 当 <see cref="DragLeave"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnDragLeave(DragEventArgs e) { }

        /// <summary>
        /// 当 <see cref="DragOver"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnDragOver(DragEventArgs e) { }

        /// <summary>
        /// 当 <see cref="DragStart"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnDragStart(DragEventArgs e) { }

        /// <summary>
        /// 当 <see cref="DragStop"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnDragStop(DragEventArgs e) { }

        /// <summary>
        /// 当 <see cref="Drop"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnDrop(DragEventArgs e) { }

        /// <summary>
        /// 当 <see cref="KeyDown"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnKeyDown(KeyEventArgs e) { }

        /// <summary>
        /// 当 <see cref="KeyUp"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnKeyUp(KeyEventArgs e) { }

        /// <summary>
        /// 当 <see cref="MouseDown"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnMouseDown(MouseButtonEventArgs e) { }

        /// <summary>
        /// 当 <see cref="MouseEnter"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnMouseEnter(RoutedEventArgs e) { }

        /// <summary>
        /// 当 <see cref="MouseLeave"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnMouseLeave(RoutedEventArgs e) { }

        /// <summary>
        /// 当 <see cref="MouseUp"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnMouseUp(MouseButtonEventArgs e) { }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static void BubbleEvent(Control sender, RoutedEventArgs args)
        {
            while (sender != null)
            {
                if (sender._handlerCollections.TryGetValue(args.RoutedEvent, out var handlers))
                {
                    handlers.Invoke(sender, args);

                    if (args.Handled)
                    {
                        return;
                    }
                }
                else if (args.RoutedEvent.TryGetClassHandler(sender.Type, out var classHandler))
                {
                    classHandler.Invoke(sender, args);

                    if (args.Handled)
                    {
                        return;
                    }
                }

                sender = sender._parent;
            }
        }

        private static void InvokeEvent(Control sender, RoutedEventArgs args)
        {
            if (sender._handlerCollections.TryGetValue(args.RoutedEvent, out var handlers))
            {
                handlers.Invoke(sender, args);
            }
            else if (args.RoutedEvent.TryGetClassHandler(sender.Type, out var classHandler))
            {
                classHandler.Invoke(sender, args);
            }
        }

        private static void TopHitEvent(RoutedEventArgs args)
        {
            var node = HitTestUtility.Results.Head;

            while (node != null)
            {
                if (node.Data._handlerCollections.TryGetValue(args.RoutedEvent, out var handlers))
                {
                    handlers.Invoke(node.Data, args);

                    if (args.Handled)
                    {
                        return;
                    }
                }
                else if (args.RoutedEvent.TryGetClassHandler(node.Data.Type, out var classHandler))
                {
                    classHandler.Invoke(node.Data, args);

                    if (args.Handled)
                    {
                        return;
                    }
                }

                node = node.next;
            }
        }

        private static void TunnelEvent(Control sender, RoutedEventArgs args)
        {
            var eventRoute = EventRoute.Fetch();

            while (sender != null)
            {
                if (sender._handlerCollections.TryGetValue(args.RoutedEvent, out var handlers))
                {
                    handlers.Fill(sender, args.RoutedEvent, eventRoute);
                }
                else if (args.RoutedEvent.TryGetClassHandler(sender.Type, out var handler))
                {
                    eventRoute.Add(handler);
                }

                sender = sender._parent;
            }

            eventRoute.Route(sender, args);
            EventRoute.Return(eventRoute);
        }

        private static void OnDragEnterThunk(object sender, DragEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnDragEnter(e);
            }
        }

        private static void OnDragLeaveThunk(object sender, DragEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnDragLeave(e);
            }
        }

        private static void OnDragOverThunk(object sender, DragEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnDragOver(e);
            }
        }

        private static void OnDragStartThunk(object sender, DragEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnDragStart(e);
            }
        }

        private static void OnDragStopThunk(object sender, DragEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnDragStop(e);
            }
        }

        private static void OnDropThunk(object sender, DragEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnDrop(e);
            }
        }

        private static void OnKeyDownThunk(object sender, KeyEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnKeyDown(e);
            }
        }

        private static void OnKeyUpThunk(object sender, KeyEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnKeyUp(e);
            }
        }

        private static void OnMouseDownThunk(object sender, MouseButtonEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnMouseDown(e);
            }
        }

        private static void OnMouseEnterThunk(object sender, RoutedEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnMouseEnter(e);
            }
        }

        private static void OnMouseLeaveThunk(object sender, RoutedEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnMouseLeave(e);
            }
        }

        private static void OnMouseUpThunk(object sender, MouseButtonEventArgs e)
        {
            if (sender is Control control)
            {
                control.OnMouseUp(e);
            }
        }

        #endregion


        private readonly Dictionary<RoutedEvent, ControlRoutedEventHandlerCollection> _handlerCollections = new Dictionary<RoutedEvent, ControlRoutedEventHandlerCollection>();
    }
}
