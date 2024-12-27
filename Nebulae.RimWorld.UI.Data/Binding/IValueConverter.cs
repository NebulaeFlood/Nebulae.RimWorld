namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 綁定源和目标的成员值的转换器
    /// </summary>
    /// <typeparam name="TSource">绑定源成员值的类型</typeparam>
    /// <typeparam name="TTarget">绑定目标成员值的类型</typeparam>
    public interface IValueConverter<TSource, TTarget>
    {
        /// <summary>
        /// 将源成员的值转化为目标成员的值
        /// </summary>
        /// <param name="value">源成员的值</param>
        /// <returns>目标成员的值。</returns>
        TTarget Convert(TSource value);

        /// <summary>
        /// 将目标成员的值转化为源成员的值
        /// </summary>
        /// <param name="value">目标成员的值</param>
        /// <returns>源成员的值。</returns>
        TSource ConvertBack(TTarget value);
    }
}
