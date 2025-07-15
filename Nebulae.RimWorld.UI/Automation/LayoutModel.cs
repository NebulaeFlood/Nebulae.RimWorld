using Nebulae.RimWorld.UI.Automation.Attributes;
using Nebulae.RimWorld.UI.Core.Data.Utilities;
using System.Reflection;
using System.Text;
using Verse;

namespace Nebulae.RimWorld.UI.Automation
{
    internal readonly struct LayoutModel
    {
        public static readonly StringBuilder KeyBuilder = new StringBuilder(0);

        public readonly LayoutEntryBaseAttribute Info;
        public readonly LayoutEntryType EntryType;

        public readonly string Label;
        public readonly string Name;
        public readonly string Tooltip;

        public readonly object Owner;
        public readonly object Value;


        internal LayoutModel(string translationKeyPrefix, LayoutEntryBaseAttribute info, MemberInfo member, object owner)
        {
            Info = info;
            EntryType = info.EntryType;
            Name = member.Name;
            Owner = owner;

            string key = info.GetLabel(member);

            Label = KeyBuilder.Append(translationKeyPrefix).Append('.').Append(key).Append(".Label").ToString().Translate().Resolve();

            if (info.Prompted)
            {
                KeyBuilder.Clear();
                Tooltip = KeyBuilder.Append(translationKeyPrefix).Append('.').Append(key).Append(".Tooltip").ToString().Translate().Resolve();
            }
            else
            {
                Tooltip = string.Empty;
            }

            switch (EntryType)
            {
                case LayoutEntryType.Boolean:
                    Value = member.GetValue<bool>(owner);
                    break;
                case LayoutEntryType.Number:
                    Value = member.GetValue<float>(owner);
                    break;
                case LayoutEntryType.String:
                    Value = member.GetValue<string>(owner);
                    break;
                default:
                    Value = null;
                    break;
            }

            KeyBuilder.Clear();
        }
    }
}
