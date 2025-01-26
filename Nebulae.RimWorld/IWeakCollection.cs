namespace Nebulae.RimWorld
{
    /// <summary>
    /// 定义一个通过弱引用管理对象的集合
    /// </summary>
    public interface IWeakCollection
    {
        /// <summary>
        /// 取消所有管理
        /// </summary>
        void Clear();

        /// <summary>
        /// 清理已经被回收的被管理对象
        /// </summary>
        void Purge();
    }
}
