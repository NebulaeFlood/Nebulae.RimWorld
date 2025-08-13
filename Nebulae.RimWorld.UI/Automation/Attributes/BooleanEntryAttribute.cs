using System;

namespace Nebulae.RimWorld.UI.Automation.Attributes
{
    /// <summary>
    /// 标记 <see cref="bool"/> 类型的成员为布局条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class BooleanEntryAttribute : LayoutEntryBaseAttribute
    {
        /// <summary>
        /// 初始化 <see cref="BooleanEntryAttribute"/> 的新实例
        /// </summary>
        public BooleanEntryAttribute() : base(LayoutEntryType.Boolean) { }
    }
}
