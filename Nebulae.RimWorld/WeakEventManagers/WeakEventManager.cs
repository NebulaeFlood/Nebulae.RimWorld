﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Nebulae.RimWorld.WeakEventManagers
{
    /// <summary>
    /// 通过订阅者直接引发操作的弱事件管理器
    /// </summary>
    /// <typeparam name="T">管理的对象类型</typeparam>
    /// <remarks>实现此类后，根据自定义的统一方法实现相应的调用方法。</remarks>
    public abstract class WeakEventManager<T> : IWeakCollection where T : class
    {
        /// <summary>
        /// 事件的订阅者
        /// </summary>
        private readonly List<WeakReference<T>> _subscribers;


        /// <summary>
        /// 管理对象的存活个数
        /// </summary>
        /// <remarks>每次访问都会遍历一次存活对象，不建议频繁访问。</remarks>
        public int Count
        {
            get
            {
                Purge();

                return _subscribers.Count;
            }
        }


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 为 <see cref="WeakEventManager{T}"/> 派生类实现基本初始化
        /// </summary>
        public WeakEventManager()
        {
            _subscribers = new List<WeakReference<T>>(1);
        }

        /// <summary>
        /// 为 <see cref="WeakEventManager{T}"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="capcity">期望管理的对象数量</param>
        public WeakEventManager(int capcity)
        {
            _subscribers = new List<WeakReference<T>>(capcity);
        }

        /// <summary>
        /// 为 <see cref="WeakEventManager{T}"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="subscribers">需要管理的对象</param>
        public WeakEventManager(IEnumerable<T> subscribers)
        {
            _subscribers = new List<WeakReference<T>>(
                from subscriber in subscribers.Distinct()
                where subscriber != null
                select new WeakReference<T>(subscriber));
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 获取存活的可管理对象
        /// </summary>
        /// <returns>存活的可管理对象。</returns>
        /// <remarks>返回的序列为订阅序列的倒序。</remarks>
        public List<T> GetSubcribers()
        {
            bool anyDead = false;

            List<T> subscribers = new List<T>(_subscribers.Count);

            for (int i = _subscribers.Count - 1; i >= 0; i--)
            {
                if (_subscribers[i].TryGetTarget(out T target))
                {
                    subscribers.Add(target);
                }
                else
                {
                    _subscribers.RemoveAt(i);

                    anyDead = true;
                }
            }

            if (anyDead)
            {
                _subscribers.TrimExcess();
            }

            return subscribers;
        }

        /// <summary>
        /// 管理指定对象
        /// </summary>
        /// <param name="subscriber">要添加的对象</param>
        public void Manage(T subscriber)
        {
            if (subscriber is null)
            {
                return;
            }

            for (int i = _subscribers.Count - 1; i >= 0; i--)
            {
                if (!_subscribers[i].TryGetTarget(out T target))
                {
                    _subscribers.RemoveAt(i);
                }
                else if (ReferenceEquals(target, subscriber))
                {
                    return;
                }
            }

            _subscribers.Add(new WeakReference<T>(subscriber));
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _subscribers.Clear();
        }

        /// <inheritdoc/>
        public void Purge()
        {
            if (_subscribers.RemoveAll(x => !x.TryGetTarget(out _)) > 0)
            {
                _subscribers.TrimExcess();
            }
        }

        /// <summary>
        /// 取消管理指定对象
        /// </summary>
        /// <param name="subscriber">取消管理的对象</param>
        public void Remove(T subscriber)
        {
            if (subscriber is null)
            {
                return;
            }

            for (int i = _subscribers.Count - 1; i >= 0; i--)
            {
                if (!_subscribers[i].TryGetTarget(out T target))
                {
                    _subscribers.RemoveAt(i);
                }
                else if (ReferenceEquals(target, subscriber))
                {
                    _subscribers.RemoveAt(i);
                    return;
                }
            }
        }

        #endregion
    }


#if DEBUG

    /// <summary>
    /// 表示一个订阅了由 <see cref="WeakEventManager{TSender, TArgs}"/> 管理的事件的对象
    /// </summary>
    /// <typeparam name="TSender">事件源类型</typeparam>
    /// <typeparam name="TArgs">事件参数类型</typeparam>
    public interface IWeakEventSubscriber<TSender, TArgs>
    {
        /// <summary>
        /// 事件被触发时执行的操作
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="args">事件数据</param>
        void OnEventActivated(TSender sender, TArgs args);
    }

    /// <summary>
    /// <see cref="WeakEventManager{T}"/> 的标准实现
    /// </summary>
    /// <typeparam name="TSender">事件源类型</typeparam>
    /// <typeparam name="TArgs">事件参数类型</typeparam>
    public class WeakEventManager<TSender, TArgs> : WeakEventManager<IWeakEventSubscriber<TSender, TArgs>>
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="WeakEventManager{TSender, TArgs}"/> 的新实例
        /// </summary>
        public WeakEventManager() : base()
        {
        }

        /// <summary>
        /// 初始化 <see cref="WeakEventManager{TSender, TArgs}"/> 的新实例
        /// </summary>
        /// <param name="capcity">期望管理的对象数量</param>
        public WeakEventManager(int capcity) : base(capcity)
        {
        }

        /// <summary>
        /// 初始化 <see cref="WeakEventManager{TSender, TArgs}"/> 的新实例
        /// </summary>
        /// <param name="subscribers">需要管理的对象</param>
        public WeakEventManager(IEnumerable<IWeakEventSubscriber<TSender, TArgs>> subscribers) : base(subscribers)
        {
        }

        #endregion


        /// <summary>
        /// 激活所有订阅者的事件处理方法
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="args">事件数据</param>
        public void Invoke(TSender sender, TArgs args)
        {
            foreach (var subcriber in GetSubcribers())
            {
                subcriber.OnEventActivated(sender, args);
            }
        }
    }

#endif
}
