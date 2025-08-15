using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Verse.AI;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// <see cref="Control"/> 的不重复有序集合
    /// </summary>
    public class ControlCollection<T> : ICollection<T> where T : Control
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取该 <see cref="ControlCollection{T}"/> 包含的控件数
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 获取一个值，该值指示此 <see cref="ControlCollection{T}"/> 是否为只读
        /// </summary>
        public bool IsReadOnly => false;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="ControlCollection{T}"/> 的新实例
        /// </summary>
        public ControlCollection()
        {
            _count = 0;
            _hashTable = new Dictionary<T, Node>();
        }

        /// <summary>
        /// 初始化 <see cref="ControlCollection{T}"/> 的新实例
        /// </summary>
        /// <param name="items">要添加到集合的控件</param>
        public ControlCollection(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _count = 0;
            _hashTable = new Dictionary<T, Node>();

            foreach (var item in items)
            {
                _count++;
                _hashTable[item] = AddLast(item);

                OnAddedControl(item);
            }

            _hashTable.TrimExcess();
            OnCollectionChanged();
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将控件添加或移动到集合末尾
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_hashTable.TryGetValue(item, out var node))
            {
                PickToLast(node);
            }
            else
            {
                _count++;
                _hashTable[item] = AddLast(item);

                OnAddedControl(item);
            }

            OnCollectionChanged();
        }

        /// <summary>
        /// 从集合中删除所有控件
        /// </summary>
        public void Clear()
        {
            if (_count < 1)
            {
                return;
            }

            _hashTable.Clear();

            _tail = null;

            while (_head != null)
            {
                _count--;

                OnRemovedControl(_head.value);
                _head = _head.next;
            }

            OnCollectionChanged();
        }

        /// <summary>
        /// 确定控件是否在集合中
        /// </summary>
        /// <param name="item">要确定的控件</param>
        /// <returns>如果在集合中找到 <paramref name="item"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(T item)
        {
            return _hashTable.ContainsKey(item);
        }

        /// <summary>
        /// 从目标数组的指定索引处开始，复制集合内的控件
        /// </summary>
        /// <param name="array">接收集合内控件的数组</param>
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

            if (arrayIndex + _count > capcity)
            {
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof(array));
            }

            var node = _head;

            while (node != null)
            {
                array[arrayIndex++] = node.value;
                node = node.next;
            }
        }

        /// <summary>
        /// 获取集合中满足指定条件的所有控件
        /// </summary>
        /// <param name="predicate">要搜索的控件应满足的条件</param>
        /// <returns>满足 <paramref name="predicate"/> 的所有控件。</returns>
        public IEnumerable<T> FindAll(Predicate<T> predicate)
        {
            if (_count < 1)
            {
                yield break;
            }

            var node = _head;

            while (node != null)
            {
                if (predicate(node.value))
                {
                    yield return node.value;
                }

                node = node.next;
            }
        }

        /// <summary>
        /// 对集合中的每个控件执行指定操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public void ForEach(Action<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var node = _head;

            while (node != null)
            {
                action(node.value);
                node = node.next;
            }
        }

        /// <summary>
        /// 以倒序对集合中的每个控件执行指定操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public void ForEachReverse(Action<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var node = _tail;

            while (node != null)
            {
                action(node.value);
                node = node.prev;
            }
        }

        /// <summary>
        /// 将控件按视觉逻辑插入或移动到指定控件处
        /// </summary>
        /// <param name="index">作为索引的控件</param>
        /// <param name="item">要插入的控件</param>
        /// <returns>若插入了 <paramref name="item"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Insert(T index, T item)
        {
            if (index is null || item is null || index == item || !_hashTable.TryGetValue(index, out var indexNode))
            {
                return false;
            }

            if (!_hashTable.TryGetValue(item, out var controlNode))
            {
                _count++;
                _hashTable[item] = controlNode = AddLast(item);

                InsetBefore(indexNode, controlNode);
                OnAddedControl(item);
            }
            else
            {
                if (controlNode.Before(indexNode))
                {
                    InsetAfter(indexNode, controlNode);
                }
                else
                {
                    InsetBefore(indexNode, controlNode);
                }
            }

            OnCollectionChanged();
            return true;
        }

        /// <summary>
        /// 从集合中删除所有控件，将新控件添加到集合
        /// </summary>
        /// <param name="items">要添加的控件</param>
        public void Override(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Clear();

            foreach (var item in items)
            {
                _count++;
                _hashTable[item] = AddLast(item);

                OnAddedControl(item);
            }

            _hashTable.TrimExcess();
            OnCollectionChanged();
        }

        /// <summary>
        /// 将指定控件从集合中移除
        /// </summary>
        /// <param name="item">要移除的控件</param>
        /// <returns>若移除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(T item)
        {
            if (!_hashTable.Remove(item, out var node))
            {
                return false;
            }

            _count--;

            PickUp(node);
            OnRemovedControl(item);
            OnCollectionChanged();

            return true;
        }

        /// <summary>
        /// 使用控件名称对集合中的控件排序
        /// </summary>
        public void Sort()
        {
            if (_count < 1)
            {
                return;
            }

            var sorter = new MergeSorter(_head, _count, StringComparer.CurrentCulture.Compare);

            _head = sorter.Head;
            _tail = sorter.Tail;

            OnCollectionChanged();
        }

        /// <summary>
        /// 使用指定的比较器对集合中的控件排序
        /// </summary>
        /// <param name="comparison">比较控件时要使用的比较器</param>
        public void Sort(Comparison<T> comparison)
        {
            if (comparison is null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            if (_count < 1)
            {
                return;
            }

            var sorter = new MergeSorter(_head, _count, comparison);

            _head = sorter.Head;
            _tail = sorter.Tail;

            OnCollectionChanged();
        }

        /// <summary>
        /// 使用指定的比较器排序子控件
        /// </summary>
        /// <param name="comparer">比较子控件时使用的比较器</param>
        public void Sort(IComparer<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (_count < 1)
            {
                return;
            }

            var sorter = new MergeSorter(_head, _count, comparer.Compare);

            _head = sorter.Head;
            _tail = sorter.Tail;

            OnCollectionChanged();
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
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var node = _head;

            while (node != null)
            {
                yield return node.value;
                node = node.next;
            }
        }

        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>可用于循环访问集合的枚举器。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            var node = _head;

            while (node != null)
            {
                yield return node.value;
                node = node.next;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 当控件被添加到该 <see cref="ControlCollection{T}"/> 时调用
        /// </summary>
        /// <param name="control">被添加的控件</param>
        protected virtual void OnAddedControl(T control) { }

        /// <summary>
        /// 当该 <see cref="ControlCollection{T}"/> 变化时调用
        /// </summary>
        protected virtual void OnCollectionChanged() { }

        /// <summary>
        /// 当控件从该 <see cref="ControlCollection{T}"/> 移除时调用
        /// </summary>
        /// <param name="control">被移除的控件</param>
        protected virtual void OnRemovedControl(T control) { }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private Node AddLast(T control)
        {
            if (_head is null)
            {
                return _head = _tail = new Node(control);
            }
            else
            {
                return _tail = _tail.next = new Node(_tail, control);
            }
        }

        private void InsetAfter(Node index, Node node)
        {
            PickUp(node);

            if (index != _tail)
            {
                index.next.prev = node;
                node.next = index.next;
            }

            index.next = node;
            node.prev = index;
        }

        private void InsetBefore(Node index, Node node)
        {
            PickUp(node);

            if (index != _head)
            {
                index.prev.next = node;
                node.prev = index.prev;
            }

            index.prev = node;
            node.next = index;
        }

        private void PickToLast(Node node)
        {
            if (_tail == node)
            {
                return;
            }
            else
            {
                node.next.prev = node.prev;
            }

            if (_head == node)
            {
                _head = node.next;
            }
            else
            {
                node.prev.next = node.next;
            }

            node.prev = _tail;
            node.next = null;

            _tail = node;
        }

        private void PickUp(Node node)
        {
            if (_head == node)
            {
                _head = node.next;
            }
            else
            {
                node.prev.next = node.next;
            }

            if (_tail == node)
            {
                _tail = node.prev;
            }
            else
            {
                node.next.prev = node.prev;
            }

            node.prev = null;
            node.next = null;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private int _count;

        private readonly Dictionary<T, Node> _hashTable;

        private Node _head;
        private Node _tail;

        #endregion


        private sealed class Node
        {
            //------------------------------------------------------
            //
            //  Public Fields
            //
            //------------------------------------------------------

            #region Public Fields

            public T value;

            public Node prev;
            public Node next;

            #endregion


            //------------------------------------------------------
            //
            //  Constructors
            //
            //------------------------------------------------------

            #region Constructors

            public Node() { }

            public Node(T control)
            {
                value = control;
            }

            public Node(T control, Node next)
            {
                value = control;

                this.next = next;
            }

            public Node(Node prev, T control)
            {
                value = control;

                this.prev = prev;
            }

            public Node(Node prev, T control, Node next)
            {
                value = control;

                this.prev = prev;
                this.next = next;
            }

            #endregion


            //------------------------------------------------------
            //
            //  Public Methods
            //
            //------------------------------------------------------

            #region Public Methods

            public bool After(Node node)
            {
                var currentNode = node.next;

                while (currentNode != null)
                {
                    if (this == currentNode)
                    {
                        return true;
                    }

                    currentNode = currentNode.next;
                }

                return false;
            }

            public bool Before(Node node)
            {
                var currentNode = node.prev;

                while (currentNode != null)
                {
                    if (this == currentNode)
                    {
                        return true;
                    }

                    currentNode = currentNode.prev;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(prev?.value, value, next?.value);
            }

            public override string ToString() => value.AsLog();

            #endregion
        }

        private readonly struct MergeSorter
        {
            public readonly Node Head;
            public readonly Node Tail;


            public MergeSorter(Node head, int length, Comparison<T> comparison)
            {
                Tail = null;

                var dummy = new Node(null, head);

                for (int step = 1; step < length; step *= 2)
                {
                    Node lastGroupTail = dummy;
                    Node currentHeader = dummy.next;

                    while (currentHeader != null)
                    {
                        Node left = Spilt(currentHeader, step, out Node rest);
                        Node right = Spilt(rest, step, out rest);

                        lastGroupTail.next = Merge(left, right, comparison, out var tail);
                        lastGroupTail.next.prev = lastGroupTail;
                        lastGroupTail = tail;

                        currentHeader = rest;

                        if (currentHeader != null)
                        {
                            currentHeader.prev = lastGroupTail;
                        }
                    }

                    Tail = lastGroupTail;
                }

                Head = dummy.next;
                Head.prev = null;
            }


            private static Node Merge(Node left, Node right, Comparison<T> comparison, out Node tail)
            {
                var dummy = new Node();
                tail = dummy;

                while (true)
                {
                    if (left is null)
                    {
                        tail.next = right;
                        right.prev = tail;
                        break;
                    }

                    if (right is null)
                    {
                        tail.next = left;
                        left.prev = tail;
                        break;
                    }

                    if (comparison(left.value, right.value) <= 0)
                    {
                        tail.next = left;
                        left.prev = tail;
                        left = left.next;
                    }
                    else
                    {
                        tail.next = right;
                        right.prev = tail;
                        right = right.next;
                    }

                    tail = tail.next;
                }

                while (tail.next != null)
                {
                    tail = tail.next;
                }

                dummy.next.prev = null;
                return dummy.next;
            }

            private static Node Spilt(Node head, int length, out Node rest)
            {
                if (head is null)
                {
                    rest = null;
                    return null;
                }

                Node current = head;

                for (int i = 1; i < length && current.next != null; i++)
                {
                    if (current.next is null)
                    {
                        break;
                    }

                    current = current.next;
                }

                rest = current.next;

                if (rest != null)
                {
                    rest.prev = null;
                }

                current.next = null;
                return head;
            }
        }
    }
}
