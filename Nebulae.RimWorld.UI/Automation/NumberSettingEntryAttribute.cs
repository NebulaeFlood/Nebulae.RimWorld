using System;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记成员为数值类型的设置条目
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public class NumberSettingEntryAttribute : Attribute
    {
        /// <summary>
        /// 小数部分的位数
        /// </summary>
        public int DecimalPartDigit = 0;

        /// <summary>
        /// 以百分数形式显示数值
        /// </summary>
        public bool DisplayAsPercent;

        /// <summary>
        /// 最大值
        /// </summary>
        public readonly float MaxValue;

        /// <summary>
        /// 最小值
        /// </summary>
        public readonly float MinValue;

        /// <summary>
        /// 条目名称
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// 最小的相邻可选值的差值
        /// </summary>
        public float SilderStep = 1f;


        /// <summary>
        /// 初始化 <see cref="NumberSettingEntryAttribute"/> 的新实例
        /// </summary>
        /// <param name="min">条目的最小值</param>
        /// <param name="max">条目的最大值</param>
        public NumberSettingEntryAttribute(float min, float max)
        {
            MinValue = min;
            MaxValue = max;
        }
    }
}
