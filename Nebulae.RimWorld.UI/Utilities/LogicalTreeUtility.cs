using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Data.Binding;
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
        public static bool IsChildOf(this Control control, Control target)
        {
            if (control.IsChild)
            {
                if (ReferenceEquals(control.Parent, target))
                {
                    return true;
                }
                else
                {
                    return IsChildOf(control.Parent, target);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 显示以目标控件为根控件的控件树上的所有控件的信息
        /// </summary>
        /// <param name="root">目标控件</param>
        public static void ShowInfo(this Control root)
        {
            DebugWindow.ShowWindow(root);
        }

        /// <summary>
        /// 解除以目标控件为根控件的控件树上的所有绑定关系
        /// </summary>
        /// <param name="root">目标控件</param>
        public static void Unbind(this Control root)
        {
            BindingManager.Unbind(root);

            foreach (var child in root.LogicalChildren)
            {
                BindingManager.Unbind(child);
            }
        }
    }
}
