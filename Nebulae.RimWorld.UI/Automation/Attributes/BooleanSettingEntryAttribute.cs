using System;

namespace Nebulae.RimWorld.UI.Automation.Attributes
{
    /// <summary>
    /// 标记 <see cref="bool"/> 类型的成员为的设置条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BooleanSettingEntryAttribute : SettingEntryBaseAttribute
    {
        /// <summary>
        /// 初始化 <see cref="BooleanSettingEntryAttribute"/> 的新实例
        /// </summary>
        public BooleanSettingEntryAttribute() : base(SettingEntryType.Boolean) { }
    }
}
