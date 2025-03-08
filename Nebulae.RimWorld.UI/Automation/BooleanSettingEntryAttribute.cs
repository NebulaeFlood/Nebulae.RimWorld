using System;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记成员为 <see cref="bool"/> 类型的设置条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class BooleanSettingEntryAttribute : SettingEntryBaseAttribute
    {
        /// <summary>
        /// 控件的高度
        /// </summary>
        public float Height = 34f;

        /// <summary>
        /// 初始化 <see cref="BooleanSettingEntryAttribute"/> 的新实例
        /// </summary>
        public BooleanSettingEntryAttribute() : base(SettingEntryType.Boolean)
        {
        }
    }
}
