using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记成员为设置条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public abstract class SettingEntryBaseAttribute : Attribute
    {
        /// <summary>
        /// 该设置条目的类型
        /// </summary>
        public readonly SettingEntryType EntryType;

        /// <summary>
        /// 条目名称
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// 条目是否拥有提示框
        /// </summary>
        public bool Prompted = true;


        /// <summary>
        /// 为 <see cref="SettingEntryBaseAttribute"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="entryType">该设置条目的类型</param>
        protected SettingEntryBaseAttribute(SettingEntryType entryType)
        {
            EntryType = entryType;
        }


        /// <summary>
        /// 获取条目的标签文字
        /// </summary>
        /// <param name="field">条目关联的字段的信息</param>
        /// <returns>条目将显示的标签文字。</returns>
        public string GetLabel(FieldInfo field)
        {
            return string.IsNullOrEmpty(Name) ? field.Name : Name;
        }
    }
}
