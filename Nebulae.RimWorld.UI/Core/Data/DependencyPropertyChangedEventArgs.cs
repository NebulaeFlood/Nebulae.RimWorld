using System;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖属性的值更改后的事件数据
    /// </summary>
    public sealed class DependencyPropertyChangedEventArgs : EventArgs
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 属性的元数据
        /// </summary>
        public readonly PropertyMetadata Metadata;

        /// <summary>
        /// 值被更改的属性
        /// </summary>
        public readonly DependencyProperty Property;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 属性的新值
        /// </summary>
        public object NewValue => NewEntry.IsTemporary ? NewEntry.TemporaryValue : NewEntry.Value;

        /// <summary>
        /// 属性的旧值
        /// </summary>
        public object OldValue => OldEntry.IsTemporary ? OldEntry.TemporaryValue : OldEntry.Value;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="DependencyPropertyChangedEventArgs"/> 的新实例
        /// </summary>
        /// <param name="property">更改的属性</param>
        /// <param name="metadata">属性元数据</param>
        /// <param name="newEntry">新的有效项</param>
        internal DependencyPropertyChangedEventArgs(DependencyProperty property, PropertyMetadata metadata, EffectiveValueEntry newEntry)
        {
            Metadata = metadata;
            Property = property;

            OldEntry = new EffectiveValueEntry(metadata.DefaultValue);
            NewEntry = newEntry;
        }

        /// <summary>
        /// 初始化 <see cref="DependencyPropertyChangedEventArgs"/> 的新实例
        /// </summary>
        /// <param name="property">更改的属性</param>
        /// <param name="metadata">属性元数据</param>
        /// <param name="oldEntry">旧的有效项</param>
        /// <param name="newEntry">新的有效项</param>
        internal DependencyPropertyChangedEventArgs(DependencyProperty property, PropertyMetadata metadata, EffectiveValueEntry oldEntry, EffectiveValueEntry newEntry)
        {
            Metadata = metadata;
            Property = property;

            OldEntry = oldEntry;
            NewEntry = newEntry;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        internal readonly EffectiveValueEntry NewEntry;
        internal readonly EffectiveValueEntry OldEntry;

        #endregion
    }
}
