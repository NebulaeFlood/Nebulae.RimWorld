using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nebulae.RimWorld.Collections
{
    /// <summary>
    /// 弱引用集合，其中的元素不会重复
    /// </summary>
    /// <typeparam name="T">存储的元素类型</typeparam>
    /// <remarks>不允许存储为 <see langword="null"/> 的元素。</remarks>
    public class WeakSet<T> : IEnumerable<T>, IWeakCollection where T : class
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取该集合中包含元素的数量
        /// </summary>
        public int Count
        {
            get
            {
                Purge();

                return _items.Count;
            }
        }

        // <summary>
        // 获取一个值，该值指示此集合是否为只读集合
        // </summary>
        // public bool IsReadOnly => false;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="WeakSet{T}"/> 的新实例
        /// </summary>
        public WeakSet()
        {
            _items = new HashSet<Entry>();
        }

        /// <summary>
        /// 初始化 <see cref="WeakSet{T}"/> 的新实例
        /// </summary>
        /// <param name="capcity">集合的初始容量</param>
        public WeakSet(int capcity)
        {
            _items = new HashSet<Entry>(capcity);
        }

        /// <summary>
        /// 初始化 <see cref="WeakList{T}"/> 的新实例
        /// </summary>
        /// <param name="items">集合的初始元素</param>
        public WeakSet(IEnumerable<T> items)
        {
            _items = new HashSet<Entry>(items
                .Where(x => x != null)
                .Select(x => new Entry(x)));
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将指定元素添加到集合
        /// </summary>
        /// <param name="item">要添加的元素</param>
        public bool Add(T item)
        {
            if (item is null)
            {
                return false;
            }

            return _items.Add(new Entry(item));
        }

        /// <summary>
        /// 移除集合中的所有元素
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// 判断集合内是否含有指定元素
        /// </summary>
        /// <param name="item">要判断的集合是否拥有的元素</param>
        /// <returns>若集合中存在元素，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(T item)
        {
            if (item is null)
            {
                return false;
            }

            return _items.TryGetValue(new Entry(item), out _);
        }


        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举器</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _items)
            {
                if (item.TryGetValue(out T target))
                {
                    yield return target;
                }
            }
        }

        /// <summary>
        /// 清理集合内已经被回收的元素
        /// </summary>
        public void Purge()
        {
            _items.RemoveWhere(x => !x.TryGetValue(out _));
        }

        /// <summary>
        /// 移除集合内的指定元素
        /// </summary>
        /// <param name="item">要移除的元素</param>
        /// <returns>若从集合中移除了元素，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(T item)
        {
            if (item is null)
            {
                return false;
            }

            return _items.Remove(new Entry(item));
        }

        #endregion


        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in _items)
            {
                if (item.TryGetValue(out T target))
                {
                    yield return target;
                }
            }
        }


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly HashSet<Entry> _items;


        /// <summary>
        /// 集合的条目
        /// </summary>
        private readonly struct Entry : IEquatable<Entry>
        {
            /// <summary>
            /// 初始化 <see cref="Entry"/> 的新实例
            /// </summary>
            /// <param name="target"></param>
            internal Entry(T target)
            {
                _reference = new WeakReference<T>(target);
                _hashCode = target.GetHashCode();
            }


            public bool Equals(Entry other)
            {
                if (_reference.TryGetTarget(out T targetA) && _reference.TryGetTarget(out T targetB))
                {
                    return ReferenceEquals(targetA, targetB);
                }

                return !_reference.TryGetTarget(out T _) && !other._reference.TryGetTarget(out T _);
            }

            public override bool Equals(object obj)
            {
                if (obj is Entry other)
                {
                    if (_reference.TryGetTarget(out T targetA) && _reference.TryGetTarget(out T targetB))
                    {
                        return ReferenceEquals(targetA, targetB);
                    }

                    return !_reference.TryGetTarget(out T _) && !other._reference.TryGetTarget(out T _);
                }

                return false;
            }

            public override int GetHashCode() => _hashCode;


            public bool TryGetValue(out T value)
            {
                return _reference.TryGetTarget(out value);
            }


            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            internal T Value => _reference.TryGetTarget(out T target) ? target : null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly int _hashCode;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly WeakReference<T> _reference;
        }
    }
}
