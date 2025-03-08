using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 当资源完全加载时执行的操作的管理器
    /// </summary>
    public static class StartUp
    {
        private static List<Action> _startUpQuests = new List<Action>();


        /// <summary>
        /// 添加要在资源加载完毕时执行的操作
        /// </summary>
        /// <param name="action">资源加载完毕时执行的操作</param>
        public static void AddQuest(Action action)
        {
            if (_startUpQuests is null)
            {
                return;
            }

            void WrappedAction()
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    "NebulaeFlood's Lib".Error(
                        $"An exception occured when try finish quest from {action.Method.DeclaringType}.{action.Method.Name}. ---> {e}");
                }
            }

            _startUpQuests.Add(WrappedAction);
        }


        internal static void FinishQuests()
        {
            _startUpQuests.ForEach(x => Task.Run(x));
            _startUpQuests.Clear();
            _startUpQuests = null;
        }
    }
}
