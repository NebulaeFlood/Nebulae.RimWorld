using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖属性值来源的优先级
    /// </summary>
    public enum ValuePrecedence : byte
    {
        /// <summary>
        /// 来源未知
        /// </summary>
        Unknown,

        /// <summary>
        /// 来自属性的默认值
        /// </summary>
        Default,

        /// <summary>
        /// 来自父级的值
        /// </summary>
        Inherited,

        /// <summary>
        /// 来自本地设置的值
        /// </summary>
        Local,

        /// <summary>
        /// 来自缓存的值
        /// </summary>
        Cache,

        /// <summary>
        /// 来自动画的值
        /// </summary>
        Animation,

        /// <summary>
        /// 来自 <see cref="PropertyMetadata.CoerceValueCallback"/> 的强制值
        /// </summary>
        Coercion
    }
}
