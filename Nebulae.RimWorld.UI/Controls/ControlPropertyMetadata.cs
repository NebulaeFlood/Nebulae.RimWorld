using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 属性与控件布局的关系
    /// </summary>
    public enum ControlRelation : byte
    {
        /// <summary>
        /// 无标记
        /// </summary>
        None = 0b00,
        /// <summary>
        /// 与控件排布关联
        /// </summary>
        Arrange = 0b01,
        /// <summary>
        /// 与控件度量关联
        /// </summary>
        Measure = 0b11
    }

    /// <summary>
    /// <see cref="Control"/> 的依赖属性的元数据
    /// </summary>
    public class ControlPropertyMetadata : PropertyMetadata
    {
        /// <summary>
        /// 依赖属性与控件布局的关系
        /// </summary>
        public readonly ControlRelation Relation;


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
            Relation = ControlRelation.None;
        }

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="flag">依赖属性与控件的关系</param>
        public ControlPropertyMetadata(object defaultValue, ControlRelation flag = ControlRelation.None)
            : base(defaultValue)
        {
            Relation = flag;
        }

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        /// <param name="flags">属性的特殊标记</param>
        public ControlPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, ControlRelation flags = ControlRelation.None)
            : base(defaultValue, propertyChangedCallback)
        {
            Relation = flags;
        }

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        /// <param name="flags">属性的特殊标记</param>
        public ControlPropertyMetadata(object defaultValue, CoerceValueCallback coerceValueCallback, ControlRelation flags = ControlRelation.None)
            : base(defaultValue, coerceValueCallback)
        {
            Relation = flags;
        }

        /// <summary>
        /// 初始化 <see cref="ControlPropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        /// <param name="flags">属性的特殊标记</param>
        public ControlPropertyMetadata(object defaultValue, CoerceValueCallback coerceValueCallback, PropertyChangedCallback propertyChangedCallback, ControlRelation flags = ControlRelation.None)
            : base(defaultValue, coerceValueCallback, propertyChangedCallback)
        {
            Relation = flags;
        }

        #endregion
    }
}
