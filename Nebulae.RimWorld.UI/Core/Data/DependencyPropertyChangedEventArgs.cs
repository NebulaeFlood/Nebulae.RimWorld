using Verse;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖属性值更改后的事件数据
    /// </summary>
    public sealed class DependencyPropertyChangedEventArgs
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 值被更改的依赖属性
        /// </summary>
        public readonly DependencyProperty Property;

        /// <summary>
        /// 依赖属性的元数据
        /// </summary>
        public readonly PropertyMetadata Metadata;

        /// <summary>
        /// 依赖属性的新值
        /// </summary>
        public object NewValue;

        /// <summary>
        /// 依赖属性的旧值
        /// </summary>
        public object OldValue;

        #endregion


        /// <summary>
        /// 初始化 <see cref="DependencyPropertyChangedEventArgs"/> 的新实例
        /// </summary>
        /// <param name="property">更改的依赖属性</param>
        /// <param name="metadata">依赖属性元数据</param>
        /// <param name="oldValue">旧的依赖属性值</param>
        /// <param name="newValue">新的依赖属性值</param>
        public DependencyPropertyChangedEventArgs(DependencyProperty property, PropertyMetadata metadata, object oldValue, object newValue)
        {
            Property = property;
            Metadata = metadata;

            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
