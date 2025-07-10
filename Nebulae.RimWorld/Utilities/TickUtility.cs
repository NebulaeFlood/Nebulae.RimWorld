using System;
using System.Collections.Generic;
using Verse;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 定义一个可借助 <see cref="TickUtility"/> 以每隔 <see cref="TickUtility.IntervalTick"/> 刻执行一次指定方法的对象
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// 每隔 <see cref="TickUtility.IntervalTick"/> 刻执行一次的方法
        /// </summary>
        /// <param name="interval">每次调用该方法的间隔</param>
        /// <returns>若为 <see langword="false"/>，则将该对象移出 Tick 队列。</returns>
        bool Tick(int interval);

        /// <summary>
        /// 当该对象被添加到 Tick 队列时执行的方法
        /// </summary>
        void OnStarted();

        /// <summary>
        /// 当该对象被移出 Tick 队列时执行的方法
        /// </summary>
        void OnStopped();
    }


    /// <summary>
    /// 游戏刻帮助类
    /// </summary>
    public static class TickUtility
    {
        /// <summary>
        /// 每次执行 <see cref="IClock.Tick"/> 的间隔
        /// </summary>
        public static int IntervalTick
        {
            get => _intervalTick;
            set
            {
                if (_intervalTick != value)
                {
                    if (value < 30 || value > 900000)
                    {
                        throw new ArgumentException($"{value} is not available for TickUtility.IntervalTick");
                    }

                    _intervalTick = value;
                }
            }
        }


        /// <summary>
        /// 将指定对象添加至 Tick 队列
        /// </summary>
        /// <param name="clock">要添加到 Tick 队列的对象</param>
        public static void StartTick(this IClock clock)
        {
            if (clock is null || Clocks.Contains(clock))
            {
                return;
            }

            clock.OnStarted();
            Clocks.AddLast(clock);
        }

        /// <summary>
        /// 将指定对象移出 Tick 队列
        /// </summary>
        /// <param name="clock">要从 Tick 队列移出的对象</param>
        public static void StopTick(this IClock clock)
        {
            if (clock is null)
            {
                return;
            }

            if (Clocks.Remove(clock))
            {
                clock.OnStopped();
            }
        }


        /// <summary>
        /// 计算剩余的刻数，并判断是否小于零
        /// </summary>
        /// <param name="ticksLeft">剩余的刻数</param>
        /// <returns>如果剩余的刻数小于零，返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
        public static bool Time(ref int ticksLeft)
        {
            ticksLeft -= _intervalTick;
            return ticksLeft < 0;
        }


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static readonly LinkedList<IClock> Clocks = new LinkedList<IClock>();

        private static int _intervalTick = 60;

        #endregion



        private sealed class GameComponent_TickHelper : GameComponent
        {
            private int _tick = 0;

#pragma warning disable IDE0060 // 删除未使用的参数
            public GameComponent_TickHelper(Game game) { }
#pragma warning restore IDE0060 // 删除未使用的参数

            public override void GameComponentTick()
            {
                if (Clocks.Count < 1 || ++_tick % _intervalTick > 0)
                {
                    return;
                }

                var node = Clocks.First;

                while (node != null)
                {
                    if (!node.Value.Tick(_intervalTick))
                    {
                        var nextNode = node.Next;
                        Clocks.Remove(node);
                        node = nextNode;
                    }
                    else
                    {
                        node = node.Next;
                    }
                }

                _tick = 0;
            }

            public override void StartedNewGame()
            {
                Clocks.Clear();
            }
        }
    }
}
