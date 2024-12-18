using System;
using System.Collections.Generic;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 弱引用集合
    /// </summary>
    /// <typeparam name="T">集合内元素的类型</typeparam>
    /// <remarks>
    /// 已被回收的对象不会出现在集合中。<para/>
    /// 该集合内元素具有唯一性。<para/>
    /// </remarks>
    public class WeakCollection<T> where T : class
    {
        private readonly HashSet<Entry> _entries;


        /// <summary>
        /// 初始化 <see cref="WeakCollection{T}"/> 的新实例
        /// </summary>
        public WeakCollection()
        {
            _entries = new HashSet<Entry>();
        }


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
        /// 获取集合中满足条件的元素
        /// </summary>
        /// <param name="match">条件表达式</param>
        /// <returns>集合中满足条件的元素</returns>
        public List<T> GetWhere(Predicate<T> match)
        {
            List<T> results = new List<T>();
            List<Entry> deadEntries = new List<Entry>();

            foreach (var entry in _entries)
            {
                if (entry.Value.TryGetTarget(out var value))
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
        public List<T> GetWhereAlive()
        {
            List<T> results = new List<T>();
            List<Entry> deadEntries = new List<Entry>();

            foreach (var entry in _entries)
            {
                if (entry.Value.TryGetTarget(out var value))
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

            if (_entries.TryGetValue(new Entry(equalValue), out Entry entry))
            {
                entry.Value.TryGetTarget(out actualValue);
                return true;
            }
            else
            {
                actualValue = default;
                return false;
            }
        }

        /// <summary>
        /// 移除关联对象已被回收的条目
        /// </summary>
        private void ClearDeadEntry() => _entries.RemoveWhere(x => !x.Value.TryGetTarget(out _));


        /// <summary>
        /// 集合存储元素的条目
        /// </summary>
        private struct Entry
        {
            /// <summary>
            /// 条目的值的哈希码
            /// </summary>
            private int _hashCode;

            /// <summary>
            /// 条目存储的元素
            /// </summary>
            public WeakReference<T> Value;


            /// <summary>
            /// 初始化 <see cref="Entry"/> 的新实例
            /// </summary>
            /// <param name="value">条目的值</param>
            public Entry(T value)
            {
                Value = new WeakReference<T>(value);
                _hashCode = value.GetHashCode();
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
                    if (Value.TryGetTarget(out T targetA) && entry.Value.TryGetTarget(out T targetB))
                    {
                        return ReferenceEquals(targetA, targetB);
                    }
                    return !Value.TryGetTarget(out _) && !entry.Value.TryGetTarget(out _);
                }
                return base.Equals(obj);
            }

            /// <summary>
            /// 哈希函数
            /// </summary>
            /// <returns>当前对象的哈希代码</returns>
            public override int GetHashCode()
            {
                if (Value.TryGetTarget(out T target))
                {
                    _hashCode = target.GetHashCode();
                }
                return _hashCode;
            }
        }
    }
}
