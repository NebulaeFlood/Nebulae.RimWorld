using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Text;

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
            var sb = new StringBuilder("A control ");

            if (!string.IsNullOrEmpty(control.Name))
            {
                sb.Append("named: ")
                    .Append(control.Name)
                    .Append(' ');
            }

            sb.Append("raised a logical tree error: ")
              .Append(message);

            if (hasInnerException)
            {
                sb.Append("\n--->");
            }

            return sb.ToString();
        }
    }
}
