using System;

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
            base(CreateMessageText(control, message, hasInnerException: true), innerException)
        {
        }

        private static string CreateMessageText(Control control, string message, bool hasInnerException = false)
        {
            string test = "An control";

            test += string.IsNullOrEmpty(control.Name)
                ? string.Empty
                : " named: " + control.Name;

            test += "raised an logical tree error: " + message;

            if (hasInnerException)
            {
                test += " --->";
            }

            return test;
        }
    }
}
