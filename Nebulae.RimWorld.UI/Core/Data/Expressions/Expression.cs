using Nebulae.RimWorld.UI.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data.Expressions
{
    /// <summary>
    /// 表达式
    /// </summary>
    public abstract class Expression
    {
        /// <summary>
        /// 表达式的 <see langword="null"/> 值
        /// </summary>
        /// <remarks>作为设置给依赖属性的值时，移除相关依赖属性上已设置的表达式。</remarks>
        public static readonly Expression Empty;


        /// <summary>
        /// 该表达式是否可赋值
        /// </summary>
        public readonly bool Assignable;


        /// <summary>
        /// 为 <see cref="Expression"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="assignable">表达式是否可赋值</param>
        protected Expression(bool assignable)
        {
            Assignable = assignable;
        }


        /// <summary>
        /// 获取表示该表达式的字符串
        /// </summary>
        /// <returns>表示该表达式的字符串。</returns>
        public abstract override string ToString();


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 将该表达式附加到依赖对象的指定依赖属性
        /// </summary>
        /// <param name="target">该表达式要附加到的依赖对象</param>
        /// <param name="property">该表达式要附加到的依赖属性</param>
        protected void Attach(DependencyObject target, DependencyProperty property)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            target.AddDependent(property, this);
        }

        /// <summary>
        /// 从依赖对象的指定依赖属性中移除该表达式
        /// </summary>
        /// <param name="target">要从中移除该表达式的依赖对象</param>
        /// <param name="property">要从中移除该表达式的依赖属性</param>
        protected void Detach(DependencyObject target, DependencyProperty property)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            target.RemoveDependent(property, this);
        }

        /// <summary>
        /// 计算该表达式的值
        /// </summary>
        /// <param name="caller">要求计算值的对象</param>
        /// <param name="property">要求计算值的属性</param>
        /// <returns>由该表达式计算得到的结果。</returns>
        /// <remarks>使用该方法时，不应使传入的 <paramref name="caller"/> 或 <paramref name="property"/> 为 <see langword="null"/>。</remarks>
        protected internal abstract object GetValue(DependencyObject caller, DependencyProperty property);

        /// <summary>
        /// 更新该表达式
        /// </summary>
        /// <param name="sender">引发更新的对象</param>
        /// <param name="property">有关更新的属性</param>
        /// <param name="value">属性的新值</param>
        protected internal abstract void Update(DependencyObject sender, DependencyProperty property, object value);

        #endregion
    }
}
