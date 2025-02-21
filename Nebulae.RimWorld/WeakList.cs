using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 弱引用集合
    /// </summary>
    /// <typeparam name="T">存储的对象类型</typeparam>
    /// <remarks>无法存放为 <see langword="null"/> 的对象。</remarks>
    public class WeakList<T> : IEnumerable<T>, IWeakCollection where T : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly List<WeakReference<T>> _items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly WeakReference _obsoletedItem = new WeakReference(new object());


        /// <summary>
        /// 集合中实际的对象个数
        /// </summary>
        /// <remarks>每次访问都会遍历一次集合，不建议频繁访问。</remarks>
        public int Count
        {
            get
            {
                Purge();

                return _items.Count;
            }
        }


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="WeakList{T}"/> 的新实例
        /// </summary>
        public WeakList()
        {
            _items = new List<WeakReference<T>>();
        }

        /// <summary>
        /// 初始化 <see cref="WeakList{T}"/> 的新实例
        /// </summary>
        /// <param name="capcity">集合的初始容量</param>
        public WeakList(int capcity)
        {
            _items = new List<WeakReference<T>>(capcity);
        }

        /// <summary>
        /// 初始化 <see cref="WeakList{T}"/> 的新实例
        /// </summary>
        /// <param name="items">集合的初始元素</param>
        public WeakList(IEnumerable<T> items)
        {
            _items = new List<WeakReference<T>>(
                from item in items
                where item != null
                select new WeakReference<T>(item));
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将指定对象添加到集合
        /// </summary>
        /// <param name="item">要添加的对象</param>
        public void Add(T item)
        {
            PrivatePurge();

            if (item is null)
            {
                return;
            }

            _items.Add(new WeakReference<T>(item));
        }

        /// <summary>
        /// 移除集合中的所有对象
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// 判断集合内是否含有指定对象
        /// </summary>
        /// <param name="item">要判断的集合是否拥有的对象</param>
        /// <returns>若集合中存在对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(T item)
        {
            PrivatePurge();

            if (item is null)
            {
                return false;
            }

            return _items.FindIndex(x =>
                x.TryGetTarget(out T target)
                && ReferenceEquals(target, item)) >= 0;
        }

        /// <summary>
        /// 对集合中的每个对象执行指定操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public void ForEach(Action<T> action)
        {
            PrivatePurge();

            if (action is null)
            {
                return;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].TryGetTarget(out T target))
                {
                    action(target);
                }
            }
        }


        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举器</returns>
        /// <remarks>此方法设计用于方便对集合内容进行访问，使用时注意不要破坏当前引用关系，</remarks>
        public IEnumerator<T> GetEnumerator()
        {
            Purge();

            foreach (var item in _items)
            {
                if (item.TryGetTarget(out T target))
                {
                    yield return target;
                }
            }
        }

        /// <summary>
        /// 清理集合内已经被回收的对象
        /// </summary>
        public void Purge()
        {
            if (_items.RemoveAll(x => !x.TryGetTarget(out _)) > 0)
            {
                _items.TrimExcess();
            }
        }

        /// <summary>
        /// 移除集合内的指定对象
        /// </summary>
        /// <param name="item">要移除的对象</param>
        /// <returns>若从集合中移除了对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(T item)
        {
            PrivatePurge();

            if (item is null)
            {
                return false;
            }

            int index = _items.FindIndex(x =>
                x.TryGetTarget(out T target)
                && ReferenceEquals(target, item));

            if (index < 0)
            {
                return false;
            }

            _items.RemoveAt(index);

            return true;
        }

        #endregion


        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举器</returns>
        /// <remarks>此方法设计用于方便对集合内容进行访问，使用时注意不要破坏当前引用关系，</remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private void PrivatePurge()
        {
            if (_obsoletedItem.IsAlive)
            {
                return;
            }

            Purge();

            _obsoletedItem.Target = new object();
        }
    }
}
