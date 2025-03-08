using System;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记设置类拥有统一的翻译 Key
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StandardTranslationKeyAttribute : Attribute
    {
        /// <summary>
        /// 统一的翻译键
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// 初始化 <see cref="StandardTranslationKeyAttribute"/> 的新实例
        /// </summary>
        /// <param name="key">翻译键</param>
        public StandardTranslationKeyAttribute(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            Key = key;
        }
    }
}
