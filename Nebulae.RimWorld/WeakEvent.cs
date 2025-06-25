using System;
using System.Collections.Generic;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 表示一个弱事件
    /// </summary>
    /// <typeparam name="TSender">处理器的 sender 参数类型</typeparam>
    /// <typeparam name="TArgs">事件数据的类型</typeparam>
    public sealed class WeakEvent<TSender, TArgs> where TArgs : EventArgs
    {
        /// <summary>
        /// 获取当前事件处理程序的数量
        /// </summary>
        public int Count => _handlers.Count;


        /// <summary>
        /// 初始化 <see cref="WeakEvent{TSender, TArgs}"/> 的新实例
        /// </summary>
        public WeakEvent() { }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 添加事件处理程序
        /// </summary>
        /// <param name="handler">要添加的处理程序</param>
        public void AddHandler(Action<TSender, TArgs> handler)
        {
            if (handler is null)
            {
                return;
            }

            _handlers.AddLast(WeakEventHandlerFactory.Convert(handler));
        }

        /// <summary>
        /// 添加事件处理程序
        /// </summary>
        /// <param name="handler">要添加的处理程序</param>
        public void AddHandler(IWeakEventHandler<TSender, TArgs> handler)
        {
            if (handler is null)
            {
                return;
            }

            _handlers.AddLast(handler);
        }

        /// <summary>
        /// 移除所有事件处理程序
        /// </summary>
        public void Clear() => _handlers.Clear();

        /// <summary>
        /// 调用所有事件处理程序
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="args">包含事件数据的对象</param>
        public void Invoke(TSender sender, TArgs args)
        {
            if (_handlers.Count < 1)
            {
                return;
            }

            var node = _handlers.First;

            while (node != null)
            {
                node.Value.Invoke(sender, args);
            }
        }

        /// <summary>
        /// 清除所有已过期的事件处理程序
        /// </summary>
        public void Purge()
        {
            if (_handlers.Count < 1)
            {
                return;
            }

            var node = _handlers.Last;

            while (node != null)
            {
                if (!node.Value.IsAlive)
                {
                    _handlers.Remove(node);
                }

                node = node.Previous;
            }

            return;
        }

        /// <summary>
        /// 移除事件处理程序
        /// </summary>
        /// <param name="handler">要移除的处理程序</param>
        /// <returns>若成功移除，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool RemoveHandler(Delegate handler)
        {
            if (_handlers.Count < 1 || handler is null)
            {
                return false;
            }

            var node = _handlers.Last;

            while (node != null)
            {
                if (node.Value.Equals(handler))
                {
                    _handlers.Remove(node);
                    return true;
                }

                node = node.Previous;
            }

            return false;
        }

        /// <summary>
        /// 移除事件处理程序
        /// </summary>
        /// <param name="handler">要移除的处理程序</param>
        /// <returns>若成功移除，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool RemoveHandler(IWeakEventHandler<TSender, TArgs> handler)
        {
            if (_handlers.Count < 1 || handler is null)
            {
                return false;
            }

            var node = _handlers.Last;

            while (node != null)
            {
                if (node.Value.Equals(handler))
                {
                    _handlers.Remove(node);
                    return true;
                }

                node = node.Previous;
            }

            return false;
        }

        #endregion


        private readonly LinkedList<IWeakEventHandler<TSender, TArgs>> _handlers = new LinkedList<IWeakEventHandler<TSender, TArgs>>();
    }
}
