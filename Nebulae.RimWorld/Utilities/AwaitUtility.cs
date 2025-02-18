using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 等待帮助类
    /// </summary>
    /// <remarks>包含了一些异步任务。</remarks>
    public static class AwaitUtility
    {
        /// <summary>
        /// 等待游戏结束暂停
        /// </summary>
        public static async void WaitUntilUnpaused()
        {
            await Task.Run(() =>
            {
                while (Find.TickManager.Paused)
                {
                    Thread.Sleep(60);
                }
            });
        }

        /// <summary>
        /// 等待游戏结束暂停
        /// </summary>
        /// <param name="onGameUnpaused">游戏接触暂停时执行的操作</param>
        public static async void WaitUntilUnpaused(Action onGameUnpaused)
        {
            await Task.Run(() =>
            {
                while (Find.TickManager.Paused)
                {
                    Thread.Sleep(60);
                }
            });

            onGameUnpaused.Invoke();
        }
    }
}
