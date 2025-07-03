namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 表示用于验证要设置给依赖属性的值的方法
    /// </summary>
    /// <param name="value">要验证的值</param>
    /// <returns>如果值对于依赖属性有效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
    public delegate bool ValidateValueCallback(object value);
}
