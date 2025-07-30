using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 在相同区域内叠加呈现子控件的面板
    /// </summary>
    public sealed class LayeredPanel : Panel
    {
        /// <summary>
        /// 初始化 <see cref="LayeredPanel"/> 的新实例
        /// </summary>
        public LayeredPanel() { }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 向面板顶部添加一个控件
        /// </summary>
        /// <param name="control">要添加的控件</param>
        /// <returns>该面板控件。</returns>
        public LayeredPanel Below(Control control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件。</returns>
        public LayeredPanel Set(IEnumerable<Control> controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件。</returns>
        public LayeredPanel Set(params Control[] controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板中的所有控件
        /// </summary>
        public void Clear()
        {
            Children.Clear();
        }

        /// <summary>
        /// 将控件插入或移动到面板中的指定控件之前
        /// </summary>
        /// <param name="control">要插入的控件</param>
        /// <param name="index">被挤开的控件</param>
        /// <returns>若插入了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Insert(Control control, Control index)
        {
            return Children.Insert(index, control);
        }

        /// <summary>
        /// 移除面板中的指定控件
        /// </summary>
        /// <param name="control">要移除的控件</param>
        /// <returns>若移除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(Control control)
        {
            return Children.Remove(control);
        }

        /// <summary>
        /// 使用指定的比较强排序面板中的控件
        /// </summary>
        /// <param name="comparison">比较控件时使用的比较器</param>
        /// <returns>该面板控件。</returns>
        public LayeredPanel Sort(Comparison<Control> comparison)
        {
            Children.Sort(comparison);
            return this;
        }

        /// <summary>
        /// 使用指定的比较强排序面板中的控件
        /// </summary>
        /// <param name="comparer">比较控件时使用的比较器</param>
        /// <returns>该面板控件。</returns>
        public LayeredPanel Sort(IComparer<Control> comparer)
        {
            Children.Sort(comparer);
            return this;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeOverride(Rect availableRect, Control[] children)
        {
            for (int i = children.Length - 1; i >= 0; i--)
            {
                children[i].Arrange(availableRect);
            }

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            for (int i = children.Length - 1; i >= 0; i--)
            {
                children[i].Measure(availableSize);
            }

            return availableSize;
        }

        #endregion
    }
}
