using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记属性或字段为 Debug 信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DebugInfoEntryAttribute : Attribute
    {
        /// <summary>
        /// 条目的优先级
        /// </summary>
        public int Priority = int.MaxValue;

        /// <summary>
        /// 条目的别名
        /// </summary>
        public string Name = string.Empty;
    }
}
