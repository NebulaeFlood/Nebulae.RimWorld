namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 表示设置给依赖属性的值需要转换类型时调用的方法
    /// </summary>
    /// <param name="dp">属性值将要更改的依赖属性</param>
    /// <param name="baseValue">将要设置的属性值</param>
    /// <returns>强制转换后的值。</returns>
    public delegate object CoerceValueCallback(DependencyObject dp, object baseValue);

    /// <summary>
    /// 表示依赖属性的有效值更改时调用的方法
    /// </summary>
    /// <param name="dp">属性值更改的依赖属性</param>
    /// <param name="args">有关属性更改的数据</param>
    public delegate void PropertyChangedCallback(DependencyObject dp, DependencyPropertyChangedEventArgs args);

    /// <summary>
    /// 属性元数据
    /// </summary>
    public class PropertyMetadata
    {
        internal event PropertyChangedCallback PropertyChanged;

        /// <summary>
        /// 强制转换回调函数
        /// </summary>
        private readonly CoerceValueCallback _coerceValueCallback;

        /// <summary>
        /// 属性更改回调函数
        /// </summary>
        private readonly PropertyChangedCallback _propertyChangedCallback;


        internal readonly object _defaultValue;


        /// <summary>
        /// 属性默认值
        /// </summary>
        public object DefaultValue => _defaultValue;


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        public PropertyMetadata()
        {
            _defaultValue = default;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        public PropertyMetadata(object defaultValue)
        {
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
        {
            _defaultValue = defaultValue;
            _propertyChangedCallback = propertyChangedCallback;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        public PropertyMetadata(object defaultValue, CoerceValueCallback coerceValueCallback)
        {
            _coerceValueCallback = coerceValueCallback;
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        public PropertyMetadata(object defaultValue, CoerceValueCallback coerceValueCallback, PropertyChangedCallback propertyChangedCallback)
        {
            _coerceValueCallback = coerceValueCallback;
            _defaultValue = defaultValue;
            _propertyChangedCallback = propertyChangedCallback;
        }

        #endregion


        /// <summary>
        /// 强制转换要设置给属性的值
        /// </summary>
        /// <param name="obj">请求转换属性值的对象</param>
        /// <param name="baseValue">要设置给属性的值</param>
        /// <returns>转换后的值。</returns>
        internal object CoerceValue(DependencyObject obj, object baseValue)
        {
            return _coerceValueCallback?.Invoke(obj, baseValue) ?? baseValue;
        }

        /// <summary>
        /// 通知属性更改
        /// </summary>
        /// <param name="obj">更改属性值的对象</param>
        /// <param name="args">有关属性被更改的数据</param>
        internal void NotifyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OnProertyChanged(obj, args);
            _propertyChangedCallback?.Invoke(obj, args);
            PropertyChanged?.Invoke(obj, args);
        }


        /// <summary>
        /// 当属性值更改时调用
        /// </summary>
        /// <param name="obj">更改属性值的对象</param>
        /// <param name="args">有关属性被更改的数据</param>
        protected virtual void OnProertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { }
    }
}
