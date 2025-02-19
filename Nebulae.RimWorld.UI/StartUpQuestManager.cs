using System;
using System.Collections.Generic;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 当资源完全加载时执行的操作的管理器
    /// </summary>
    public static class StartUpQuestManager
    {
        private static List<Action> _actionQueue = new List<Action>();


        /// <summary>
        /// 添加要在资源加载完毕时执行的操作
        /// </summary>
        /// <param name="action">资源加载完毕时执行的操作</param>
        public static void AddQuest(Action action)
        {
            if (_actionQueue is null)
            {
                return;
            }

            _actionQueue.Add(action);
        }


        internal static void FinishQuests()
        {
            for (int i = 0; i < _actionQueue.Count; i++)
            {
                try
                {
                    _actionQueue[i].Invoke();
                }
                catch (Exception e)
                {
                    Log.Error($"An exception occured when try finish quest from {_actionQueue[i].Method.DeclaringType}. ---> {e}");
                }
            }

            _actionQueue.Clear();
            _actionQueue = null;
        }
    }


    [StaticConstructorOnStartup]
    internal static class StartUp
    {
        static StartUp()
        {
            StartUpQuestManager.FinishQuests();
        }
    }
}
