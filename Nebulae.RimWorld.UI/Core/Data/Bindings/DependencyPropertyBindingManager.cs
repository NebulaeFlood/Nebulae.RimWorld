using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data.Bindings
{
    internal sealed class DependencyPropertyBindings : LinkedListThin<Binding>
    {
        internal void Add(Binding binding)
        {
            AddLast(binding);
        }

        internal void Remove(Binding binding)
        {
            var node = head;

            while (node != null)
            {
                if (ReferenceEquals(node.Data, binding))
                {
                    Remove(node);
                    return;
                }
            }
        }

        internal void Update(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var node = head;

            while (node != null)
            {
                if (node.Data.IsSource(d, args.Property.Name))
                {
                    node.Data.OnDependencySourceChanged(d, args);
                }
                else if (node.Data.IsTarget(d, args.Property.Name))
                {
                    node.Data.OnDependencyTargetChanged(d, args);
                }

                node = node.next;
            }
        }
    }
}
