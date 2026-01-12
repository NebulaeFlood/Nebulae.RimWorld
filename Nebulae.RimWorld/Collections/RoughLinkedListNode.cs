using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Nebulae.RimWorld.Collections
{
    /// <summary>
    /// <see cref="RoughLinkedListBase{T}"/> 的节点
    /// </summary>
    /// <typeparam name="T">节点存储元素的类型</typeparam>
    public sealed class RoughLinkedListNode<T> : IEquatable<RoughLinkedListNode<T>>
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 节点元素
        /// </summary>
        public T Item;

        /// <summary>
        /// 前驱节点
        /// </summary>
        public RoughLinkedListNode<T> Prev;

        /// <summary>
        /// 后继节点
        /// </summary>
        public RoughLinkedListNode<T> Next;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="RoughLinkedListNode{T}"/> 的新实例
        /// </summary>
        /// <param name="item">节点元素</param>
        public RoughLinkedListNode(T item)
        {
            Item = item;
        }

        /// <summary>
        /// 初始化 <see cref="RoughLinkedListNode{T}"/> 的新实例
        /// </summary>
        /// <param name="item">节点元素</param>
        /// <param name="next">后继节点</param>
        internal RoughLinkedListNode(T item, RoughLinkedListNode<T> next)
        {
            Item = item;

            Next = next;
        }

        /// <summary>
        /// 初始化 <see cref="RoughLinkedListNode{T}"/> 的新实例
        /// </summary>
        /// <param name="prev">前驱节点</param>
        /// <param name="item">节点元素</param>
        internal RoughLinkedListNode(RoughLinkedListNode<T> prev, T item)
        {
            Item = item;

            Prev = prev;
        }

        /// <summary>
        /// 初始化 <see cref="RoughLinkedListNode{T}"/> 的新实例
        /// </summary>
        /// <param name="prev">前驱节点</param>
        /// <param name="item">节点元素</param>
        /// <param name="next">后继节点</param>
        internal RoughLinkedListNode(RoughLinkedListNode<T> prev, T item, RoughLinkedListNode<T> next)
        {
            Item = item;

            Prev = prev;
            Next = next;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断该节点是否在另一个节点之后
        /// </summary>
        /// <param name="node">要比较的节点</param>
        /// <returns>若该节点位于 <paramref name="node"/> 之后，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool After(RoughLinkedListNode<T> node)
        {
            while (node is not null)
            {
                if (this == node.Next)
                {
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        /// <summary>
        /// 判断该节点是否在另一个节点之前
        /// </summary>
        /// <param name="node">要比较的节点</param>
        /// <returns>若该节点位于 <paramref name="node"/> 之前，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Before(RoughLinkedListNode<T> node)
        {
            while (node is not null)
            {
                if (this == node.Prev)
                {
                    return true;
                }

                node = node.Prev;
            }

            return false;
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            return obj is RoughLinkedListNode<T> other
                && Prev == other.Prev
                && Next == other.Next
                && Equals(Item, other.Item);
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(RoughLinkedListNode<T> other)
        {
            return Prev == other.Prev
                && Next == other.Next
                && Equals(Item, other.Item);
        }

        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Prev, Item, Next);
        }

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return Item.AsLog();
        }

        #endregion
    }
}
