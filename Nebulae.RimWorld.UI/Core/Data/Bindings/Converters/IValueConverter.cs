using System;
using System.Globalization;

namespace Nebulae.RimWorld.UI.Core.Data.Bindings.Converters
{
    /// <summary>
    /// 綁定源和目标的成员值转换器
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// 将源成员的值转化为目标成员的值
        /// </summary>
        /// <param name="value">源成员的值</param>
        /// <param name="targetType">要转换为的类型</param>
        /// <param name="culture">地区信息</param>
        /// <returns>转换后的值。</returns>
        object Convert(object value, Type targetType, CultureInfo culture);

        /// <summary>
        /// 将目标成员的值转化为源成员的值
        /// </summary>
        /// <param name="value">目标成员的值</param>
        /// <param name="targetType">要转换为的类型</param>
        /// <param name="culture">地区信息</param>
        /// <returns>转换后的值。</returns>
        object ConvertBack(object value, Type targetType, CultureInfo culture);
    }
}
