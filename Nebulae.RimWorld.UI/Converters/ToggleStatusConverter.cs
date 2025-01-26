using Nebulae.RimWorld.UI.Data.Binding.Converters;
using System.Globalization;

namespace Nebulae.RimWorld.UI.Converters
{
    /// <summary>
    /// <see cref="ToggleStatus"/> 和 <see cref="bool"/> 之间的转换器
    /// </summary>
    public class ToggleStatusConverter : IValueConverter
    {
        /// <summary>
        /// 该转换器的实例
        /// </summary>
        public static readonly ToggleStatusConverter Instance = new ToggleStatusConverter();


        /// <inheritdoc/>
        public object Convert(object value, CultureInfo culture)
        {
            if (value is ToggleStatus status)
            {
                return status > 0;
            }
            else
            {
                return value is true
                    ? ToggleStatus.Checked
                    : ToggleStatus.Unchecked;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, CultureInfo culture)
        {
            if (value is ToggleStatus status)
            {
                return status > 0;
            }
            else
            {
                return value is true
                    ? ToggleStatus.Checked
                    : ToggleStatus.Unchecked;
            }
        }
    }
}
