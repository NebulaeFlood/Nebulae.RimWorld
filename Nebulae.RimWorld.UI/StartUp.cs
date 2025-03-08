using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 初始化任务管理器
    /// </summary>
    public static class StartUp
    {
        private static List<Action> _finalQuests = new List<Action>();
        private static List<Action> _startUpQuests = new List<Action>();


        /// <summary>
        /// 添加要在初始化任务完成后的任务
        /// </summary>
        /// <param name="action">初始化任务完成后的任务</param>
        public static void AddFinalQuest(Action action)
        {
            if (_finalQuests is null)
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
                        $"An exception occured when try finish a final quest from {action.Method.DeclaringType}.{action.Method.Name}.\n---> {e}");
                }
            }

            _finalQuests.Add(WrappedAction);
        }

        /// <summary>
        /// 添加要在资源加载完毕时执行的初始化任务
        /// </summary>
        /// <param name="action">资源加载完毕时执行的初始化任务</param>
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
                        $"An exception occured when try finish a initialization quest from {action.Method.DeclaringType}.{action.Method.Name}.\n---> {e}");
                }
            }

            _startUpQuests.Add(WrappedAction);
        }




        internal static async void FinishQuestsAsync()
        {
            await Task.WhenAll(_startUpQuests.Select(x => Task.Run(x)).ToArray());

            _startUpQuests.Clear();
            _startUpQuests = null;

            _finalQuests.Select(x => Task.Run(x));
            _finalQuests.Clear();
            _finalQuests = null;
        }
    }
}
