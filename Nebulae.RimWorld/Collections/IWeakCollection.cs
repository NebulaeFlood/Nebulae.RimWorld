namespace Nebulae.RimWorld.Collections
{
    /// <summary>
    /// 定义通过弱引用管理对象的集合
    /// </summary>
    public interface IWeakCollection
    {
        /// <summary>
        /// 从集合中移除所有元素
        /// </summary>
        void Clear();

        /// <summary>
        /// 清理已经被 GC 回收的集合元素
        /// </summary>
        void Purge();
    }
}
