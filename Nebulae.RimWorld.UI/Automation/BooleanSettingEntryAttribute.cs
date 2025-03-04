using System;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记成员为 <see cref="bool"/> 类型的设置条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class BooleanSettingEntryAttribute : Attribute
    {
        /// <summary>
        /// 条目名称
        /// </summary>
        public string Name = string.Empty;
    }
}
