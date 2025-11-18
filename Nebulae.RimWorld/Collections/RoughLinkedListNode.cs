using Nebulae.RimWorld.Utilities;

namespace Nebulae.RimWorld.Collections
{
    /// <summary>
    /// 由 <see cref="RoughLinkedListBase{T}"/> 实现的链表的节点
    /// </summary>
    /// <typeparam name="T">节点存储元素的类型</typeparam>
    public sealed class RoughLinkedListNode<T>
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
