using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 定义一个可借助 <see cref="TickUtility"/> 每隔 <see cref="TickUtility.IntervalTick"/> 刻执行一次指定方法的对象
    /// </summary>
    public interface ITickAvailable
    {
        /// <summary>
        /// 每隔 <see cref="TickUtility.IntervalTick"/> 刻执行一次的方法
        /// </summary>
        /// <returns>是否将该对象移出 Tick 队列。</returns>
        bool ProcessTick();

        /// <summary>
        /// 当该对象被添加到 Tick 队列时执行的方法
        /// </summary>
        void OnTickStart();

        /// <summary>
        /// 当该对象被移出 Tick 队列时执行的方法
        /// </summary>
        void OnTickStopped();
    }

    /// <summary>
    /// 游戏刻帮助类
    /// </summary>
    public static class TickUtility
    {
        private static readonly List<ITickAvailable> _objectsNeedToTick = new List<ITickAvailable>();

        private static int _intervalTick = 60;

        /// <summary>
        /// 每次 Tick 的间隔
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
        /// <param name="obj">要添加到 Tick 队列的对象</param>
        public static void StartTick(this ITickAvailable obj)
        {
            if (obj is null || _objectsNeedToTick.Contains(obj))
            {
                return;
            }

            _objectsNeedToTick.Add(obj);
        }

        /// <summary>
        /// 将指定对象移出 Tick 队列
        /// </summary>
        /// <param name="obj">要从 Tick 队列移出的对象</param>
        public static void StopTick(this ITickAvailable obj)
        {
            if (obj is null)
            {
                return;
            }

            _objectsNeedToTick.Remove(obj);
            obj.OnTickStopped();
        }



        private sealed class GameComponent_TickHelper : GameComponent
        {
            private int _tick = 0;

            public GameComponent_TickHelper(Game game)
            {
            }

            public override void GameComponentTick()
            {
                int count = _objectsNeedToTick.Count;

                if (count < 1 || ++_tick % _intervalTick > 0)
                {
                    return;
                }

                for (int i = count - 1; i >= 0; i--)
                {
                    if (_objectsNeedToTick[i].ProcessTick())
                    {
                        _objectsNeedToTick[i].OnTickStopped();
                        _objectsNeedToTick.RemoveAt(i);
                    }
                }

                _tick = 0;
            }

            public override void StartedNewGame()
            {
                _objectsNeedToTick.Clear();
            }
        }
    }
}
