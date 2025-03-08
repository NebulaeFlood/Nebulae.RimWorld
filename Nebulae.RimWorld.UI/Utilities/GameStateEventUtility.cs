using Nebulae.RimWorld.WeakEventManagers;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 游戏状态监听器接口
    /// </summary>
    public interface IGameStateListener
    {
        /// <summary>
        /// 当游戏状态发生变化时执行的方法
        /// </summary>
        /// <param name="oldState">旧的游戏状态</param>
        /// <param name="newState">当前游戏状态</param>
        void OnGameStateChanged(ProgramState oldState, ProgramState newState);
    }

    /// <summary>
    /// 游戏状态帮助类
    /// </summary>
    public static class GameStateEventUtility
    {
        private static readonly GameStateEvent _event = new GameStateEvent();
        private static ProgramState _currentGameState = ProgramState.Entry;


        /// <summary>
        /// 添加游戏状态事件监听器
        /// </summary>
        /// <param name="listener">要监听事件的对象</param>
        public static void AddListener(IGameStateListener listener)
        {
            _event.Manage(listener);
        }

        /// <summary>
        /// 移除游戏状态事件监听器
        /// </summary>
        /// <param name="listener">要取消监听事件的对象</param>
        public static void RemoveListener(IGameStateListener listener)
        {
            _event.Remove(listener);
        }


        internal static void CheckState(ProgramState state)
        {
            if (state != _currentGameState)
            {
                _event.Invoke(_currentGameState, state);
                _currentGameState = state;
            }
        }


        internal sealed class GameStateEvent : WeakEventManager<IGameStateListener>
        {
            internal void Invoke(ProgramState oldState, ProgramState newState)
            {
                var subscribers = GetSubcribers();

                for (int i = subscribers.Count - 1; i >= 0; i--)
                {
                    subscribers[i].OnGameStateChanged(oldState, newState);
                }
            }
        }
    }
}
