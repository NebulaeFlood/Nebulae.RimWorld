using System.Reflection;
using Verse;

namespace Nebulae.RimWorld.UI.Automation
{
    internal struct SettingEntry
    {
        internal bool IsBooleanEntry;
        internal FieldInfo Field;
        internal string Name;
        internal NumberSettingEntryAttribute SliderInfo;

        internal string Label;
        internal string Tooltip;

        internal SettingEntry(
            FieldInfo field,
            string overriddenName,
            bool isBooleanEntry,
            string standardKey,
            bool containTooltip = true,
            NumberSettingEntryAttribute sliderInfo = null)
        {
            Field = field;
            IsBooleanEntry = isBooleanEntry;
            Name = string.IsNullOrEmpty(overriddenName)
                ? field.Name
                : overriddenName;
            SliderInfo = sliderInfo;

            Label = (standardKey + "." + Name + "." + "Label").Translate();

            if (containTooltip)
            {
                Tooltip = (standardKey + "." + Name + "." + "Tooltip").Translate();
            }
            else
            {
                Tooltip = string.Empty;
            }
        }
    }
}
