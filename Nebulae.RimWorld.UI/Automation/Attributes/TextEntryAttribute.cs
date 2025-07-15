using System;
using Verse;

namespace Nebulae.RimWorld.UI.Automation.Attributes
{
    /// <summary>
    /// 标记 <see cref="string"/> 类型的成员为布局条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TextEntryAttribute : LayoutEntryBaseAttribute
    {
        /// <summary>
        /// 字体尺寸
        /// </summary>
        public GameFont FontSize = GameFont.Small;

        /// <summary>
        /// 输入框是否自动换行
        /// </summary>
        public bool WrapText;


        /// <summary>
        /// 初始化 <see cref="TextEntryAttribute"/> 的新实例
        /// </summary>
        public TextEntryAttribute() : base(LayoutEntryType.String) { }
    }
}
