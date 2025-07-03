using Nebulae.RimWorld.UI.Core.Data.Bindings.Converters;
using Nebulae.RimWorld.UI.Core.Data.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI.Core.Data.Bindings
{
    /// <summary>
    /// 表示一个或多个对象指定成员间的绑定关系的基类，定义了其共同行为
    /// </summary>
    public abstract class BindingBase
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 绑定成员间的值转换器
        /// </summary>
        public readonly IValueConverter Converter;

        /// <summary>
        /// 绑定成员之间的关系
        /// </summary>
        public readonly BindingMode Mode;

        /// <summary>
        /// 绑定成员之间的值是否需要使用 <see cref="Converter"/> 进行转换
        /// </summary>
        public readonly bool RequiresConversion;

        #endregion


        /// <summary>
        /// 获取一个值，该值指示绑定关系是否正在保持
        /// </summary>
        public bool IsBinding => _isBinding;


        /// <summary>
        /// 为 <see cref="BindingBase"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <param name="converter">绑定成员间的值转换器</param>
        protected BindingBase(BindingMode mode, IValueConverter converter)
        {
            Converter = converter;
            Mode = mode;
            RequiresConversion = converter != null;
        }


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

        /// <summary>
        /// 添加类型间的默认的值转换器
        /// </summary>
        /// <param name="sourceType">源数据类型</param>
        /// <param name="targetType">目标数据类型</param>
        /// <param name="converter">从 <paramref name="sourceType"/> 到 <paramref name="targetType"/> 的值转换器</param>
        /// <returns>若成功添加，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool AddDefaultConverter(Type sourceType, Type targetType, IValueConverter converter)
        {
            return DefaultConverters.TryAdd(new ConverterKey(sourceType, targetType), converter);
        }

        /// <summary>
        /// 获取类型间的默认的值转换器
        /// </summary>
        /// <param name="sourceType">源数据类型</param>
        /// <param name="targetType">目标数据类型</param>
        /// <returns>从 <paramref name="sourceType"/> 到 <paramref name="targetType"/> 的值转换器。</returns>
        public static IValueConverter GetDefaultConverter(Type sourceType, Type targetType)
        {
            if (sourceType == targetType)
            {
                return null;
            }

            if (DefaultConverters.TryGetValue(new ConverterKey(sourceType, targetType), out var converter))
            {
                return converter;
            }

            if (SystemConvertUtility.CanConvert(sourceType, targetType))
            {
                return SystemConverter.Instance;
            }
            else
            {
                throw new InvalidOperationException($"Default binding converter cannot cast type: {sourceType} between type: {targetType}.");
            }
        }

        /// <summary>
        /// 设置类型间的默认的值转换器
        /// </summary>
        /// <param name="sourceType">源数据类型</param>
        /// <param name="targetType">目标数据类型</param>
        /// <param name="converter">从 <paramref name="sourceType"/> 到 <paramref name="targetType"/> 的值转换器</param>
        public static void SetDefaultConverter(Type sourceType, Type targetType, IValueConverter converter)
        {
            var key = new ConverterKey(sourceType, targetType);

            DefaultConverters.Remove(key);
            DefaultConverters[key] = converter;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 强制以当前绑定模式同步绑定成员的值
        /// </summary>
        public void Synchronize()
        {
            if (_isBinding)
            {
                OnSynchronize();
            }
        }

        /// <summary>
        /// 解除该绑定关系
        /// </summary>
        public void Unbind()
        {
            OnUnbind();
            _isBinding = false;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 判断指定成员是否为该绑定关系的源成员
        /// </summary>
        /// <param name="obj">要判断的源</param>
        /// <param name="memberName">要判断的源成员名</param>
        /// <returns>若指定成员是该绑定关系的源成员，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        protected internal abstract bool IsSource(object obj, string memberName);

        /// <summary>
        /// 判断指定成员是否为该绑定关系的目标成员
        /// </summary>
        /// <param name="obj">要判断的目标</param>
        /// <param name="memberName">要判断的目标成员名</param>
        /// <returns>若指定成员是该绑定关系的目标成员，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        protected internal abstract bool IsTarget(object obj, string memberName);

        /// <summary>
        /// 强制以当前绑定模式同步绑定成员的值时调用的方法
        /// </summary>
        protected abstract void OnSynchronize();

        /// <summary>
        /// 解除绑定关系时调用的方法
        /// </summary>
        protected abstract void OnUnbind();

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly Dictionary<ConverterKey, IValueConverter> DefaultConverters = new Dictionary<ConverterKey, IValueConverter>();

        private bool _isBinding = true;

        #endregion
    }
}
