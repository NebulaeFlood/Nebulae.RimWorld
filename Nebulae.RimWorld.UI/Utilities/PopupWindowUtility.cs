using Nebulae.RimWorld.UI.Windows;
using System.Collections.Generic;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// <see cref="PopupWindow"/> 帮助类
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
            window.absorbInputAroundWindow = false;
            window.closeOnAccept = false;
            window.closeOnCancel = false;
            window.focusWhenOpened = false;

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
                        if (window.StayOpen)
                        {
                            window.StayOpen = false;
                        }
                        else
                        {
                            window.PreClose();
                            windowStack.RemoveAt(i);
                            window.PostClose();

                            window.Popped = false;

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
}