using Nebulae.RimWorld.UI.Automation.Attributes;
using Nebulae.RimWorld.UI.Core.Data.Utilities;
using System;
using System.Reflection;
using System.Text;
using Verse;

namespace Nebulae.RimWorld.UI.Automation
{
    internal readonly struct SettingEntryInfo
    {
        public static readonly StringBuilder KeyBuilder = new StringBuilder(0);

        public readonly SettingEntryBaseAttribute EntryInfo;
        public readonly SettingEntryType EntryType;

        public readonly string Label;
        public readonly string Name;
        public readonly string Tooltip;

        public readonly object Owner;
        public readonly object Value;


        internal SettingEntryInfo(string translationKeyPrefix, SettingEntryBaseAttribute info, MemberInfo member, object owner)
        {
            EntryInfo = info;
            EntryType = info.EntryType;
            Name = member.Name;
            Owner = owner;

            string key = info.GetLabel(member);

            Label = KeyBuilder.Append(translationKeyPrefix).Append('.').Append(key).Append(".Label").ToString().Translate().Resolve();

            if (info.Prompted)
            {
                Tooltip = KeyBuilder.Append(translationKeyPrefix).Append('.').Append(key).Append(".Tooltip").ToString().Translate().Resolve();
            }
            else
            {
                Tooltip = string.Empty;
            }

            switch (EntryType)
            {
                case SettingEntryType.Boolean:
                    Value = member.GetValue<bool>(owner);
                    break;
                case SettingEntryType.Number:
                    Value = member.GetValue<float>(owner);
                    break;
                case SettingEntryType.String:
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
