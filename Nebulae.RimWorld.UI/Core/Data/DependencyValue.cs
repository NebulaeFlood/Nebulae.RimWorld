using Nebulae.RimWorld.Collections;
using Nebulae.RimWorld.UI.Core.Data.Expressions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Core.Data
{
    internal sealed class DependencyValue : RoughLinkedListBase<DependencyValueEntry>
    {
        public readonly PropertyMetadata Metadata;

        public Expression Expression;


        public DependencyValue(PropertyMetadata metadata)
        {
            Metadata = metadata;
        }


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal object GetValue()
        {
            return head is null ? Metadata.DefaultValue : head.Item.Value;
        }

        /// <summary>
        /// 未设置对应优先级的值时，返回默认值
        /// </summary>
        internal object GetValue(ValuePrecedence precedence)
        {
            var node = head;

            while (node != null)
            {
                if (node.Item.Precedence == precedence)
                {
                    return node.Item.Value;
                }

                node = node.Next;
            }

            return Metadata.DefaultValue;
        }

        internal object GetValue(DependencyObject owner, DependencyProperty property)
        {
            if (head is null)
            {
                if (Expression is null)
                {
                    return Metadata.DefaultValue;
                }

                object rawValue = Expression.GetValue(owner, property);
                property.ValidateValue(rawValue);

                object value = owner.CoerceValue(property, Metadata, rawValue, out bool isCoerced);

                if (isCoerced)
                {
                    SaveValue(rawValue, value, ValuePrecedence.Cache);
                }
                else
                {
                    SaveValue(value, ValuePrecedence.Cache);
                }

                return head.Item.Value;
            }

            if (Expression is null || head.Item.Precedence > ValuePrecedence.Local)
            {
                if (head.Item.IsRaw)
                {
                    object value = owner.CoerceValue(property, Metadata, head.Item.Value, out bool isCoerced);

                    if (isCoerced)
                    {
                        SaveValue(value, ValuePrecedence.Coercion);
                    }
                }
            }
            else
            {
                object rawValue = Expression.GetValue(owner, property);
                property.ValidateValue(rawValue);

                object value = owner.CoerceValue(property, Metadata, rawValue, out bool isCoerced);

                if (isCoerced)
                {
                    SaveValue(rawValue, value, ValuePrecedence.Cache);
                }
                else
                {
                    SaveValue(value, ValuePrecedence.Cache);
                }
            }

            return head.Item.Value;
        }

        /// <summary>
        /// 移除优先级为 <paramref name="precedence"/> 的有效值
        /// </summary>
        internal void RemoveValue(ValuePrecedence precedence)
        {
            var node = head;

            while (node != null)
            {
                if (node.Item.Precedence == precedence)
                {
                    PickUp(node);
                    return;
                }

                node = node.Next;
            }
        }

        internal void SetValue(DependencyObject owner, DependencyProperty property, object rawValue, ValuePrecedence precedence)
        {
            object value = owner.CoerceValue(property, Metadata, rawValue, out bool isCoerced);

            if (isCoerced)
            {
                SaveValue(rawValue, value, precedence);
            }
            else
            {
                SaveValue(value, precedence);
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private void SaveValue(object value, ValuePrecedence precedence)
        {
            if (head is null)
            {
                InsertLast(new DependencyValueEntry(value, precedence, isRaw: false));
                return;
            }

            var node = head;

            do
            {
                if (node.Item.Precedence > precedence)
                {
                    node = node.Next;
                }
                else if (node.Item.Precedence < precedence)
                {
                    InsertBefore(node, new DependencyValueEntry(value, precedence, isRaw: false));
                    return;
                }
                else
                {
                    node.Item = new DependencyValueEntry(value, precedence, isRaw: false);
                    return;
                }
            }
            while (node != null);

            InsertLast(new DependencyValueEntry(value, precedence, isRaw: false));
        }

        private void SaveValue(object rawValue, object coercedValue, ValuePrecedence precedence)
        {
            if (head is null)
            {
                InsertLast(new DependencyValueEntry(rawValue, precedence, isRaw: true));
                InsertLast(new DependencyValueEntry(coercedValue, ValuePrecedence.Coercion, isRaw: false));
                return;
            }

            if (head.Item.Precedence is ValuePrecedence.Coercion)
            {
                head.Item = new DependencyValueEntry(coercedValue, ValuePrecedence.Coercion, isRaw: false);
            }
            else
            {
                InsertFirst(new DependencyValueEntry(coercedValue, ValuePrecedence.Coercion, isRaw: false));
            }

            var node = head;

            do
            {
                if (node.Item.Precedence > precedence)
                {
                    node = node.Next;
                }
                else if (node.Item.Precedence < precedence)
                {
                    InsertBefore(node, new DependencyValueEntry(rawValue, precedence, isRaw: true));
                    return;
                }
                else
                {
                    node.Item = new DependencyValueEntry(rawValue, precedence, isRaw: true);
                    return;
                }
            }
            while (node != null);

            InsertLast(new DependencyValueEntry(rawValue, precedence, isRaw: true));
        }

        #endregion
    }


    internal readonly struct DependencyValueEntry
    {
        public static readonly DependencyValueEntry Empty = new DependencyValueEntry();

        public readonly ValuePrecedence Precedence;
        public readonly object Value;

        public readonly bool IsRaw;
        public readonly bool IsValid;


        public DependencyValueEntry(object value, ValuePrecedence precedence, bool isRaw)
        {
            Value = value;
            Precedence = precedence;

            IsRaw = isRaw;
            IsValid = true;
        }
    }
}
