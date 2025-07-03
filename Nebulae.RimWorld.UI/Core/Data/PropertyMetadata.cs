using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 依赖属性元数据
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
        /// 获取或设置对此元数据中所指定 <see cref="Data.CoerceValueCallback"/> 实现的引用
        /// </summary>
        public CoerceValueCallback CoerceValueCallback
        {
            get => coerceValueCallback;
            set
            {
                if (isSealed)
                {
                    throw new InvalidOperationException("Cannot modify metadata after it has been used by a dependency property.");
                }

                coerceValueCallback = value;
            }
        }

        /// <summary>
        /// 获取或设置依赖属性的默认值
        /// </summary>
        public object DefaultValue
        {
            get => defaultValue;
            set
            {
                if (isSealed)
                {
                    throw new InvalidOperationException("Cannot modify metadata after it has been used by a dependency property.");
                }

                defaultValue = value;
            }
        }

        /// <summary>
        /// 此元数据是否不可变
        /// </summary>
        /// <remarks>该值确定是否已通过某种方式将元数据应用于依赖属性</remarks>
        public bool IsSealed => isSealed;

        /// <summary>
        /// 获取或设置对此元数据中所指定 <see cref="Data.PropertyChangedCallback"/> 实现的引用
        /// </summary>
        public PropertyChangedCallback PropertyChangedCallback
        {
            get => propertyChangedCallback;
            set
            {
                if (isSealed)
                {
                    throw new InvalidOperationException("Cannot modify metadata after it has been used by a dependency property.");
                }

                propertyChangedCallback = value;
            }
        }

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
        public PropertyMetadata()
        {
            defaultValue = null;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        public PropertyMetadata(object defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        public PropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
        {
            this.defaultValue = defaultValue;
            this.propertyChangedCallback = propertyChangedCallback;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        public PropertyMetadata(
            object defaultValue,
            CoerceValueCallback coerceValueCallback)
        {
            this.defaultValue = defaultValue;

            this.coerceValueCallback = coerceValueCallback;
        }

        /// <summary>
        /// 初始化 <see cref="PropertyMetadata"/> 的新实例
        /// </summary>
        /// <param name="defaultValue">属性默认值</param>
        /// <param name="coerceValueCallback">强制转换回调函数</param>
        /// <param name="propertyChangedCallback">属性更改回调函数</param>
        public PropertyMetadata(
            object defaultValue,
            CoerceValueCallback coerceValueCallback,
            PropertyChangedCallback propertyChangedCallback)
        {
            this.defaultValue = defaultValue;

            this.coerceValueCallback = coerceValueCallback;
            this.propertyChangedCallback = propertyChangedCallback;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Method
        //
        //------------------------------------------------------

        #region Public Method

        /// <summary>
        /// 合并元数据
        /// </summary>
        /// <param name="metadata">被合并的元数据</param>
        public virtual void Merge(PropertyMetadata metadata)
        {
            if (defaultValue is null)
            {
                defaultValue = metadata.defaultValue;
            }

            if (coerceValueCallback is null)
            {
                coerceValueCallback = metadata.coerceValueCallback;
            }

            if (propertyChangedCallback is null)
            {
                propertyChangedCallback = metadata.propertyChangedCallback;
            }
            else if (metadata.propertyChangedCallback != null)
            {
                propertyChangedCallback = (PropertyChangedCallback)Delegate.Combine(
                    metadata.propertyChangedCallback, propertyChangedCallback);
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        internal object defaultValue;

        internal CoerceValueCallback coerceValueCallback;
        internal PropertyChangedCallback propertyChangedCallback;

        internal bool isSealed;

        #endregion
    }
}
