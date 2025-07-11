using System;
using System.Reflection;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 定义一个弱事件处理程序
    /// </summary>
    /// <typeparam name="TSender">处理器的 sender 参数类型</typeparam>
    /// <typeparam name="TArgs">处理器的 args 参数类型</typeparam>
    public interface IWeakEventHandler<TSender, TArgs> : IEquatable<Delegate>, IEquatable<MethodInfo> where TArgs : EventArgs
    {
        /// <summary>
        /// 获取一个值，该值指示当前事件处理程序是否应该被回收
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// 调用事件处理程序
        /// </summary>
        /// <param name="sender">发起事件的对象</param>
        /// <param name="args">事件数据</param>
        void Invoke(TSender sender, TArgs args);
    }
}
