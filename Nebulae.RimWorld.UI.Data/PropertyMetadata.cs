using System;

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
    /// 表示依赖属性的有效值更改后调用的方法
    /// </summary>
    /// <param name="dp">属性值更改的依赖属性</param>
    /// <param name="args">有关属性更改的数据</param>
    public delegate void PropertyChangedCallback(DependencyObject dp, DependencyPropertyChangedEventArgs args);

    /// <summary>
    /// 元数据的特殊标记
    /// </summary>
    [Flags]
    public enum MetadataFlag : int
    {
        /// <summary>
        /// 无标记
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// 继承元数据的默认值
        /// </summary>
        InheritDefaultValue = 0x00000001,
        /// <summary>
        /// 允许继承 <see cref="PropertyChangedCallback"/>
        /// </summary>
        InheritablePropertyChangedCallback = 0x00000002
    }

    /// <summary>
    /// 属性元数据
    /// </summary>
    public class PropertyMetadata
    {
        private readonly MetadataFlag _flags;


        /// <summary>
        /// 强制转换回调函数
        /// </summary>
        internal CoerceValueCallback CoerceValueCallback;

        /// <summary>
        /// 属性更改回调函数
        /// </summary>
        internal PropertyChangedCallback PropertyChangedCallback;

        internal object DefaultValue;
        internal DependencyProperty Property;


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
            DefaultValue = null;

            _flags = MetadataFlag.InheritablePropertyChangedCallback;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="flags">元数据的特殊标记</param>
        public PropertyMetadata(
            object defaultValue,
            MetadataFlag flags = MetadataFlag.InheritablePropertyChangedCallback)
        {
            DefaultValue = defaultValue;

            _flags = flags;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        /// <param name="flags">元数据的特殊标记</param>
        public PropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            MetadataFlag flags = MetadataFlag.InheritablePropertyChangedCallback)
        {
            DefaultValue = defaultValue;

            PropertyChangedCallback = propertyChangedCallback;
            _flags = flags;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        /// <param name="flags">元数据的特殊标记</param>
        public PropertyMetadata(
            object defaultValue,
            CoerceValueCallback coerceValueCallback,
            MetadataFlag flags = MetadataFlag.InheritablePropertyChangedCallback)
        {
            DefaultValue = defaultValue;

            CoerceValueCallback = coerceValueCallback;
            _flags = flags;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        /// <param name="flags">元数据的特殊标记</param>
        public PropertyMetadata(
            object defaultValue,
            CoerceValueCallback coerceValueCallback,
            PropertyChangedCallback propertyChangedCallback,
            MetadataFlag flags = MetadataFlag.InheritablePropertyChangedCallback)
        {
            DefaultValue = defaultValue;

            CoerceValueCallback = coerceValueCallback;
            PropertyChangedCallback = propertyChangedCallback;
            _flags = flags;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 强制转换要设置给属性的值
        /// </summary>
        /// <param name="obj">请求转换属性值的对象</param>
        /// <param name="baseValue">要设置给属性的值</param>
        /// <returns>转换后的值。</returns>
        public object CoerceValue(DependencyObject obj, object baseValue)
        {
            return CoerceValueCallback?.Invoke(obj, baseValue) ?? baseValue;
        }

        /// <summary>
        /// 获取属性的默认值
        /// </summary>
        /// <returns>属性的默认值。</returns>
        public object GetDefaultValue() => DefaultValue;

        /// <summary>
        /// 获取元数据关联的依赖属性
        /// </summary>
        /// <returns>元数据关联的依赖属性。</returns>
        public DependencyProperty GetProperty() => Property;

        /// <summary>
        /// 合并元数据
        /// </summary>
        /// <param name="baseMetadata">被合并的元数据</param>
        public virtual void MergeMetadata(PropertyMetadata baseMetadata)
        {
            if (((baseMetadata._flags & MetadataFlag.InheritDefaultValue) != 0)
                || DefaultValue is null)
            {
                DefaultValue = baseMetadata.DefaultValue;
            }

            if (((baseMetadata._flags & MetadataFlag.InheritablePropertyChangedCallback) != 0)
                && baseMetadata.PropertyChangedCallback != null)
            {
                Delegate[] baseDelegates = baseMetadata.PropertyChangedCallback.GetInvocationList();
                Delegate[] delegates = PropertyChangedCallback?.GetInvocationList() ?? Array.Empty<Delegate>();

                PropertyChangedCallback = (PropertyChangedCallback)baseDelegates[0];
                for (int i = 1; i < baseDelegates.Length; i++)
                {
                    PropertyChangedCallback += (PropertyChangedCallback)baseDelegates[i];
                }
                for (int i = 0; i < delegates.Length; i++)
                {
                    PropertyChangedCallback += (PropertyChangedCallback)delegates[i];
                }
            }

            if (CoerceValueCallback == null)
            {
                CoerceValueCallback = baseMetadata.CoerceValueCallback;
            }

            Property = baseMetadata.Property;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods



        /// <summary>
        /// 通知属性更改
        /// </summary>
        /// <param name="obj">更改属性值的对象</param>
        /// <param name="newEntry">新的有效项</param>
        /// <returns>有关属性更改的数据。</returns>
        internal DependencyPropertyChangedEventArgs NotifyPropertyChanged(DependencyObject obj, EffectiveValueEntry newEntry)
        {
            DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs(Property, this, newEntry);

            PropertyChangedCallback?.Invoke(obj, args);
            return args;
        }

        /// <summary>
        /// 通知属性更改
        /// </summary>
        /// <param name="obj">更改属性值的对象</param>
        /// <param name="oldEntry">旧的有效项</param>
        /// <param name="newEntry">新的有效项</param>
        /// <returns>有关属性更改的数据。</returns>
        internal DependencyPropertyChangedEventArgs NotifyPropertyChanged(DependencyObject obj, EffectiveValueEntry oldEntry, EffectiveValueEntry newEntry)
        {
            DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs(Property, this, oldEntry, newEntry);

            PropertyChangedCallback?.Invoke(obj, args);
            return args;
        }

        #endregion
    }
}
