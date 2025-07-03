using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Controls
{
    [DebuggerStepThrough]
    internal sealed class LayoutQueue : LinkedListThin<Control>
    {
        internal void Arrange()
        {
            var node = tail;

            while (node != null)
            {
                node.Data.Arrange(node.Data.DesiredRect);
                node = node.prev;
            }

            Clear();
        }

        internal void Measure()
        {
            var node = tail;

            while (node != null)
            {
                node.Data.Measure(node.Data.DesiredSize);
                node = node.prev;
            }

            Clear();
        }

        internal void Segment()
        {
            var node = tail;

            while (node != null)
            {
                node.Data.Segment(node.Data.VisibleRect);
                node = node.prev;
            }

            Clear();
        }

        internal bool Exist(Control control)
        {
            var node = tail;

            while (node != null)
            {
                if (ReferenceEquals(control, node.Data) || control.IsChildOf(node.Data))
                {
                    return true;
                }

                node = node.prev;
            }

            return false;
        }

        internal void Enqueue(Control control)
        {
            var anyChild = false;
            var node = tail;

            while (node != null)
            {
                if (!anyChild && (ReferenceEquals(control, node.Data) || control.IsChildOf(node.Data)))
                {
                    return;
                }

                if (node.Data.IsChildOf(control))
                {
                    anyChild = true;

                    Remove(node);
                }

                node = node.prev;
            }

            AddLast(control);
        }
    }
}
