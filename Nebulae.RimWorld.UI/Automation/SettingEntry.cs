using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Data.Utilities;
using System;
using System.Reflection;
using Verse;

namespace Nebulae.RimWorld.UI.Automation
{
    internal readonly struct SettingEntry
    {
        internal static readonly Comparison<SettingEntry> Comparison = Compare;

        internal readonly SettingEntryBaseAttribute EntryInfo;
        internal readonly SettingEntryType EntryType;

        internal readonly DependencyProperty Property;

        internal readonly string Label;
        internal readonly string Name;
        internal readonly string Tooltip;

        internal readonly object Owner;
        internal readonly object Value;


        internal SettingEntry(
            string standardKey,
            SettingEntryBaseAttribute info,
            MemberInfo member,
            Type ownerType,
            object owner)
        {
            EntryInfo = info;
            EntryType = info.EntryType;
            Name = member.Name;
            Owner = owner;

            string key = info.GetLabel(member);

            Label = (standardKey + "." + key + ".Label").Translate().Resolve();
            Tooltip = info.Prompted
                ? (standardKey + "." + key + ".Tooltip").Translate().Resolve()
                : string.Empty;

            if (info.IsDependencyProperty)
            {
                Property = DependencyProperty.Search(ownerType, Name);
                Value = null;
            }
            else
            {
                Property = null;

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
            }
        }


        private static int Compare(SettingEntry x, SettingEntry y)
        {
            return x.EntryInfo.Order > y.EntryInfo.Order
                ? 1
                : -1;
        }
    }
}
