namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 表示设置给依赖属性的值需要转换类型时调用的方法
    /// </summary>
    /// <param name="d">属性值将要更改的依赖对象</param>
    /// <param name="baseValue">将要设置的属性值</param>
    /// <returns>强制转换后的值。</returns>
    public delegate object CoerceValueCallback(DependencyObject d, object baseValue);
}
