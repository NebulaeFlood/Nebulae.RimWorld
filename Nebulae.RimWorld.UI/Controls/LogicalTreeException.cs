using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    [Serializable]
    internal sealed class LogicalTreeException : Exception
    {
        internal LogicalTreeException(string message, Control control) :
            base(CreateMessageText(message, control))
        {
        }

        internal LogicalTreeException(string message, Control control, Exception innerException) :
            base(CreateMessageText(message, control, hasInnerException: true), innerException)
        {
        }

        private static string CreateMessageText(string message, Control control, bool hasInnerException = false)
        {
            var sb = new StringBuilder("A control ");

            if (!string.IsNullOrEmpty(control.Name))
            {
                sb.Append("named: ").Append(control.Name).Append(' ');
            }

            sb.Append("raised a logical tree error: ").Append(message);

            if (hasInnerException)
            {
                sb.Append("\n--->");
            }

            return sb.ToString();
        }
    }
}
