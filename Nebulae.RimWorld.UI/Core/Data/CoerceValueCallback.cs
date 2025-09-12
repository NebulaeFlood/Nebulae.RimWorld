namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 表示处理设置给依赖属性的值时调用的方法
    /// </summary>
    /// <param name="d">包含依赖属性的对象</param>
    /// <param name="value">将要设置的值</param>
    /// <returns>由 <paramref name="value"/> 处理而来的值。</returns>
    public delegate object CoerceValueCallback(DependencyObject d, object value);
}
