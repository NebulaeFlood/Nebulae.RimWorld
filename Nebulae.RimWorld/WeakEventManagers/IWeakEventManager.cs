using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.WeakEventManagers
{
    /// <summary>
    /// 定义一个通过订阅者直接引发操作的弱事件管理器
    /// </summary>
    public interface IWeakEventManager
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
