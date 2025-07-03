using System.Collections;
using System.Collections.Generic;


namespace Nebulae.RimWorld.UI.Core
{
    internal abstract class LinkedListThin<T> : IEnumerable<T> where T : class
    {
        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        internal int Count => count;
        internal bool IsEmpty => count is 0;
        internal Node Head => head;
        internal Node Tail => tail;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        protected LinkedListThin()
        {
            count = 0;
        }

        protected LinkedListThin(T value)
        {
            count = 1;

            head = tail = new Node(value);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public void Clear()
        {
            count = 0;

            head = null;
            tail = null;
        }

        public bool Contains(T value)
        {
            if (count < 1)
            {
                return false;
            }

            var node = head;

            while (node != null)
            {
                if (ReferenceEquals(node.Data, value))
                {
                    return true;
                }

                node = node.next;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var node = head;

            while (node != null)
            {
                yield return node.Data;
                node = node.next;
            }
        }

        #endregion


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        protected void AddLast(T value)
        {
            if (count < 1)
            {
                head = tail = new Node(value);
            }
            else
            {
                tail = tail.next = new Node(tail, value);
            }

            count++;
        }

        protected void Insert(Node index, T value)
        {
            if (count < 1)
            {
                head = tail = new Node(value);
            }
            else
            {
                if (ReferenceEquals(head, index))
                {
                    head = head.prev = new Node(value, index);
                }
                else if (ReferenceEquals(tail, index))
                {
                    tail = tail.next = new Node(index, value);
                }
                else
                {
                    index.prev = index.prev.next = new Node(index.prev, value, index);
                }
            }

            count++;
        }

        protected void Remove(Node node)
        {
            if (count < 1)
            {
                return;
            }
            else
            {
                if (ReferenceEquals(head, node))
                {
                    head = node.next;
                }
                else
                {
                    node.prev.next = node.next;
                }

                if (ReferenceEquals(tail, node))
                {
                    tail = node.prev;
                }
                else
                {
                    node.next.prev = node.prev;
                }

                node.prev = null;
                node.next = null;
            }

            count--;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Fields
        //
        //------------------------------------------------------

        #region Protected Fields

        protected int count;

        protected Node head;
        protected Node tail;

        #endregion


        internal sealed class Node
        {
            internal readonly T Data;

            internal Node prev;
            internal Node next;


            //------------------------------------------------------
            //
            //  Constructors
            //
            //------------------------------------------------------

            #region Constructors

            internal Node(T data)
            {
                Data = data;
            }

            internal Node(T data, Node next)
            {
                Data = data;

                this.next = next;
            }

            internal Node(Node prev, T data)
            {
                Data = data;

                this.prev = prev;
            }

            internal Node(Node prev, T data, Node next)
            {
                Data = data;

                this.prev = prev;
                this.next = next;
            }

            #endregion
        }
    }
}
