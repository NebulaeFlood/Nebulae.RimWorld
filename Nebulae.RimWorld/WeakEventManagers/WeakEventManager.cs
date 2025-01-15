using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.WeakEventManagers
{
    /// <summary>
    /// 一个通过订阅者直接引发操作的弱事件管理器
    /// </summary>
    /// <typeparam name="T">管理的对象类型</typeparam>
    /// <remarks>实现此类后，根据自定义的统一方法实现相应的调用方法。</remarks>
    public abstract class WeakEventManager<T> where T : class
    {
        /// <summary>
        /// 事件的订阅者
        /// </summary>
        internal protected readonly List<WeakReference<T>> Subscribers;


        /// <summary>
        /// 管理对象的存活个数
        /// </summary>
        /// <remarks>每次访问都会遍历一次存活对象，不建议频繁访问。</remarks>
        public int Count
        {
            get
            {
                Purge();

                return Subscribers.Count;
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
            Subscribers = new List<WeakReference<T>>(1);
        }

        /// <summary>
        /// 为 <see cref="WeakEventManager{T}"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="capcity">期望管理的对象数量</param>
        public WeakEventManager(int capcity)
        {
            Subscribers = new List<WeakReference<T>>(capcity);
        }

        /// <summary>
        /// 为 <see cref="WeakEventManager{T}"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="subscribers">需要管理的对象</param>
        public WeakEventManager(IEnumerable<T> subscribers)
        {
            Subscribers = new List<WeakReference<T>>(
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
        /// 管理指定对象
        /// </summary>
        /// <param name="subscriber">要添加的对象</param>
        public void Manager(T subscriber)
        {
            if (subscriber is null)
            {
                return;
            }

            for (int i = Subscribers.Count - 1; i >= 0; i--)
            {
                if (!Subscribers[i].TryGetTarget(out T target))
                {
                    Subscribers.RemoveAt(i);
                }
                else if (ReferenceEquals(target, subscriber))
                {
                    return;
                }
            }

            Subscribers.Add(new WeakReference<T>(subscriber));
        }

        /// <summary>
        /// 取消所有管理
        /// </summary>
        public void Clear()
        {
            Subscribers.Clear();
        }

        /// <summary>
        /// 清理管理的已经被回收的对象
        /// </summary>
        public void Purge()
        {
            if (Subscribers.RemoveAll(x => !x.TryGetTarget(out _)) > 0)
            {
                Subscribers.TrimExcess();
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

            for (int i = Subscribers.Count - 1; i >= 0; i--)
            {
                if (!Subscribers[i].TryGetTarget(out T target))
                {
                    Subscribers.RemoveAt(i);
                }
                else if (ReferenceEquals(target, subscriber))
                {
                    Subscribers.RemoveAt(i);
                    return;
                }
            }
        }

        #endregion
    }

#if DEBUG

    /// <summary>
    /// <see cref="WeakEventManager{T}"/> 的标准实现
    /// </summary>
    /// <typeparam name="TSender">事件源类型</typeparam>
    /// <typeparam name="TArgs">事件参数类型</typeparam>
    public class WeakEventManager<TSender, TArgs> : WeakEventManager<IWeakEventSubscriber<TSender, TArgs>>
    {
        private readonly WeakReference _obsoletedSubcriber = new WeakReference(new object());


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
            if (!_obsoletedSubcriber.IsAlive)
            {
                Purge();

                _obsoletedSubcriber.Target = new object();
            }

            Subscribers.ForEach(x =>
            {
                if (x.TryGetTarget(out var target))
                {
                    target.OnEventActivated(sender, args);
                }
            });
        }
    }

#endif
}
