using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Utilities
{
    internal static class HitTestUtility
    {
        internal static void InputHitTest(Control control)
        {
            inputHitTest = true;

            Results.TransferTo(PreviousResults);
            control.HitTest(Event.current.mousePosition);

            inputHitTest = false;
        }


        //------------------------------------------------------
        //
        //  Internal Static Fields
        //
        //------------------------------------------------------

        #region Internal Static Fields

        internal static readonly HitTestResults PreviousResults = new HitTestResults();
        internal static readonly HitTestResults Results = new HitTestResults();

        internal static bool inputHitTest;

        #endregion


        internal sealed class HitTestResults : LinkedListThin<Control>
        {
            internal Control HoveredControl => head?.Data;


            internal void Add(Control item)
            {
                AddLast(item);
            }

            internal void TransferTo(HitTestResults other)
            {
                other.count = count;
                other.head = head;
                other.tail = tail;

                count = 0;
                head = null;
                tail = null;
            }
        }
    }
}
