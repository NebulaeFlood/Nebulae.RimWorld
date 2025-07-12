using System;

namespace Nebulae.RimWorld.UI.Automation.Diagnostics
{
    /// <summary>
    /// 标记属性或字段为 Debug 成员
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DebugMemberAttribute : Attribute
    {
        /// <summary>
        /// 成员的优先级
        /// </summary>
        public int Priority;

        /// <summary>
        /// 成员的别名
        /// </summary>
        public string Name = string.Empty;


        /// <summary>
        /// 初始化 <see cref="DebugMemberAttribute"/> 的新实例
        /// </summary>
        /// <param name="priority">成员的优先级</param>
        public DebugMemberAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
