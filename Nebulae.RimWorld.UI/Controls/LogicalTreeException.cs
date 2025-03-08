using Nebulae.RimWorld.UI.Controls.Basic;
using System;

namespace Nebulae.RimWorld.UI.Controls
{
    [Serializable]
    internal sealed class LogicalTreeException : Exception
    {
        internal LogicalTreeException(Visual control, string message) :
            base(CreateMessageText(control, message))
        {
        }

        internal LogicalTreeException(Visual control, string message, Exception innerException) :
            base(CreateMessageText(control, message, hasInnerException: true), innerException)
        {
        }

        private static string CreateMessageText(Visual control, string message, bool hasInnerException = false)
        {
            string test = "An control";

            test += string.IsNullOrEmpty(control.Name)
                ? string.Empty
                : " named: " + control.Name;

            test += "raised an logical tree error:" + message;

            if (hasInnerException)
            {
                test += "\n--->";
            }

            return test;
        }
    }
}
