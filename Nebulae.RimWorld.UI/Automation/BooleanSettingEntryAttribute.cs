using Nebulae.RimWorld.UI.Utilities;
using System;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记成员为 <see cref="bool"/> 类型的设置条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BooleanSettingEntryAttribute : SettingEntryBaseAttribute
    {
        /// <summary>
        /// 控件的高度
        /// </summary>
        public float Height = AutoLayoutUtility.StandardRowHeight;

        /// <summary>
        /// 初始化 <see cref="BooleanSettingEntryAttribute"/> 的新实例
        /// </summary>
        public BooleanSettingEntryAttribute() : base(SettingEntryType.Boolean)
        {
        }
    }
}
