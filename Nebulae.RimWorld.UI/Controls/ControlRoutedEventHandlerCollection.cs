using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core;
using Nebulae.RimWorld.UI.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    internal sealed class ControlRoutedEventHandlerCollection : LinkedListThin<IRoutedEventHandler>
    {
        internal ControlRoutedEventHandlerCollection(IRoutedEventHandler handler) : base(handler) { }


        internal void Add(IRoutedEventHandler handler)
        {
            AddLast(handler);
        }

        internal void Fill(Control owner, RoutedEvent @event, EventRoute route)
        {
            var node = head;

            while (node != null)
            {
                route.Add(node.Data);
            }

            if (@event.TryGetClassHandler(owner.Type, out var handler))
            {
                route.Add(handler);
            }
        }

        internal void Invoke(Control sender, RoutedEventArgs args)
        {
            var node = head;

            while (node != null)
            {
                node.Data.Invoke(sender, args);

                if (args.Handled)
                {
                    return;
                }

                node = node.next;
            }

            if (args.RoutedEvent.TryGetClassHandler(sender.Type, out var classHandler))
            {
                classHandler.Invoke(sender, args);
            }

            return;
        }

        internal void Purge()
        {
            if (Count < 1)
            {
                return;
            }

            var node = head;

            while (node != null)
            {
                if (!node.Data.IsAlive)
                {
                    var nextNode = node.next;
                    Remove(node);
                    node = nextNode;
                }
                else
                {
                    node = node.next;
                }
            }
        }

        internal void Remove(Delegate handler)
        {
            var node = tail;

            while (node != null)
            {
                if (!node.Data.IsAlive || node.Data.Equals(handler))
                {
                    var prevNode = node.prev;
                    Remove(node);
                    node = prevNode;
                }
                else
                {
                    node = node.prev;
                }
            }
        }
    }
}
