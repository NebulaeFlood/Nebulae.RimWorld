using Nebulae.RimWorld.Collections;
using System;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 弱事件
    /// </summary>
    /// <typeparam name="TSender">处理器的 sender 参数类型</typeparam>
    /// <typeparam name="TArgs">事件数据的类型</typeparam>
    /// <remarks>保存订阅者的弱引用，使订阅者在订阅此事件时可被释放。</remarks>
    public sealed class WeakEvent<TSender, TArgs> : RoughLinkedListBase<IWeakEventHandler<TSender, TArgs>> where TArgs : EventArgs
    {
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
        /// 添加事件处理器
        /// </summary>
        /// <param name="handler">要添加的处理器</param>
        public void AddHandler(Delegate handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            InsertHandler(WeakEventHandlerFactory.Convert<TSender, TArgs>(handler));
        }

        /// <summary>
        /// 添加事件处理器
        /// </summary>
        /// <param name="handler">要添加的处理器</param>
        public void AddHandler(Action<TSender, TArgs> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            InsertHandler(WeakEventHandlerFactory.Convert(handler));
        }

        /// <summary>
        /// 添加事件处理器
        /// </summary>
        /// <param name="handler">要添加的处理器</param>
        public void AddHandler(IWeakEventHandler<TSender, TArgs> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            InsertHandler(handler);
        }

        /// <summary>
        /// 添加事件处理器
        /// </summary>
        /// <param name="handler">要添加的处理器</param>
        /// <remarks>该方法在转换事件处理器时不检查处理器参数。</remarks>
        public void AddHandlerUnsafe(Delegate handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            InsertHandler(WeakEventHandlerFactory.ConvertUnsafe<TSender, TArgs>(handler));
        }

        /// <summary>
        /// 移除所有事件处理器
        /// </summary>
        public void Clear()
        {
            head = null;
            tail = null;

            count = 0;
        }

        /// <summary>
        /// 调用所有事件处理器
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="args">包含事件数据的对象</param>
        public void Invoke(TSender sender, TArgs args)
        {
            var node = head;

            while (node != null)
            {
                node.Item.Invoke(sender, args);
                node = node.Next;
            }
        }

        /// <summary>
        /// 清除所有已过期的事件处理器
        /// </summary>
        public void Purge()
        {
            var node = tail;

            while (node != null)
            {
                if (!node.Item.IsAlive)
                {
                    count--;
                    PickUp(node);
                }

                node = node.Prev;
            }
        }

        /// <summary>
        /// 移除事件处理器
        /// </summary>
        /// <param name="handler">要移除的处理器</param>
        /// <returns>若成功移除，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool RemoveHandler(Delegate handler)
        {
            if (handler is null)
            {
                return false;
            }

            var node = tail;

            while (node != null)
            {
                if (node.Item.Equals(handler))
                {
                    count--;
                    PickUp(node);

                    return true;
                }

                node = node.Prev;
            }

            return false;
        }

        /// <summary>
        /// 移除事件处理器
        /// </summary>
        /// <param name="handler">要移除的处理器</param>
        /// <returns>若成功移除，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool RemoveHandler(IWeakEventHandler<TSender, TArgs> handler)
        {
            if (handler is null)
            {
                return false;
            }

            var node = tail;

            while (node != null)
            {
                if (node.Item.Equals(handler))
                {
                    count--;
                    PickUp(node);

                    return true;
                }

                node = node.Prev;
            }

            return false;
        }

        #endregion


        private void InsertHandler(IWeakEventHandler<TSender, TArgs> handler)
        {
            count++;
            InsertLast(handler);
        }


        //------------------------------------------------------
        //
        //  Operators
        //
        //------------------------------------------------------

        #region Operators
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public static WeakEvent<TSender, TArgs> operator +(WeakEvent<TSender, TArgs> left, Delegate right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            left.InsertHandler(WeakEventHandlerFactory.Convert<TSender, TArgs>(right));
            return left;
        }

        public static WeakEvent<TSender, TArgs> operator +(WeakEvent<TSender, TArgs> left, Action<TSender, TArgs> right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            left.InsertHandler(WeakEventHandlerFactory.Convert(right));
            return left;
        }

        public static WeakEvent<TSender, TArgs> operator +(WeakEvent<TSender, TArgs> left, IWeakEventHandler<TSender, TArgs> right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            left.InsertHandler(right);
            return left;
        }

        public static WeakEvent<TSender, TArgs> operator -(WeakEvent<TSender, TArgs> left, Delegate right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            left.RemoveHandler(right);
            return left;
        }

        public static WeakEvent<TSender, TArgs> operator -(WeakEvent<TSender, TArgs> left, Action<TSender, TArgs> right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            left.RemoveHandler(right);
            return left;
        }

        public static WeakEvent<TSender, TArgs> operator -(WeakEvent<TSender, TArgs> left, IWeakEventHandler<TSender, TArgs> right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            left.RemoveHandler(right);
            return left;
        }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        #endregion
    }
}
