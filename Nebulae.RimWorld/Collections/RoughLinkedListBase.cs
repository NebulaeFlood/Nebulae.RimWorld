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
    public abstract class RoughLinkedListBase<T> : IEnumerable<T>
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
        /// 获取该链表中包含元素的数量
        /// </summary>
        public int Count => count;

        /// <summary>
        /// 获取一个值，该值指示此链表是否不包含任何元素
        /// </summary>
        public bool IsEmpty => head is null;

        #endregion


        /// <summary>
        /// 为 <see cref="RoughLinkedListBase{T}"/> 派生类实现基本初始化
        /// </summary>
        protected RoughLinkedListBase() { }


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

            count++;
        }

        /// <summary>
        /// 将节点插入到指定节点之后
        /// </summary>
        /// <param name="index">作为索引的节点</param>
        /// <param name="node">要插入的节点</param>
        /// <remarks>需保证 <paramref name="node"/> 不在该链表中。</remarks>
        protected void InsertAfter(RoughLinkedListNode<T> index, RoughLinkedListNode<T> node)
        {
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

            count++;
        }

        /// <summary>
        /// 将元素插入到指定节点之前
        /// </summary>
        /// <param name="index">作为索引的节点</param>
        /// <param name="item">要插入的元素</param>
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

            count++;
        }

        /// <summary>
        /// 将节点插入到指定节点之前
        /// </summary>
        /// <param name="index">作为索引的节点</param>
        /// <param name="node">要插入的节点</param>
        /// <remarks>需保证 <paramref name="node"/> 不在该链表中。</remarks>
        protected void InsertBefore(RoughLinkedListNode<T> index, RoughLinkedListNode<T> node)
        {
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

            count++;
        }

        /// <summary>
        /// 将元素添加到链表的头部
        /// </summary>
        /// <param name="item">要添加的元素</param>
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

            count++;
        }

        /// <summary>
        /// 将节点添加到链表的头部
        /// </summary>
        /// <param name="node">要添加的节点</param>
        /// <returns>添加到链表头部的节点。</returns>
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

            count++;
        }

        /// <summary>
        /// 将元素添加到链表的尾部
        /// </summary>
        /// <param name="item">要添加的元素</param>
        /// <returns>添加到链表尾部的节点。</returns>
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

            count++;
        }

        /// <summary>
        /// 将节点添加到链表的尾部
        /// </summary>
        /// <param name="node">要添加的节点</param>
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

            count++;
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

            count--;
        }

        #endregion


        //------------------------------------------------------
        //
        //  IEnumerable
        //
        //------------------------------------------------------

        #region IEnumerable

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        #endregion


        private struct Enumerator : IEnumerator<T>
        {
            public readonly T Current => _currentValue;


            public Enumerator(RoughLinkedListBase<T> list)
            {
                _list = list;

                _currentNode = null;
                _currentValue = default;
            }


            //------------------------------------------------------
            //
            //  Public Methods
            //
            //------------------------------------------------------

            #region Public Methods

            public void Dispose()
            {
                _currentNode = null;
                _currentValue = default;
            }

            public bool MoveNext()
            {
                if (_currentNode is null)
                {
                    if (_list.count < 1)
                    {
                        return false;
                    }

                    _currentNode = _list.head;
                    _currentValue = _currentNode.Item;

                    return true;
                }

                if (_currentNode.Next is null)
                {
                    return false;
                }

                _currentNode = _currentNode.Next;
                _currentValue = _currentNode.Item;

                return true;
            }

            public void Reset()
            {
                _currentNode = null;
                _currentValue = default;
            }

            #endregion


            readonly object IEnumerator.Current => _currentValue;


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
