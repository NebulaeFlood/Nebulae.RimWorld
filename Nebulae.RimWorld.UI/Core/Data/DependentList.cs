using Nebulae.RimWorld.Collections;
using Nebulae.RimWorld.UI.Core.Data.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data
{
    internal sealed class DependentList : RoughLinkedListBase<WeakReference<Expression>>
    {
        internal void Add(Expression expression)
        {
            var node = head;

            while (node != null)
            {
                if (!node.Item.TryGetTarget(out var target))
                {
                    var next = node.Next;

                    count--;
                    PickUp(node);

                    node = next;
                }
                else if (target == expression)
                {
                    return;
                }
                else
                {
                    node = node.Next;
                }
            }

            count++;
            InsertLast(new WeakReference<Expression>(expression));
        }

        internal void Remove(Expression expression)
        {
            var node = head;

            while (node != null)
            {
                if (!node.Item.TryGetTarget(out var target))
                {
                    var next = node.Next;

                    count--;
                    PickUp(node);

                    node = next;
                }
                else if (target == expression)
                {
                    count--;
                    PickUp(node);
                    return;
                }
                else
                {
                    node = node.Next;
                }
            }
        }

        internal void Update(DependencyObject source, DependencyProperty property, object value)
        {
            var node = head;

            while (node != null)
            {
                if (!source.updatableProperties.Contains(property))
                {
                    return;
                }

                if (node.Item.TryGetTarget(out var target))
                {
                    target.Update(source, property, value);
                    node = node.Next;
                }
                else
                {
                    var next = node.Next;

                    count--;
                    PickUp(node);

                    node = next;
                }
            }
        }
    }
}
