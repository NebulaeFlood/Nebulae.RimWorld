using Nebulae.RimWorld.UI.Core.Data.Bindings.Converters;
using System;
using System.Globalization;

namespace Nebulae.RimWorld.UI.Controls.Converters
{
    /// <summary>
    /// 将 <see cref="ToggleState"/> 转换为 <see cref="bool"/> 的转换器
    /// </summary>
    /// <remarks>支持双向转换。</remarks>
    public sealed class ToggleStateToBoolean : IValueConverter
    {
        /// <summary>
        /// <see cref="ToggleStateToBoolean"/> 的单例
        /// </summary>
        public static readonly ToggleStateToBoolean Instance = new ToggleStateToBoolean();


        private ToggleStateToBoolean() { }


        /// <inheritdoc/>
        public object Convert(object value, Type targetType, CultureInfo culture)
        {
            if (value is ToggleState status)
            {
                return status > 0;
            }
            else
            {
                return value is true ? ToggleState.On : ToggleState.Off;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, CultureInfo culture)
        {
            if (value is ToggleState status)
            {
                return status > 0;
            }
            else
            {
                return value is true ? ToggleState.On : ToggleState.Off;
            }
        }
    }
}
