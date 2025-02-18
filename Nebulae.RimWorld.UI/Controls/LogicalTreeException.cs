using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Controls
{
    [Serializable]
    internal sealed class LogicalTreeException : Exception
    {
        internal LogicalTreeException(Control control, string message) : 
            base(CreateMessageText(control, message))
        {
        }

        internal LogicalTreeException(Control control, string message, Exception innerException) :
            base(CreateMessageText(control, message), innerException)
        {
        }

        private static string CreateMessageText(Control control, string message)
        {
            string test = "An control";

            test += string.IsNullOrEmpty(control.Name)
                ? string.Empty
                : " named: " + control.Name;

            test += "raised an logical tree error: " + message;

            return test;
        }
    }
}
