namespace Nebulae.RimWorld.UI.Core.Events
{
    /// <summary>
    /// 指示 <see cref="RoutedEvent"/> 的路由策略
    /// </summary>
    public enum RoutingStrategy
    {
        /// <summary>
        /// 隧道
        /// </summary>
        /// <remarks>事件通过树向下路由</remarks>
        Tunnel,

        /// <summary>
        /// 冒泡
        /// </summary>
        /// <remarks>事件通过树向上路由</remarks>
        Bubble,

        /// <summary>
        /// 直接
        /// </summary>
        /// <remarks>事件不进行路由</remarks>
        Direct,

        /// <summary>
        /// 点击
        /// </summary>
        /// <remarks>事件通过控件在光标下的层次向下路由</remarks>
        TopHit
    }
}
