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

            Results.Clear();
            control.HitTest(Event.current.mousePosition);

            inputHitTest = false;
        }

        internal static void InputHitTestIndependently(Control control)
        {
            inputHitTest = true;

            Results.TransferTo(_results);
            control.HitTest(Event.current.mousePosition);

            if (Results.IsEmpty)
            {
                _results.TransferTo(Results);
            }
            else
            {
                _results.Clear();
            }

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


        private static readonly HitTestResults _results = new HitTestResults();


        internal sealed class HitTestResults : LinkedListThin<Control>
        {
            internal Control HoveredControl => tail?.Data;


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
