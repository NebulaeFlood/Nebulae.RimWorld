using Nebulae.RimWorld.UI.Data.Utilities;
using System.Reflection;
using Verse;

namespace Nebulae.RimWorld.UI.Automation
{
    internal readonly struct SettingEntry
    {
        internal readonly SettingEntryBaseAttribute EntryInfo;
        internal readonly SettingEntryType EntryType;
        internal readonly FieldInfo Field;
        internal readonly string Label;
        internal readonly string Name;
        internal readonly string Tooltip;
        internal readonly object Value;


        internal SettingEntry(string standardKey, SettingEntryBaseAttribute info, FieldInfo field, object settings)
        {
            EntryInfo = info;
            EntryType = info.EntryType;
            Field = field;
            Name = field.Name;

            string key = info.GetLabel(field);

            Label = (standardKey + "." + key + ".Label").Translate().Resolve();
            Tooltip = info.Prompted
                ? (standardKey + "." + key + ".Tooltip").Translate().Resolve()
                : string.Empty;

            switch (EntryType)
            {
                case SettingEntryType.Boolean:
                    Value = field.GetValue<bool>(settings);
                    break;
                case SettingEntryType.Number:
                    Value = field.GetValue<float>(settings);
                    break;
                case SettingEntryType.String:
                    Value = field.GetValue<string>(settings);
                    break;
                default:
                    Value = null;
                    break;
            }
        }
    }
}
