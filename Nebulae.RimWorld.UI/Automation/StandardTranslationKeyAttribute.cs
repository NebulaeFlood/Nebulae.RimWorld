using System;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记设置类拥有统一的翻译 Key
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class StandardTranslationKeyAttribute : Attribute
    {
        /// <summary>
        /// 所有条目带有提示框
        /// </summary>
        public readonly bool AllContainTooltip;

        /// <summary>
        /// 统一的翻译键
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// 初始化 <see cref="StandardTranslationKeyAttribute"/> 的新实例
        /// </summary>
        /// <param name="key">翻译键</param>
        /// <param name="allContainTooltip">所有条目拥有提示框</param>
        public StandardTranslationKeyAttribute(string key, bool allContainTooltip = true)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            AllContainTooltip = allContainTooltip;
            Key = key;
        }
    }
}
