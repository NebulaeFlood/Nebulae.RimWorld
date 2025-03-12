using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Data.Binding.Converters;
using System.Globalization;

namespace Nebulae.RimWorld.UI.Converters
{
    /// <summary>
    /// <see cref="Visibility"/> 和 <see cref="ToggleStatus"/> 之间的转换器
    /// </summary>
    public sealed class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 该转换器的实例
        /// </summary>
        public static readonly VisibilityConverter Instance = new VisibilityConverter();


        /// <inheritdoc/>
        public object Convert(object value, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return (visibility is Visibility.Visible)
                    ? ToggleStatus.Checked
                    : ToggleStatus.Unchecked;
            }
            else
            {
                return (value is ToggleStatus.Checked)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return (visibility is Visibility.Visible)
                    ? ToggleStatus.Checked
                    : ToggleStatus.Unchecked;
            }
            else
            {
                return (value is ToggleStatus.Checked)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}
