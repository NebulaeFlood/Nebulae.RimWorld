using Nebulae.RimWorld.UI.Core.Data.Bindings.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Controls.Converters
{
    /// <summary>
    /// 将 <see cref="ToggleState"/> 转换为 <see cref="Visibility"/> 的转换器
    /// </summary>
    /// <remarks>支持双向转换。</remarks>
    public sealed class ToggleStateToVisibility : IValueConverter
    {
        /// <summary>
        /// <see cref="ToggleStateToVisibility"/> 的单例
        /// </summary>
        public static readonly ToggleStateToVisibility Instance = new ToggleStateToVisibility();


        /// <inheritdoc/>
        public object Convert(object value, Type targetType, CultureInfo culture)
        {
            if (value is ToggleState toggleState)
            {
                return toggleState is ToggleState.On ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return value is Visibility.Visible ? ToggleState.On : ToggleState.Off;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, CultureInfo culture)
        {
            if (value is ToggleState toggleState)
            {
                return toggleState is ToggleState.On ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return value is Visibility.Visible ? ToggleState.On : ToggleState.Off;
            }
        }
    }
}
