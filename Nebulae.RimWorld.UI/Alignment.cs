namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 指示控件在分配的空间的水平方向上的对齐方式
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// 与分配空间的左侧对齐
        /// </summary>
        Left,
        /// <summary>
        /// 与分配空间的中央对齐
        /// </summary>
        Center,
        /// <summary>
        /// 与分配空间的右侧对齐
        /// </summary>
        Right,
        /// <summary>
        /// 拉伸以填充分配空间的水平方向
        /// </summary>
        Stretch
    }

    /// <summary>
    /// 指示控件在分配的空间的垂直方向上的对齐方式
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// 与分配空间的顶部对齐
        /// </summary>
        Top,
        /// <summary>
        /// 与分配空间的中部对齐
        /// </summary>
        Center,
        /// <summary>
        /// 与分配空间的底部对齐
        /// </summary>
        Bottom,
        /// <summary>
        /// 拉伸以填充分配空间的垂直方向
        /// </summary>
        Stretch
    }
}
