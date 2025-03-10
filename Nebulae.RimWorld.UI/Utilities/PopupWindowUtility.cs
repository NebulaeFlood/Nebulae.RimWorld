using Nebulae.RimWorld.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// <see cref="ImmediateWindow"/> 帮助类
    /// </summary>
    /// <remarks>帮助 <see cref="PopupWindow"/> 获取类似 <see cref="ImmediateWindow"/> 的特性。</remarks>
    internal static class PopupWindowUtility
    {
        private static readonly HashSet<PopupWindow> _windows = new HashSet<PopupWindow>();


        /// <summary>
        /// 管理窗口，使之获取获取类似 <see cref="ImmediateWindow"/> 的特性
        /// </summary>
        /// <param name="window">要管理的窗口</param>
        internal static void Manage(PopupWindow window)
        {
            _windows.Add(window);
        }


        internal static void CheckState(List<Window> windowStack)
        {
            int count = _windows.Count;

            if (count > 0)
            {
                for (int i = windowStack.Count - 1; i >= 0; i--)
                {
                    if (windowStack[i] is PopupWindow window
                        && !window.StayOpen)
                    {
                        windowStack.RemoveAt(i);
                        _windows.Remove(window);

                        if (--count < 1)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}