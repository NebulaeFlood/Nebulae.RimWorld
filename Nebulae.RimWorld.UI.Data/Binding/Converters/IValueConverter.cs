using System.Globalization;

namespace Nebulae.RimWorld.UI.Data.Binding.Converters
{
    /// <summary>
    /// 綁定源和目标的成员值的转换器
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// 将源成员的值转化为目标成员的值
        /// </summary>
        /// <param name="value">源成员的值</param>
        /// <param name="culture">地区信息</param>
        /// <returns>目标成员的值。</returns>
        object Convert(object value, CultureInfo culture);

        /// <summary>
        /// 将目标成员的值转化为源成员的值
        /// </summary>
        /// <param name="value">目标成员的值</param>
        /// <param name="culture">地区信息</param>
        /// <returns>源成员的值。</returns>
        object ConvertBack(object value, CultureInfo culture);
    }
}
