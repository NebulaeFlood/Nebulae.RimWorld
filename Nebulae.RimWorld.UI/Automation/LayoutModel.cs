using Nebulae.RimWorld.UI.Automation.Attributes;
using Nebulae.RimWorld.UI.Core.Data.Utilities;
using Nebulae.RimWorld.UI.Utilities;
using System.Reflection;
using System.Text;
using Verse;

namespace Nebulae.RimWorld.UI.Automation
{
    internal readonly struct LayoutModel
    {
        public readonly LayoutEntryBaseAttribute Info;
        public readonly LayoutEntryType EntryType;

        public readonly string Label;
        public readonly string Name;
        public readonly string Tooltip;

        public readonly object Owner;
        public readonly object Value;


        internal LayoutModel(StringBuilder keyBuilder, string translationKeyPrefix, LayoutEntryBaseAttribute info, MemberInfo member, object owner)
        {
            Info = info;
            EntryType = info.EntryType;
            Name = member.Name;
            Owner = owner;

            string key = info.GetLabel(member);
            Label = keyBuilder.Append(translationKeyPrefix).Append('.').Append(key).Append(".Label").ToString().Translate().ResolveSafe();

            if (info.Prompted)
            {
                keyBuilder.Remove(keyBuilder.Length - 6, 6);
                keyBuilder.Append(".Tooltip");

                Tooltip = keyBuilder.ToString().Translate().ResolveSafe();
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

            keyBuilder.Clear();
        }
    }
}
