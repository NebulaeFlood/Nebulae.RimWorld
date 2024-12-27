using Nebulae.RimWorld.UI.Data;
using System;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// <see cref="Control"/> 的属性的元数据的特殊标记
    /// </summary>
    [Flags]
    public enum ControlPropertyMetadataFlag : int
    {
        /// <summary>
        /// 无标记
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// 与控件排布关联
        /// </summary>
        Arrange = 0x00000001,
        /// <summary>
        /// 与控件度量关联
        /// </summary>
        Measure = 0x00000002
    }

    /// <summary>
    /// <see cref="Control"/> 的属性的元数据
    /// </summary>
    public class ControlPropertyMetadata : PropertyMetadata
    {
        private readonly ControlPropertyMetadataFlag _flags;

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        public ControlPropertyMetadata() : base()
        {
            _flags = ControlPropertyMetadataFlag.None;
        }

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="flags">属性的特殊标记</param>
        public ControlPropertyMetadata(object defaultValue, ControlPropertyMetadataFlag flags = ControlPropertyMetadataFlag.None) : base(defaultValue)
        {
            _flags = flags;
        }

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        /// <param name="flags">属性的特殊标记</param>
        public ControlPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, ControlPropertyMetadataFlag flags = ControlPropertyMetadataFlag.None) : base(defaultValue, propertyChangedCallback)
        {
            _flags = flags;
        }

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        /// <param name="flags">属性的特殊标记</param>
        public ControlPropertyMetadata(object defaultValue, CoerceValueCallback coerceValueCallback, ControlPropertyMetadataFlag flags = ControlPropertyMetadataFlag.None) : base(defaultValue, coerceValueCallback)
        {
            _flags = flags;
        }

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        /// <param name="flags">属性的特殊标记</param>
        public ControlPropertyMetadata(object defaultValue, CoerceValueCallback coerceValueCallback, PropertyChangedCallback propertyChangedCallback, ControlPropertyMetadataFlag flags = ControlPropertyMetadataFlag.None) : base(defaultValue, coerceValueCallback, propertyChangedCallback)
        {
            _flags = flags;
        }

        #endregion


        /// <inheritdoc/>
        protected override void OnProertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (_flags.HasFlag(ControlPropertyMetadataFlag.Measure))
            {
                ((Control)obj).InvalidateMeasure();
            }
            else if (_flags.HasFlag(ControlPropertyMetadataFlag.Arrange))
            {
                ((Control)obj).InvalidateArrange();
            }
        }
    }
}
