using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 属性的有效值
    /// </summary>
    [DebuggerStepThrough]
    internal readonly struct EffectiveValueEntry
    {
        /// <summary>
        /// 初始化 <see cref="EffectiveValueEntry"/> 的新实例
        /// </summary>
        /// <param name="value">属性的值</param>
        internal EffectiveValueEntry(object value)
        {
            IsTemporary = false;
            TemporaryValue = value;
            Value = value;
        }

        /// <summary>
        /// 初始化 <see cref="EffectiveValueEntry"/> 的新实例
        /// </summary>
        /// <param name="value">属性的值</param>
        /// <param name="tmpraryValue">属性的临时值</param>
        internal EffectiveValueEntry(object value, object tmpraryValue)
        {
            IsTemporary = true;
            TemporaryValue = tmpraryValue;
            Value = value;
        }


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        /// <summary>
        /// 是否为临时有效值
        /// </summary>
        internal readonly bool IsTemporary;

        /// <summary>
        /// 属性的临时值
        /// </summary>
        internal readonly object TemporaryValue;

        /// <summary>
        /// 属性的值
        /// </summary>
        internal readonly object Value;

        #endregion
    }
}
