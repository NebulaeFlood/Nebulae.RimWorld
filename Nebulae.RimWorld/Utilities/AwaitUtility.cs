using System.Threading.Tasks;
using UnityEngine;
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
        /// 等待指定秒
        /// </summary>
        /// <param name="seconds">要等待秒数</param>
        /// <param name="useGameTickRate">等待时间是否受游戏速度影响</param>
        /// <remarks>当 <paramref name="useGameTickRate"/> 为 <see langword="true"/> 时，等待时间仅受开始等待时的游戏速度影响。</remarks>
        public static async Task WaitForSecondsAsync(float seconds, bool useGameTickRate = true)
        {
            if (useGameTickRate)
            {
                await Task.Delay((seconds / Find.TickManager.TickRateMultiplier)
                    .SecondsToTicks());
            }
            else
            {
                await Task.Delay(seconds.SecondsToTicks());
            }
        }

        /// <summary>
        /// 等待指定 Tick 数
        /// </summary>
        /// <param name="ticks">要等待的 Tick 数</param>
        /// <param name="useGameTickRate">等待时间是否受游戏速度影响</param>
        /// <remarks>当 <paramref name="useGameTickRate"/> 为 <see langword="true"/> 时，等待时间仅受开始等待时的游戏速度影响。</remarks>
        public static async Task WaitForTicksAsync(int ticks, bool useGameTickRate = true)
        {
            if (useGameTickRate)
            {
                await Task.Delay(
                    Mathf.RoundToInt(
                        Mathf.RoundToInt((ticks / Find.TickManager.TickRateMultiplier) * 1000f)
                            .TicksToSeconds()));
            }
            else
            {
                await Task.Delay(Mathf.RoundToInt(ticks.TicksToSeconds() * 1000f));
            }
        }

        /// <summary>
        /// 等待游戏结束暂停
        /// </summary>
        public static async Task WaitForUnpauseAsync()
        {
            if (!Find.TickManager.Paused)
            {
                return;
            }

            while (Find.TickManager.Paused)
            {
                await Task.Delay(60);
            }
        }
    }
}
