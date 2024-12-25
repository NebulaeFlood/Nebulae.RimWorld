namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 依赖属性帮助类
    /// </summary>
    public static class DependencyPropertyHelper
    {
        /// <summary>
        /// 判断指定的值是否为不合理的值
        /// </summary>
        /// <param name="value">要判断的对象</param>
        /// <returns>如果 <paramref name="value"/> 为 <see cref="DependencyProperty.UnsetValue"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool IsUnsetValue(this object value) => ReferenceEquals(value, DependencyProperty.UnsetValue);
    }
}
