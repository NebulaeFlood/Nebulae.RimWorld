﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 有效值条目
    /// </summary>
    internal struct EffectiveValueEntry : IEquatable<EffectiveValueEntry>
    {
        /// <summary>
        /// 是否正在临时修改属性值
        /// </summary>
        internal bool IsModifying;

        /// <summary>
        /// 属性的值
        /// </summary>
        internal object Value;

        /// <summary>
        /// 属性的临时值
        /// </summary>
        internal object TempraryValue;

        /// <summary>
        /// 属性的有效值
        /// </summary>
        internal object EffectiveValue => IsModifying ? TempraryValue : Value;


        /// <summary>
        /// 初始化 <see cref="EffectiveValueEntry"/> 的新实例
        /// </summary>
        /// <param name="value">属性的值</param>
        internal EffectiveValueEntry(object value)
        {
            IsModifying = false;
            TempraryValue = value;
            Value = value;
        }

        /// <summary>
        /// 初始化 <see cref="EffectiveValueEntry"/> 的新实例
        /// </summary>
        /// <param name="value">属性的值</param>
        /// <param name="tmpraryValue">属性的临时值</param>
        internal EffectiveValueEntry(object value, object tmpraryValue)
        {
            IsModifying = true;
            TempraryValue = tmpraryValue;
            Value = value;
        }


        public bool Equals(EffectiveValueEntry other)
        {
            return IsModifying == other.IsModifying && Value == other.Value && TempraryValue == other.TempraryValue;
        }


        public override bool Equals(object obj)
        {
            return obj is EffectiveValueEntry entry && Equals(entry);
        }
    }
}
