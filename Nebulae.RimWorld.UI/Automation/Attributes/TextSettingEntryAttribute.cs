using System;
using Verse;

namespace Nebulae.RimWorld.UI.Automation.Attributes
{
    /// <summary>
    /// 标记 <see cref="string"/> 类型的成员为设置条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TextSettingEntryAttribute : SettingEntryBaseAttribute
    {
        /// <summary>
        /// 字体尺寸
        /// </summary>
        public GameFont FontSize = GameFont.Small;

        /// <summary>
        /// 输入框是否为只读状态
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// 输入框是否自动换行
        /// </summary>
        public bool WrapText;


        /// <summary>
        /// 初始化 <see cref="TextSettingEntryAttribute"/> 的新实例
        /// </summary>
        public TextSettingEntryAttribute() : base(SettingEntryType.String) { }
    }
}
