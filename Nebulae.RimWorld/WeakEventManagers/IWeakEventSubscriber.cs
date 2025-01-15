using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.WeakEventManagers
{
#if DEBUG

    /// <summary>
    /// 表示一个订阅了由 <see cref="WeakEventManager{TSender, TArgs}"/> 管理的事件的对象
    /// </summary>
    /// <typeparam name="TSender">事件源类型</typeparam>
    /// <typeparam name="TArgs">事件参数类型</typeparam>
    public interface IWeakEventSubscriber<TSender, TArgs>
    {
        /// <summary>
        /// 事件被触发时执行的操作
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="args">事件数据</param>
        void OnEventActivated(TSender sender, TArgs args);
    }

#endif
}
