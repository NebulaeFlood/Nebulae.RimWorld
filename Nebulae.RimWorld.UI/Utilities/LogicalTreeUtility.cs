﻿using Nebulae.RimWorld.UI.Controls.Basic;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 控件树帮助类
    /// </summary>
    public static class LogicalTreeUtility
    {
        /// <summary>
        /// 获取控件的指定代数的父控件
        /// </summary>
        /// <param name="control">要获取父控件的控件</param>
        /// <param name="depth">指定的代数</param>
        /// <returns>控件的指定代数的父控件，若 <paramref name="depth"/> 小于 <see langword="0"/>，返回结果为 <paramref name="control"/>；若不存在，则返回 <see langword="null"/>。</returns>
        public static Control GetParent(this Control control, int depth)
        {
            var result = control;

            while (depth >= 0)
            {
                if (result is null)
                {
                    break;
                }

                depth--;
                result = result.Parent;
            }

            return result;
        }

        /// <summary>
        /// 尝试查找控件的指定类型的父控件
        /// </summary>
        /// <typeparam name="T">父控件的类型</typeparam>
        /// <param name="control">要查找父控件的控件</param>
        /// <param name="parent">控件的父控件</param>
        /// <returns>若找到指定类型的父控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool TryFindPartent<T>(this Control control, out T parent) where T : Control
        {
            if (control is null)
            {
                parent = null;
                return false;
            }

            var currentParent = control.Parent;

            while (currentParent != null)
            {
                if (currentParent is T p)
                {
                    parent = p;
                    return true;
                }

                currentParent = currentParent.Parent;
            }

            parent = null;
            return false;
        }

        /// <summary>
        /// 判断指定控件是否为该控件的父控件
        /// </summary>
        /// <param name="control">要判断父控件的控件</param>
        /// <param name="target">可能为 <paramref name="control"/> 的父控件的控件</param>
        /// <returns>如果 <paramref name="target"/> 为 <paramref name="control"/> 的父控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool IsChildOf(this Control control, Control target)
        {
            var parent = control.Parent;

            while (parent != null)
            {
                if (ReferenceEquals(parent, target))
                {
                    return true;
                }

                parent = parent.Parent;
            }

            return false;
        }
    }
}
