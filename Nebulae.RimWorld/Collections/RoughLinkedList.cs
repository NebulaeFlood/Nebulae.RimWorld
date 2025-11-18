using System;
using System.Collections.Generic;

namespace Nebulae.RimWorld.Collections
{
    /// <summary>
    /// 简易双向链表
    /// </summary>
    /// <typeparam name="T">链表元素类型</typeparam>
    /// <remarks>默认实现中不允许存储为 <see langword="null"/> 的元素。</remarks>
    public class RoughLinkedList<T> : RoughLinkedListBase<T>, ICollection<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// <typeparamref name="T"/> 类型实例的比较器
        /// </summary>
        public readonly IEqualityComparer<T> Comparer;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取该集合的第一个元素
        /// </summary>
        public T First => head is null ? default : head.Item;

        /// <summary>
        /// 获取该集合的第一个节点
        /// </summary>
        public RoughLinkedListNode<T> Head => head;

        /// <summary>
        /// 获取一个值，该值指示此集合是否为只读集合
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 获取该集合的最后一个元素
        /// </summary>
        public T Last => tail is null ? default : tail.Item;

        /// <summary>
        /// 获取该集合的最后一个节点
        /// </summary>
        public RoughLinkedListNode<T> Tail => tail;

        #endregion


        //------------------------------------------------------
        //
        //  Public Contstuctors
        //
        //------------------------------------------------------

        #region Public Contstuctors

        /// <summary>
        /// 初始化 <see cref="RoughLinkedList{T}"/> 的新实例
        /// </summary>
        public RoughLinkedList()
        {
            Comparer = EqualityComparer<T>.Default;
        }

        /// <summary>
        /// 初始化 <see cref="RoughLinkedList{T}"/> 的新实例
        /// </summary>
        /// <param name="comparer">元素的比较器</param>
        public RoughLinkedList(IEqualityComparer<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            Comparer = comparer;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将元素添加到集合的尾部
        /// </summary>
        /// <param name="item">要添加的元素</param>
        public virtual void Add(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            count++;
            InsertLast(new RoughLinkedListNode<T>(item));
        }

        /// <summary>
        /// 从集合中移除所有元素
        /// </summary>
        public virtual void Clear()
        {
            count = 0;

            head = null;
            tail = null;
        }

        /// <summary>
        /// 确定指定元素是否包含在集合中
        /// </summary>
        /// <param name="item">要查找的元素</param>
        /// <returns>若在集合中找到 <paramref name="item"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public virtual bool Contains(T item)
        {
            if (item is null)
            {
                return false;
            }

            var node = head;

            while (node is not null)
            {
                if (node.Item.Equals(item))
                {
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        /// <summary>
        /// 从目标数组的指定索引处开始，复制集合内的元素
        /// </summary>
        /// <param name="array">接收集合内元素的数组</param>
        /// <param name="arrayIndex">开始复制的索引</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            int capcity = array.Length;

            if (arrayIndex < 0 || arrayIndex >= capcity)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            if (arrayIndex + count > capcity)
            {
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array step and length.", nameof(array));
            }

            var node = head;

            while (node is not null)
            {
                array[arrayIndex++] = node.Item;
                node = node.Next;
            }
        }

        /// <summary>
        /// 对集合中的每个元素执行指定操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public void ForEach(Action<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var node = head;

            while (node is not null)
            {
                action(node.Item);
                node = node.Next;
            }
        }

        /// <summary>
        /// 以倒序对集合中的每个元素执行指定操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public void ForEachReverse(Action<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var node = tail;

            while (node is not null)
            {
                action(node.Item);
                node = node.Prev;
            }
        }

        /// <summary>
        /// 将指定元素从集合中移除
        /// </summary>
        /// <param name="item">要移除的元素</param>
        /// <returns>若移除了指定元素，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public virtual bool Remove(T item)
        {
            if (item is not null)
            {
                return false;
            }

            var node = head;

            while (node is not null)
            {
                if (node.Item.Equals(item))
                {
                    count--;
                    PickUp(node);
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        /// <summary>
        /// 将集合的元素复制到新数组中
        /// </summary>
        /// <returns>一个包含该集合中元素的数组。</returns>
        public T[] ToArray()
        {
            if (count < 1)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[count];
            int index = 0;

            var node = head;

            do
            {
                array[index++] = node.Item;
                node = node.Next;
            }
            while (node is not null);

            return array;
        }

        #endregion
    }
}
