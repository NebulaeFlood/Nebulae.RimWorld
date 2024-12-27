namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 表明 <see cref="BindingBase{TSource, TTarget}"/> 中源和目标的关系
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// 源和目标将会同步绑定的成员的值
        /// </summary>
        /// <remarks>需要确保源属性和目标属性同时拥有 <see langword="get"/> 和 <see langword="set"/> 访问器。</remarks>
        TwoWay,
        /// <summary>
        /// 源的指定成员的值变化时，同步目标绑定的成员的值
        /// </summary>
        /// <remarks>需要确保源属性拥有 <see langword="get"/> 访问器且目标属性拥有 <see langword="get"/> 和 <see langword="set"/> 访问器。</remarks>
        OneWay
    }
}
