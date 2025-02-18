using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Windows;

namespace Nebulae.RimWorld.UI.Utilities
{

    /// <summary>
    /// 控件树工具类
    /// </summary>
    public static class LogicalTreeUtility
    {
        /// <summary>
        /// 判断指定控件是否为该控件的父控件
        /// </summary>
        /// <param name="control">要判断父控件的控件</param>
        /// <param name="target">可能为 <paramref name="control"/> 的父控件的控件</param>
        /// <returns>如果 <paramref name="target"/> 为 <paramref name="control"/> 的父控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool IsParent(this Control control, Control target)
        {
            if (control.IsChild)
            {
                if (ReferenceEquals(control.Parent, target))
                {
                    return true;
                }
                else
                {
                    return IsParent(control.Parent, target);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 移除控件的父控件
        /// </summary>
        /// <param name="control">要移除父控件的控件</param>
        public static void RemoveParent(this Control control)
        {
            control.IsChild = false;
            control.Owner = null;
            control.Parent = null;
            control.Rank = 0;
        }

        /// <summary>
        /// 设置控件的父控件
        /// </summary>
        /// <param name="control">要设置父控件的控件</param>
        /// <param name="parent">设置给控件的父控件</param>
        public static void SetParent(this Control control, Control parent)
        {
            if (parent is null)
            {
                control.IsChild = false;
                control.Owner = null;
                control.Parent = null;
                control.Rank = 0;
            }
            else
            {
                control.IsChild = true;
                control.Owner = parent.Owner;
                control.Parent = parent;
                control.Rank = parent.Rank + 1;
            }
        }
    }
}
