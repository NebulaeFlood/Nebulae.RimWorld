using System;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 定义一个尺寸
    /// </summary>
    public abstract class Definition
    {
        /// <summary>
        /// 自动尺寸
        /// </summary>
        public const float Auto = float.NaN;


        /// <summary>
        /// 尺寸的索引
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// 是否为自动尺寸
        /// </summary>
        public readonly bool IsAutoSize;

        /// <summary>
        /// 逻辑尺寸
        /// </summary>
        public readonly float LogicalSize;

        /// <summary>
        /// 尺寸
        /// </summary>
        public float Size;


        /// <summary>
        /// 为 <see cref="Definition"/> 派生类实现基本初始化
        /// </summary>
        protected Definition(float logicalSize, int index)
        {
            if (logicalSize < 0f)
            {
                throw new ArgumentException($"{typeof(Definition)}.{nameof(LogicalSize)} cannot be negative.");
            }

            Index = index;
            IsAutoSize = float.IsNaN(logicalSize);
            LogicalSize = logicalSize;
            Size = 0f;
        }
    }
}
