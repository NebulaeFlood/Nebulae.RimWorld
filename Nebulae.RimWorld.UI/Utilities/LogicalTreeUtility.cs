using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
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
        /// 获取控件所在控件树的根控件
        /// </summary>
        /// <param name="control">位于控件树上的控件</param>
        /// <returns><paramref name="control"/> 所在控件树的根控件。</returns>
        public static Visual GetLogicalRoot(this Visual control)
        {
            if (control.IsIndependent)
            {
                return control;
            }

            return control.LayoutManager.Root;
        }

        /// <summary>
        /// 获取控件特定类型的父控件
        /// </summary>
        /// <typeparam name="T">父控件的类型</typeparam>
        /// <param name="control">要获取父控件的控件</param>
        /// <returns>控件的特定类型的父控件，若不存在，则返回 <see langword="null"/>。</returns>
        public static Visual GetParent<T>(this Visual control) where T : Visual
        {
            if (control.IsIndependent)
            {
                return null;
            }

            if (control.Parent is T parent)
            {
                return parent;
            }

            return GetParent<T>(control.Parent);
        }

        /// <summary>
        /// 获取控件的指定代数的父控件
        /// </summary>
        /// <param name="control">要获取父控件的控件</param>
        /// <param name="depth">指定的代数</param>
        /// <returns>控件的指定代数的父控件，若不存在，则返回 <see langword="null"/>。</returns>
        public static Visual GetParent(this Visual control, int depth)
        {
            if (control.IsIndependent)
            {
                return null;
            }

            if (depth < 0)
            {
                return control;
            }

            return GetParent(control.Parent, depth - 1);
        }

        /// <summary>
        /// 判断指定控件是否为该控件的父控件
        /// </summary>
        /// <param name="control">要判断父控件的控件</param>
        /// <param name="target">可能为 <paramref name="control"/> 的父控件的控件</param>
        /// <returns>如果 <paramref name="target"/> 为 <paramref name="control"/> 的父控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool IsChildOf(this Visual control, Visual target)
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
        public static void ShowInfo(this Visual root)
        {
            DebugWindow.ShowWindow(root);
        }

        /// <summary>
        /// 解除以目标控件为根控件的控件树上的所有绑定关系
        /// </summary>
        /// <param name="root">目标控件</param>
        public static void Unbind(this Visual root)
        {
            BindingManager.Unbind(root);

            foreach (var child in root.LogicalChildren)
            {
                BindingManager.Unbind(child);
            }
        }

        /// <summary>
        /// 解除控件树上的所有绑定关系
        /// </summary>
        /// <param name="manager">目标控件树</param>
        public static void Unbind(this LayoutManager manager)
        {
            if (manager.Root is Visual root)
            {
                BindingManager.Unbind(root);

                foreach (var child in root.LogicalChildren)
                {
                    BindingManager.Unbind(child);
                }
            }
        }

        /// <summary>
        /// 解除窗口的控件树上的所有绑定关系
        /// </summary>
        /// <param name="window">目标窗口</param>
        public static void Unbind(this ControlWindow window)
        {
            if (window.Content is Visual root)
            {
                BindingManager.Unbind(root);

                foreach (var child in root.LogicalChildren)
                {
                    BindingManager.Unbind(child);
                }
            }
        }
    }
}
