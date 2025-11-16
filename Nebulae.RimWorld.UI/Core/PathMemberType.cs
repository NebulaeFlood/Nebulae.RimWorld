using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core
{
    /// <summary>
    /// <see cref="PathMemberInfo"/> 的种类
    /// </summary>
    public enum PathMemberType : byte
    {
        /// <summary>
        /// 节点是依赖属性
        /// </summary>
        DependencyProperty,

        /// <summary>
        /// 节点是索引器
        /// </summary>
        Indexer,

        /// <summary>
        /// 节点是字段
        /// </summary>
        Field,

        /// <summary>
        /// 节点是属性
        /// </summary>
        Property
    }
}
