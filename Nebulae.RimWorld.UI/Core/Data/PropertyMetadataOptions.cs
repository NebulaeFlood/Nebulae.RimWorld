using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// <see cref="PropertyMetadata"/> 用于指定依赖属性行为的选项
    /// </summary>
    [Flags]
    public enum PropertyMetadataOptions : ushort
    {
        /// <summary>
        /// 默认行为
        /// </summary>
        None            = 0b00000,

        /// <summary>
        /// 影响控件排布
        /// </summary>
        AffectsArrange  = 0b00001,

        /// <summary>
        /// 影响控件测量
        /// </summary>
        /// <remarks>影响控件测量将也将影响控件排布。</remarks>
        AffectsMeasure  = 0b00011,

        /// <summary>
        /// 影响控件渲染
        /// </summary>
        AffectsRender   = 0b00100,

        /// <summary>
        /// 子元素继承值
        /// </summary>
        Inherits        = 0b01000,

        /// <summary>
        /// 元数据不可变
        /// </summary>
        /// <remarks>当元数据应用于依赖属性时，将自动设置此选项。不要手动设置此选项。</remarks>
        Sealed          = 0b10000,
    }
}
