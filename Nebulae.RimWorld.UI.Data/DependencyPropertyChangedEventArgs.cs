namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 依赖属性的值更改后的事件数据
    /// </summary>
    public struct DependencyPropertyChangedEventArgs
    {
        private readonly EffectiveValueEntry _newEntry;
        private readonly EffectiveValueEntry _oldEntry;

        private readonly PropertyMetadata _metadata;
        private readonly bool _overriddenDefault;


        /// <summary>
        /// 属性的元数据
        /// </summary>
        public PropertyMetadata Metadata => _metadata;
        /// <summary>
        /// 属性的新值
        /// </summary>
        public object NewValue => _newEntry.EffectiveValue;

        /// <summary>
        /// 属性的旧值
        /// </summary>
        public object OldValue => _overriddenDefault ? _metadata.DefaultValue : _oldEntry.EffectiveValue;

        /// <summary>
        /// 值被更改的属性
        /// </summary>
        public DependencyProperty Property { get; }


        /// <summary>
        /// 初始化 <see cref="DependencyPropertyChangedEventArgs"/> 的新实例
        /// </summary>
        /// <param name="property">更改的属性</param>
        /// <param name="metadata">属性元数据</param>
        /// <param name="newEntry">新的有效项</param>
        internal DependencyPropertyChangedEventArgs(DependencyProperty property, PropertyMetadata metadata, EffectiveValueEntry newEntry)
        {
            Property = property;

            _metadata = metadata;

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
            Property = property;

            _metadata = metadata;

            _oldEntry = oldEntry;
            _newEntry = newEntry;

            _overriddenDefault = false;
        }
    }
}
