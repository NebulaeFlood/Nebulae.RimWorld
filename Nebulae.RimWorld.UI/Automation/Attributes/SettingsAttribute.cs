using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Automation.Attributes
{
    /// <summary>
    /// 标记类型为设置内容类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingsAttribute : Attribute
    {
        /// <summary>
        /// 类型成员的翻译键前缀
        /// </summary>
        public readonly string TraslationKeyPrefix;

        /// <summary>
        /// 初始化 <see cref="SettingsAttribute"/> 的新实例
        /// </summary>
        /// <param name="key">类型成员的翻译键前缀</param>
        public SettingsAttribute(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            TraslationKeyPrefix = key;
        }
    }
}
