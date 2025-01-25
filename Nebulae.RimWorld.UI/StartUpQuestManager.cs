using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _actionQueue.ForEach(x =>
            {
                try
                {
                    x.Invoke();
                }
                catch (Exception e)
                {
                    Log.Error($"An exception occured when try finish quest from {x.Method.DeclaringType}: {e}");
                }
            });

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
