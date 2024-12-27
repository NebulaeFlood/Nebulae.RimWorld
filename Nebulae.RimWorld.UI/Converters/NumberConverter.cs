using Nebulae.RimWorld.UI.Data.Binding;

namespace Nebulae.RimWorld.UI.Converters
{
    /// <summary>
    /// 浮点数的值转换器
    /// </summary>
    public class FloatConverter : IValueConverter<float, int>
    {
        /// <inheritdoc/>
        public int Convert(float value)
        {
            return (int)value;
        }

        /// <inheritdoc/>
        public float ConvertBack(int value)
        {
            return value;
        }
    }

    /// <summary>
    /// 32 位整形的值转换器
    /// </summary>
    public class Int32Converter : IValueConverter<int, float>
    {
        /// <inheritdoc/>
        public float Convert(int value)
        {
            return value;
        }

        /// <inheritdoc/>
        public int ConvertBack(float value)
        {
            return (int)value;
        }
    }
}
