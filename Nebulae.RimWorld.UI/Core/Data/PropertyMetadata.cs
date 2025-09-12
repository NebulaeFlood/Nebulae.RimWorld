using Nebulae.RimWorld.Utilities;
using System;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖属性的元数据
    /// </summary>
    public class PropertyMetadata
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置依赖属性的默认值
        /// </summary>
        public object DefaultValue
        {
            get => _defaultValue;
            set
            {
                if ((_options & PropertyMetadataOptions.Sealed) != 0)
                {
                    throw new InvalidOperationException("Cannot modify metadata after it has been used by a dependency property.");
                }

                _defaultValue = value;
            }
        }

        /// <summary>
        /// 获取或设置处理设置给依赖属性的值的回调函数
        /// </summary>
        public CoerceValueCallback CoerceValueCallback
        {
            get => _coerceValueCallback;
            set
            {
                if ((_options & PropertyMetadataOptions.Sealed) != 0)
                {
                    throw new InvalidOperationException("Cannot modify metadata after it has been used by a dependency property.");
                }

                _coerceValueCallback = value;
            }
        }

        /// <summary>
        /// 获取或设置依赖属性有效值更改时的回调函数
        /// </summary>
        public PropertyChangedCallback PropertyChangedCallback
        {
            get => _propertyChangedCallback;
            set
            {
                if ((_options & PropertyMetadataOptions.Sealed) != 0)
                {
                    throw new InvalidOperationException("Cannot modify metadata after it has been applied to a dependency property.");
                }

                _propertyChangedCallback = value;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示依赖属性是否影响控件排布
        /// </summary>
        public bool AffectsArrange => (_options & PropertyMetadataOptions.AffectsArrange) != 0;

        /// <summary>
        /// 获取一个值，该值指示依赖属性是否影响控件测量
        /// </summary>
        public bool AffectsMeasure => (_options & PropertyMetadataOptions.AffectsMeasure) > PropertyMetadataOptions.AffectsArrange;

        /// <summary>
        /// 获取一个值，该值指示依赖属性是否影响控件渲染
        /// </summary>
        public bool AffectsRender => (_options & PropertyMetadataOptions.AffectsRender) != 0;

        /// <summary>
        /// 获取一个值，该值指示依赖属性值是否可以被子元素继承
        /// </summary>
        public bool Inherits => (_options & PropertyMetadataOptions.Inherits) != 0;

        /// <summary>
        /// 获取一个值，该值指示该元数据是否已应用到某一依赖属性
        /// </summary>
        public bool IsSealed => (_options & PropertyMetadataOptions.Sealed) != 0;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="options">指定依赖属性行为的选项</param>
        public PropertyMetadata(PropertyMetadataOptions options = PropertyMetadataOptions.None)
        {
            _options = options;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">依赖属性默认值</param>
        /// <param name="options">指定依赖属性行为的选项</param>
        public PropertyMetadata(object defaultValue, PropertyMetadataOptions options = PropertyMetadataOptions.None)
        {
            _defaultValue = defaultValue;

            _options = options;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="coerceValueCallback">属性值处理回调</param>
        /// <param name="options">指定依赖属性行为的选项</param>
        public PropertyMetadata(CoerceValueCallback coerceValueCallback, PropertyMetadataOptions options = PropertyMetadataOptions.None)
        {
            _coerceValueCallback = coerceValueCallback;

            _options = options;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">依赖属性默认值</param>
        /// <param name="coerceValueCallback">属性值处理回调</param>
        /// <param name="options">指定依赖属性行为的选项</param>
        public PropertyMetadata(object defaultValue, CoerceValueCallback coerceValueCallback, PropertyMetadataOptions options = PropertyMetadataOptions.None)
        {
            _defaultValue = defaultValue;
            _coerceValueCallback = coerceValueCallback;

            _options = options;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="propertyChangedCallback">属性更改回调</param>
        /// <param name="options">指定依赖属性行为的选项</param>
        public PropertyMetadata(PropertyChangedCallback propertyChangedCallback, PropertyMetadataOptions options = PropertyMetadataOptions.None)
        {
            _propertyChangedCallback = propertyChangedCallback;

            _options = options;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">依赖属性默认值</param>
        /// <param name="propertyChangedCallback">属性更改回调</param>
        /// <param name="options">指定依赖属性行为的选项</param>
        public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, PropertyMetadataOptions options = PropertyMetadataOptions.None)
        {
            _defaultValue = defaultValue;
            _propertyChangedCallback = propertyChangedCallback;

            _options = options;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="coerceValueCallback">属性值处理回调</param>
        /// <param name="propertyChangedCallback">属性更改回调</param>
        /// <param name="options">指定依赖属性行为的选项</param>
        public PropertyMetadata(CoerceValueCallback coerceValueCallback, PropertyChangedCallback propertyChangedCallback, PropertyMetadataOptions options = PropertyMetadataOptions.None)
        {
            _coerceValueCallback = coerceValueCallback;
            _propertyChangedCallback = propertyChangedCallback;

            _options = options;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">依赖属性默认值</param>
        /// <param name="coerceValueCallback">属性值处理回调</param>
        /// <param name="propertyChangedCallback">属性更改回调</param>
        /// <param name="options">指定依赖属性行为的选项</param>
        public PropertyMetadata(object defaultValue, CoerceValueCallback coerceValueCallback, PropertyChangedCallback propertyChangedCallback, PropertyMetadataOptions options = PropertyMetadataOptions.None)
        {
            _defaultValue = defaultValue;

            _coerceValueCallback = coerceValueCallback;
            _propertyChangedCallback = propertyChangedCallback;

            _options = options;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 与元数据 <paramref name="baseMetadata"/> 合并
        /// </summary>
        /// <param name="baseMetadata">将合并的元数据</param>
        /// <param name="property">要应用此元数据的依赖属性</param>
        protected virtual void Merge(PropertyMetadata baseMetadata, DependencyProperty property)
        {
            if (_defaultValue is null)
            {
                _defaultValue = baseMetadata._defaultValue;
            }

            _propertyChangedCallback = (PropertyChangedCallback)
                Delegate.Combine(baseMetadata._propertyChangedCallback, _propertyChangedCallback);

            if (_coerceValueCallback is null)
            {
                _coerceValueCallback = baseMetadata._coerceValueCallback;
            }
        }

        /// <summary>
        /// 当此元数据应用到指定依赖属性时调用
        /// </summary>
        /// <param name="property">应用此元数据的依赖属性</param>
        protected virtual void OnApply(DependencyProperty property) { }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal void ApplyTo(DependencyProperty property)
        {
            OnApply(property);
            _options |= PropertyMetadataOptions.Sealed;
        }

        internal void ApplyTo(DependencyProperty property, PropertyMetadata baseMetadata)
        {
            if ((_options & PropertyMetadataOptions.Sealed) != 0)
            {
                throw new InvalidOperationException($"Metadata '{baseMetadata._defaultValue.AsLog()}' already applied to another dependency property '{property}'.");
            }

            Merge(baseMetadata, property);
            OnApply(property);

            _options |= PropertyMetadataOptions.Sealed;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private PropertyMetadataOptions _options;

        private object _defaultValue;

        private CoerceValueCallback _coerceValueCallback;
        private PropertyChangedCallback _propertyChangedCallback;

        #endregion
    }
}
