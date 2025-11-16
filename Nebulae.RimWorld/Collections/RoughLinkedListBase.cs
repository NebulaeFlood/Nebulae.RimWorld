using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nebulae.RimWorld.Collections
{
    /// <summary>
    /// 简易双向列表基类
    /// </summary>
    /// <typeparam name="T">链表元素类型</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(RoughLinkedListDebugView))]
    public abstract class RoughLinkedListBase<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        //------------------------------------------------------
        //
        //  Protected Fields
        //
        //------------------------------------------------------

        #region Protected Fields

        /// <summary>
        /// 链表元素数量
        /// </summary>
        protected int count;

        /// <summary>
        /// 链表的头节点
        /// </summary>
        protected RoughLinkedListNode<T> head;

        /// <summary>
        /// 链表的尾节点
        /// </summary>
        protected RoughLinkedListNode<T> tail;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取该集合中包含元素的数量
        /// </summary>
        public int Count => count;

        /// <summary>
        /// 获取一个值，该值指示此集合是否不包含任何元素
        /// </summary>
        public bool IsEmpty => head is null;

        #endregion


        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>可用于循环访问集合的枚举器。</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 将元素插入到指定节点之后
        /// </summary>
        /// <param name="index">作为索引的节点</param>
        /// <param name="item">要插入的元素</param>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void InsertAfter(RoughLinkedListNode<T> index, T item)
        {
            var node = new RoughLinkedListNode<T>(index, item, index.Next);

            if (index != tail)
            {
                index.Next.Prev = node;
            }
            else
            {
                tail = node;
            }

            index.Next = node;
        }

        /// <summary>
        /// 将节点插入到指定节点之后
        /// </summary>
        /// <param name="index">作为索引的节点</param>
        /// <param name="node">要插入的节点</param>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void InsertAfter(RoughLinkedListNode<T> index, RoughLinkedListNode<T> node)
        {
            PickUp(node);

            if (index != tail)
            {
                index.Next.Prev = node;
                node.Next = index.Next;
            }
            else
            {
                tail = node;
            }

            index.Next = node;
            node.Prev = index;
        }

        /// <summary>
        /// 将元素插入到指定节点之前
        /// </summary>
        /// <param name="index">作为索引的节点</param>
        /// <param name="item">要插入的元素</param>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void InsertBefore(RoughLinkedListNode<T> index, T item)
        {
            var node = new RoughLinkedListNode<T>(index.Prev, item, index);

            if (index != head)
            {
                index.Prev.Next = node;
            }
            else
            {
                head = node;
            }

            index.Prev = node;
        }

        /// <summary>
        /// 将节点插入到指定节点之前
        /// </summary>
        /// <param name="index">作为索引的节点</param>
        /// <param name="node">要插入的节点</param>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void InsertBefore(RoughLinkedListNode<T> index, RoughLinkedListNode<T> node)
        {
            PickUp(node);

            if (index != head)
            {
                index.Prev.Next = node;
                node.Prev = index.Prev;
            }
            else
            {
                head = node;
            }

            index.Prev = node;
            node.Next = index;
        }

        /// <summary>
        /// 将元素添加到链表的头部
        /// </summary>
        /// <param name="item">要添加的元素</param>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void InsertFirst(T item)
        {
            var node = new RoughLinkedListNode<T>(item, head);

            if (head is null)
            {
                head = node;
                tail = node;
            }
            else
            {
                head.Prev = node;
                head = node;
            }
        }

        /// <summary>
        /// 将节点添加到链表的头部
        /// </summary>
        /// <param name="node">要添加的节点</param>
        /// <returns>添加到链表头部的节点。</returns>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void InsertFirst(RoughLinkedListNode<T> node)
        {
            if (head is null)
            {
                head = node;
                tail = node;
            }
            else
            {
                node.Next = head;

                head.Prev = node;
                head = node;
            }
        }

        /// <summary>
        /// 将元素添加到链表的尾部
        /// </summary>
        /// <param name="item">要添加的元素</param>
        /// <returns>添加到链表尾部的节点。</returns>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void InsertLast(T item)
        {
            var node = new RoughLinkedListNode<T>(tail, item);

            if (tail is null)
            {
                head = node;
                tail = node;
            }
            else
            {
                node.Prev = tail;

                tail.Next = node;
                tail = node;
            }
        }

        /// <summary>
        /// 将节点添加到链表的尾部
        /// </summary>
        /// <param name="node">要添加的节点</param>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void InsertLast(RoughLinkedListNode<T> node)
        {
            if (tail is null)
            {
                head = node;
                tail = node;
            }
            else
            {
                node.Prev = tail;

                tail.Next = node;
                tail = node;
            }
        }

        /// <summary>
        /// 将节点移动到链表的头部
        /// </summary>
        /// <param name="node">要移动的节点</param>
        /// <returns>若移动了 <paramref name="node"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        protected bool MoveToHead(RoughLinkedListNode<T> node)
        {
            if (head == node)
            {
                return false;
            }
            else
            {
                node.Prev.Next = node.Next;
            }

            if (tail == node)
            {
                tail = node.Prev;
            }
            else
            {
                node.Next.Prev = node.Prev;
            }

            node.Prev = null;
            node.Next = head;

            head = node;
            return true;
        }

        /// <summary>
        /// 将节点移动到链表的尾部
        /// </summary>
        /// <param name="node">要移动的节点</param>
        /// <returns>若移动了 <paramref name="node"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        protected bool MoveToTail(RoughLinkedListNode<T> node)
        {
            if (tail == node)
            {
                return false;
            }
            else
            {
                node.Next.Prev = node.Prev;
            }

            if (head == node)
            {
                head = node.Next;
            }
            else
            {
                node.Prev.Next = node.Next;
            }

            node.Prev = tail;
            node.Next = null;

            tail = node;
            return true;
        }

        /// <summary>
        /// 将节点从链表中移除
        /// </summary>
        /// <param name="node">要移除的节点</param>
        /// <remarks>该方法不修改 <see cref="count"/> 字段。</remarks>
        protected void PickUp(RoughLinkedListNode<T> node)
        {
            if (head == node)
            {
                head = node.Next;
            }
            else
            {
                node.Prev.Next = node.Next;
            }

            if (tail == node)
            {
                tail = node.Prev;
            }
            else
            {
                node.Next.Prev = node.Prev;
            }

            node.Prev = null;
            node.Next = null;
        }

        #endregion


        //------------------------------------------------------
        //
        //  IEnumerable
        //
        //------------------------------------------------------

        #region IEnumerable

        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>可用于循环访问集合的枚举器。</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>可用于循环访问集合的枚举器。</returns>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        #endregion


        /// <summary>
        /// <see cref="RoughLinkedListBase{T}"/> 的枚举器
        /// </summary>
        private class Enumerator : IEnumerator<T>
        {
            /// <summary>
            /// 获取枚举器当前指向位置对应的集合元素
            /// </summary>
            public T Current => _currentValue;


            /// <summary>
            /// 初始化 <see cref="Enumerator"/> 的新实例
            /// </summary>
            /// <param name="list">枚举器要枚举的 <see cref="RoughLinkedListBase{T}"/></param>
            public Enumerator(RoughLinkedListBase<T> list)
            {
                _list = list;
                _currentNode = _list.head;
            }


            //------------------------------------------------------
            //
            //  Public Methods
            //
            //------------------------------------------------------

            #region Public Methods

            /// <summary>
            /// 释放该枚举器占用的资源
            /// </summary>
            public void Dispose()
            {
                _currentNode = null;
                _currentValue = default;
            }

            /// <summary>
            /// 将枚举器指向集合的位置后移一位
            /// </summary>
            /// <returns>若成功后移，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
            public bool MoveNext()
            {
                if (_currentNode is null || _currentNode.Next is null)
                {
                    return false;
                }

                _currentNode = _currentNode.Next;
                _currentValue = _currentNode.Item;

                return true;
            }

            public void Reset()
            {
                _currentNode = _list.head;
                _currentValue = default;
            }

            #endregion


            /// <summary>
            /// 获取枚举器当前指向位置对应的集合元素
            /// </summary>
            object IEnumerator.Current => _currentValue;


            //------------------------------------------------------
            //
            //  Private Fields
            //
            //------------------------------------------------------

            #region Private Fields

            private readonly RoughLinkedListBase<T> _list;

            private RoughLinkedListNode<T> _currentNode;
            private T _currentValue;

            #endregion
        }
    }


    internal sealed class RoughLinkedListDebugView
    {
        public RoughLinkedListDebugView(IEnumerable list)
        {
            _list = list;
        }


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Items
        {
            get
            {
                return _list.Cast<object>().ToArray();
            }
        }


        private readonly IEnumerable _list;
    }
}
