namespace Nebulae.RimWorld.UI.Core.Data.Bindings
{
    /// <summary>
    /// <see cref="BindingBase"/> 中绑定成员之间的关系
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// 源和目标将会同步绑定的成员的值
        /// </summary>
        /// <remarks>需要确保源成员和目标成员的值皆可被获取或设置。</remarks>
        TwoWay,

        /// <summary>
        /// 源的指定成员的值变化时，同步目标绑定的成员的值
        /// </summary>
        /// <remarks>需要确保源成员的值可被获取，且目标成员的值可被设置。</remarks>
        OneWay
    }
}