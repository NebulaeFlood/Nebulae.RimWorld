using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 可通知 Mod 设置被改变的 Mod 设置类
    /// </summary>
    public abstract class NotifiableModSettings : ModSettings
    {
        internal readonly WeakEvent<Mod, ModSettings> updated;

        /// <summary>
        /// 当 Mod 设置更新时触发的弱事件
        /// </summary>
        public event WeakEventHandler<Mod, ModSettings> Updated
        {
            add => updated.Add(value);
            remove => updated.Remove(value);
        }
    }
}
