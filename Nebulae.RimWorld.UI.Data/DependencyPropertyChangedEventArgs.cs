﻿namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 依赖属性的值更改后的事件数据
    /// </summary>
    public struct DependencyPropertyChangedEventArgs
    {
        private readonly EffectiveValueEntry _newEntry;
        private readonly EffectiveValueEntry _oldEntry;

        private readonly bool _overriddenDefault;


        /// <summary>
        /// 属性的元数据
        /// </summary>
        public readonly PropertyMetadata Metadata;

        /// <summary>
        /// 值被更改的属性
        /// </summary>
        public readonly DependencyProperty Property;

        /// <summary>
        /// 属性的新值
        /// </summary>
        public object NewValue => _newEntry.EffectiveValue;

        /// <summary>
        /// 属性的旧值
        /// </summary>
        public object OldValue => _overriddenDefault ? Metadata.DefaultValue : _oldEntry.EffectiveValue;


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

            _oldEntry = default;
            _newEntry = newEntry;

            _overriddenDefault = true;
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

            _oldEntry = oldEntry;
            _newEntry = newEntry;

            _overriddenDefault = false;
        }
    }
}
