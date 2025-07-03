using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data.Bindings;

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

        /// <summary>
        /// 解除指定控件及其所有子控件的所有相关的绑定关系
        /// </summary>
        /// <param name="control">指定的控件</param>
        public static void Unbind(this Control control)
        {
            Binding.Unbind(control);

            foreach (var child in control.Descendants)
            {
                Binding.Unbind(child);
            }
        }
    }
}
