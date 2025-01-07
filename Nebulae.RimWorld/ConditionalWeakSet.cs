using System;
using System.Collections.Generic;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 表示一个弱引用的集合，当集合中的对象不再被引用时，会被自动回收。
    /// </summary>
    /// <typeparam name="T">集合中的元素类型</typeparam>
    public class ConditionalWeakSet<T> where T : class
    {
        private readonly HashSet<Entry> _entries;


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="ConditionalWeakSet{T}"/> 的新实例
        /// </summary>
        public ConditionalWeakSet()
        {
            _entries = new HashSet<Entry>();
        }

        /// <summary>
        /// 初始化 <see cref="ConditionalWeakSet{T}"/> 的新实例
        /// </summary>
        /// <param name="capcity">集合的初始容量</param>
        public ConditionalWeakSet(int capcity)
        {
            _entries = new HashSet<Entry>(capcity);
        }

        /// <summary>
        /// 初始化 <see cref="ConditionalWeakSet{T}"/> 的新实例
        /// </summary>
        /// <param name="collection">要向集合添加的对象</param>
        public ConditionalWeakSet(IEnumerable<T> collection)
        {
            _entries = new HashSet<Entry>();
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 向集合添加指定的元素
        /// </summary>
        /// <param name="item">要添加的元素</param>
        /// <returns>如果成功将元素添加到集合中，则为 <see langword="true"/>；反之则为 <see langword="false"/>。</returns>
        public bool Add(T item)
        {
            if (item is null)
            {
                return false;
            }
            ClearDeadEntry();
            return _entries.Add(new Entry(item));
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear() => _entries.Clear();

        /// <summary>
        /// 判断集合是否包含指定的元素
        /// </summary>
        /// <param name="item">要在集合中查找的元素</param>
        /// <returns>如果集合包含指定的元素，则为 <see langword="true"/>；反之则为 <see langword="false"/>。</returns>
        public bool Contains(T item)
        {
            if (item is null)
            {
                return false;
            }
            ClearDeadEntry();
            return _entries.Contains(new Entry(item));
        }

        /// <summary>
        /// 从集合中移除指定元素
        /// </summary>
        /// <param name="item">要移除的元素</param>
        /// <returns>如果成功找到并移除该元素，则为 <see langword="true"/>；反之则为 <see langword="false"/>。</returns>
        public bool Remove(T item)
        {
            if (item is null)
            {
                return false;
            }
            ClearDeadEntry();
            return _entries.Remove(new Entry(item));
        }

        /// <summary>
        /// 将容量设置为包含的实际元素数
        /// </summary>
        public void TrimExcess()
        {
            ClearDeadEntry();
            _entries.TrimExcess();
        }

        /// <summary>
        /// 尝试在集合中搜索指定元素
        /// </summary>
        /// <param name="equalValue">要搜索的元素</param>
        /// <param name="actualValue">在集合中找到的元素；如果集合内不存在 <paramref name="equalValue"/>，则返回 <typeparamref name="T"/> 的默认值。</param>
        /// <returns>如果搜索到指定元素，则为 <see langword="true"/>；反之则为 <see langword="false"/>。</returns>
        public bool TryGetValue(T equalValue, out T actualValue)
        {
            if (equalValue is null)
            {
                actualValue = default;
                return false;
            }
            ClearDeadEntry();

            if (_entries.TryGetValue(new Entry(equalValue), out Entry entry))
            {
                entry.Reference.TryGetTarget(out actualValue);
                return true;
            }
            else
            {
                actualValue = default;
                return false;
            }
        }

        /// <summary>
        /// 获取集合中满足条件的元素
        /// </summary>
        /// <param name="match">条件表达式</param>
        /// <returns>集合中满足条件的元素</returns>
        public List<T> Where(Predicate<T> match)
        {
            List<T> results = new List<T>();
            List<Entry> deadEntries = new List<Entry>();

            foreach (var entry in _entries)
            {
                if (entry.Reference.TryGetTarget(out var value))
                {
                    if (match(value))
                    {
                        results.Add(value);
                    }
                }
                else
                {
                    deadEntries.Add(entry);
                }
            }

            for (int i = 0; i < deadEntries.Count; i++)
            {
                _entries.Remove(deadEntries[i]);
            }
            return results;
        }

        /// <summary>
        /// 获取集合中未被回收的元素
        /// </summary>
        /// <returns>集合中未被回收的元素</returns>
        public List<T> WhereAlive()
        {
            List<T> results = new List<T>();
            List<Entry> deadEntries = new List<Entry>();

            foreach (var entry in _entries)
            {
                if (entry.Reference.TryGetTarget(out var value))
                {
                    results.Add(value);
                }
                else
                {
                    deadEntries.Add(entry);
                }
            }

            for (int i = 0; i < deadEntries.Count; i++)
            {
                _entries.Remove(deadEntries[i]);
            }
            return results;
        }
        #endregion


        /// <summary>
        /// 移除关联对象已被回收的条目
        /// </summary>
        private void ClearDeadEntry() => _entries.RemoveWhere(x => !x.Reference.TryGetTarget(out _));


        /// <summary>
        /// 集合的条目
        /// </summary>
        private readonly struct Entry : IEquatable<Entry>
        {
            /// <summary>
            /// 条目关联对象的弱引用
            /// </summary> 
            internal readonly WeakReference<T> Reference;


            /// <summary>
            /// 初始化 <see cref="Entry"/> 的新实例
            /// </summary>
            /// <param name="target"></param>
            internal Entry(T target)
            {
                Reference = new WeakReference<T>(target);
            }


            public bool Equals(Entry other)
            {
                if (Reference.TryGetTarget(out T targetA) && Reference.TryGetTarget(out T targetB))
                {
                    return ReferenceEquals(targetA, targetB);
                }
                return !Reference.TryGetTarget(out T _) && !other.Reference.TryGetTarget(out T _);
            }

            public override bool Equals(object obj)
            {
                if (obj is Entry other)
                {
                    if (Reference.TryGetTarget(out T targetA) && Reference.TryGetTarget(out T targetB))
                    {
                        return ReferenceEquals(targetA, targetB);
                    }
                    return !Reference.TryGetTarget(out T _) && !other.Reference.TryGetTarget(out T _);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Reference.TryGetTarget(out T target) ? target.GetHashCode() : 0;
            }
        }
    }
}
