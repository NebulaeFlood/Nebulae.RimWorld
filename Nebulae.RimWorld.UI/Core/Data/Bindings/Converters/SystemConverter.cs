using System;
using System.Globalization;

namespace Nebulae.RimWorld.UI.Core.Data.Bindings.Converters
{
    internal sealed class SystemConverter : IValueConverter
    {
        internal static readonly SystemConverter Instance = new SystemConverter();


        public object Convert(object value, Type targetType, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, targetType, culture);
        }

        public object ConvertBack(object value, Type targetType, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, targetType, culture);
        }
    }
}
