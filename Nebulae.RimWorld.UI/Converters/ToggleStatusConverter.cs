using Nebulae.RimWorld.UI.Data.Binding;

namespace Nebulae.RimWorld.UI.Converters
{
    /// <summary>
    /// 复选框状态的值转换器
    /// </summary>
    public class ToggleStatusConverter : IValueConverter<ToggleStatus, bool>
    {
        /// <inheritdoc/>
        public bool Convert(ToggleStatus value)
        {
            return value is ToggleStatus.Checked || value is ToggleStatus.Indeterminate;
        }

        /// <inheritdoc/>
        public ToggleStatus ConvertBack(bool value)
        {
            return value ? ToggleStatus.Checked : ToggleStatus.Unchecked;
        }
    }
}
