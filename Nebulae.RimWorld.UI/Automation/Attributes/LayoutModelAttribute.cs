using System;

namespace Nebulae.RimWorld.UI.Automation.Attributes
{
    /// <summary>
    /// 标记类型为布局模板
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class LayoutModelAttribute : Attribute
    {
        /// <summary>
        /// 类型成员的翻译键前缀
        /// </summary>
        public readonly string TraslationKeyPrefix;

        /// <summary>
        /// 初始化 <see cref="LayoutModelAttribute"/> 的新实例
        /// </summary>
        /// <param name="key">类型成员的翻译键前缀</param>
        public LayoutModelAttribute(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            TraslationKeyPrefix = key;
        }
    }
}
