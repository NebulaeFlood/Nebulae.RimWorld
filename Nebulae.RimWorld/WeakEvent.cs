using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 定义一个弱事件处理器
    /// </summary>
    /// <typeparam name="TSender">发起事件的对象类型</typeparam>
    /// <typeparam name="TArgs">事件数据的类型</typeparam>
    /// <param name="sender">发起事件的对象</param>
    /// <param name="e">事件数据</param>
    public delegate void WeakEventHandler<TSender, TArgs>(TSender sender, TArgs e);

    /// <summary>
    /// 定义一个弱事件
    /// </summary>
    /// <typeparam name="TSender">发起事件的对象类型</typeparam>
    /// <typeparam name="TArgs">事件数据的类型</typeparam>
    public class WeakEvent<TSender, TArgs> where TSender : class
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _lock;

        /// <summary>
        /// 处理事件的所有方法
        /// </summary>
        private readonly ConditionalWeakTable<object, HandlerCollection> _handlers;

        /// <summary>
        /// 订阅事件的对象
        /// </summary>
        private readonly ConditionalWeakSet<object> _subcribers;


        /// <summary>
        /// 初始化 <see cref="WeakEventHandler{TSender, TArgs}"/> 的新实例
        /// </summary>
        public WeakEvent()
        {
            _lock = new object();
            _handlers = new ConditionalWeakTable<object, HandlerCollection>();
            _subcribers = new ConditionalWeakSet<object>();
        }


        /// <summary>
        /// 添加事件处理器
        /// </summary>
        /// <param name="eventHandler">要添加的事件处理器</param>
        /// <exception cref="InvalidOperationException">当订阅事件的对象为 null 或已被回收时发生。</exception>
        public void Add(WeakEventHandler<TSender, TArgs> eventHandler)
        {
            MethodInfo method = eventHandler.Method;

            object subcriber = (eventHandler.Target ?? method.DeclaringType)
                ?? throw new InvalidOperationException($"Subcriber of {this} can not be null.");

            lock (_lock)
            {
                if (!_subcribers.Contains(subcriber))
                {
                    _subcribers.Add(subcriber);
                    _handlers.Add(subcriber, new HandlerCollection(eventHandler));
                }
                else if (_handlers.TryGetValue(subcriber, out var handlers))
                {
                    handlers.Add(eventHandler);
                }
                else
                {
                    throw new InvalidOperationException($"Subcriber of {this} can not have been garbage collected or its Equals and GetHashCode methods are incorrect.");
                }
            }
        }

        /// <summary>
        /// 激活事件
        /// </summary>
        /// <param name="sender">事件的发送者</param>
        /// <param name="args">事件数据</param>
        public void Invoke(TSender sender, TArgs args)
        {
            lock (_lock)
            {
                List<object> aliveSbucribers = _subcribers.GetWhereAlive();
                if (aliveSbucribers.Count > 0)
                {
                    for (int i = 0; i < aliveSbucribers.Count; i++)
                    {
                        _handlers.TryGetValue(aliveSbucribers[i], out HandlerCollection handlers);
                        handlers.Invoke(sender, args);
                    }
                }
                else
                {
                    _subcribers.Clear();
                }
            }
        }

        /// <summary>
        /// 移除事件处理器
        /// </summary>
        /// <param name="eventHandler">要移除的事件处理器</param>
        public void Remove(WeakEventHandler<TSender, TArgs> eventHandler)
        {
            MethodInfo method = eventHandler.Method;

            object subcriber = eventHandler.Target ?? method.DeclaringType;
            if (subcriber is null) { return; }

            lock (_lock)
            {
                if (_handlers.TryGetValue(subcriber, out var handlers))
                {
                    handlers.Remove(eventHandler);
                }
            }
        }


        /// <summary>
        /// 事件处理器的集合
        /// </summary>
        private class HandlerCollection
        {
            /// <summary>
            /// 事件处理器的调用序列
            /// </summary>
            private readonly LinkedList<Entry> _invocationList;


            public HandlerCollection(WeakEventHandler<TSender, TArgs> eventHandler)
            {
                _invocationList = new LinkedList<Entry>();
                _invocationList.AddFirst(new Entry(eventHandler));
            }


            /// <summary>
            /// 向集合添加处理器
            /// </summary>
            /// <param name="eventHandler">要添加的处理器</param>
            public void Add(WeakEventHandler<TSender, TArgs> eventHandler)
            {
                _invocationList.AddFirst(new Entry(eventHandler));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            public void Invoke(TSender sender, TArgs args)
            {
                LinkedListNode<Entry> node = _invocationList.Last;
                while (node != null)
                {
                    node.Value.Handler(sender, args);
                    node = node.Previous;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="eventHandler"></param>
            public void Remove(WeakEventHandler<TSender, TArgs> eventHandler)
            {
                _invocationList.Remove(new Entry(eventHandler));
            }


            /// <summary>
            /// 集合存储元素的条目
            /// </summary>
            private struct Entry
            {
                /// <summary>
                /// 事件处理器
                /// </summary>
                public WeakEventHandler<TSender, TArgs> Handler;

                /// <summary>
                /// 处理方法的信息
                /// </summary>
                public MethodInfo Info;


                /// <summary>
                /// 初始化 <see cref="Entry"/> 的新实例
                /// </summary>
                public Entry(WeakEventHandler<TSender, TArgs> eventHandler)
                {
                    Info = eventHandler.Method;
                    Handler = eventHandler;
                }


                /// <summary>
                /// 确定指定对象是否等于当前对象
                /// </summary>
                /// <param name="obj">要与当前对象进行比较的对象</param>
                /// <returns>如果指定的对象等于当前对象，则为 <see langword="true"/>；反之则为 <see langword="false"/>。</returns>
                public override bool Equals(object obj)
                {
                    if (obj is Entry entry)
                    {
                        return ReferenceEquals(Info, entry.Info);
                    }
                    return base.Equals(obj);
                }

                /// <summary>
                /// 哈希函数
                /// </summary>
                /// <returns>当前对象的哈希代码</returns>
                public override int GetHashCode() => Info.GetHashCode();
            }
        }
    }
}
