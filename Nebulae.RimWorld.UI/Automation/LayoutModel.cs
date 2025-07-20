using Nebulae.RimWorld.UI.Automation.Attributes;
using Nebulae.RimWorld.UI.Core.Data.Utilities;
using System.Reflection;
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


        internal LayoutModel(string translationKeyPrefix, LayoutEntryBaseAttribute info, MemberInfo member, object owner)
        {
            Info = info;
            EntryType = info.EntryType;
            Name = member.Name;
            Owner = owner;

            string key = info.GetLabel(member);
            Label = $"{translationKeyPrefix}.{key}.Label".Translate().Resolve();

            if (info.Prompted)
            {
                Tooltip = $"{translationKeyPrefix}.{key}.Tooltip".Translate().Resolve();
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
        }
    }
}
