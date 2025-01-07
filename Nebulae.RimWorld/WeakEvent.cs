// using System.ComponentModel;

// Copyright (c) 2019-2021 dotnet campus

// "Anti 996" License Version 1.0 (Draft)

// Permission is hereby granted to any individual or legal entity
// obtaining a copy of this licensed work (including the source code,
// documentation and/or related items, hereinafter collectively referred
// to as the "licensed work"), free of charge, to deal with the licensed
// work for any purpose, including without limitation, the rights to use,
// reproduce, modify, prepare derivative works of, distribute, publish
// and sublicense the licensed work, subject to the following conditions:

// 1.The individual or the legal entity must conspicuously display,
// without modification, this License and the notice on each redistributed
// or derivative copy of the Licensed Work.

// 2. The individual or the legal entity must strictly comply with all
// applicable laws, regulations, rules and standards of the jurisdiction
// relating to labor and employment where the individual is physically
// located or where the individual was born or naturalized; or where the
// legal entity is registered or is operating (whichever is stricter). In
// case that the jurisdiction has no such laws, regulations, rules and
// standards or its laws, regulations, rules and standards are
// unenforceable, the individual or the legal entity are required to
// comply with Core International Labor Standards.

// 3. The individual or the legal entity shall not induce, suggest or force
// its employee(s), whether full-time or part-time, or its independent
// contractor(s), in any methods, to agree in oral or written form, to
// directly or indirectly restrict, weaken or relinquish his or her
// rights or remedies under such laws, regulations, rules and standards
// relating to labor and employment as mentioned above, no matter whether
// such written or oral agreements are enforceable under the laws of the
// said jurisdiction, nor shall such individual or the legal entity
// limit, in any methods, the rights of its employee(s) or independent
// contractor(s) from reporting or complaining to the copyright holder or
// relevant authorities monitoring the compliance of the license about
// its violation(s) of the said license.

// THE LICENSED WORK IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN ANY WAY CONNECTION WITH THE
// LICENSED WORK OR THE USE OR OTHER DEALINGS IN THE LICENSED WORK.

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
            _subcribers = new ConditionalWeakSet<object>(1);
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
                List<object> aliveSbucribers = _subcribers.WhereAlive();
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
            /// 调用事件处理器
            /// </summary>
            /// <param name="sender">事件的发送者</param>
            /// <param name="args">事件数据</param>
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
            /// 移除事件处理器
            /// </summary>
            /// <param name="eventHandler">要移除的事件处理器</param>
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
                    return obj is Entry entry
                        && ReferenceEquals(Info, entry.Info);
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
