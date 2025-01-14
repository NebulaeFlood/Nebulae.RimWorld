namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 有效值条目
    /// </summary>
    internal struct EffectiveValueEntry
    {
        /// <summary>
        /// 是否正在临时修改属性值
        /// </summary>
        internal bool IsTemporary;

        /// <summary>
        /// 属性的临时值
        /// </summary>
        internal object TemporaryValue;

        /// <summary>
        /// 属性的值
        /// </summary>
        internal object Value;

        /// <summary>
        /// 属性的有效值
        /// </summary>
        internal object EffectiveValue
        {
            get => IsTemporary ? TemporaryValue : Value;
            set
            {
                if (IsTemporary)
                {
                    TemporaryValue = value;
                }
                else
                {
                    Value = value;
                }
            }
        }


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
    }
}
