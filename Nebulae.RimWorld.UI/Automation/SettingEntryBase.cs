using Nebulae.RimWorld.UI.Data.Binding;
using System;
using System.Reflection;

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
        /// 条目绑定的是否为依赖属性
        /// </summary>
        /// <remarks>将对依赖属性使用 <see cref="BindingMode.TwoWay"/> 模式的绑定关系。</remarks>
        public bool IsDependencyProperty;

        /// <summary>
        /// 条目名称
        /// </summary>
        public string Label = string.Empty;

        /// <summary>
        /// 条目次序
        /// </summary>
        public int Order = 0;

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
        /// <param name="member">条目关联的字段的信息</param>
        /// <returns>条目将显示的标签文字。</returns>
        public string GetLabel(MemberInfo member)
        {
            return string.IsNullOrEmpty(Label) ? member.Name : Label;
        }
    }
}
