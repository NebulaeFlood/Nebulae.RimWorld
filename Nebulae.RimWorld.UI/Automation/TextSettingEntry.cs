using Nebulae.RimWorld.UI.Data;
using System;
using System.Text.RegularExpressions;
using Verse;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记成员为 <see cref="string"/> 类型的设置条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TextSettingEntryAttribute : SettingEntryBaseAttribute
    {
        /// <summary>
        /// 默认的字符串
        /// </summary>
        public string Default;

        /// <summary>
        /// 字体尺寸
        /// </summary>
        public GameFont FontSize = GameFont.Small;

        /// <summary>
        /// 输入框是否为只读状态
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// 是否自动换行
        /// </summary>
        public bool WrapText;


        /// <summary>
        /// 初始化 <see cref="TextSettingEntryAttribute"/> 的新实例
        /// </summary>
        public TextSettingEntryAttribute() : base(SettingEntryType.String)
        {
        }
    }
}
