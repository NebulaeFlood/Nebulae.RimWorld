using System;
using System.Globalization;

namespace Nebulae.RimWorld.UI.Data.Binding.Converters
{
    /// <summary>
    /// 支持 <see cref="System.Convert"/> 能够转换的类型的值转换器
    /// </summary>
    internal class SystemConverter : IValueConverter
    {
        private readonly Type _sourceType;
        private readonly Type _targetType;

        internal SystemConverter(Type sourceType, Type targetType)
        {
            _sourceType = sourceType;
            _targetType = targetType;
        }

        public object Convert(object value, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, _targetType, culture);
        }

        public object ConvertBack(object value, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, _sourceType, culture);
        }
    }
}
