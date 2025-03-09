using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 初始化任务管理器
    /// </summary>
    public static class StartUp
    {
        private static List<Task> _startUpQuests = new List<Task>();


        /// <summary>
        /// 添加要在资源加载完毕时执行的初始化任务
        /// </summary>
        /// <param name="action">初始化方法</param>
        /// <param name="name">任务名</param>
        /// <param name="callBack">回调函数</param>
        public static void AddQuest(Action action, string name = null, Action callBack = null)
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
                    callBack?.Invoke();
                    LogQuestFinished(action, name);
                }
                catch (Exception e)
                {
                    LogQuestFailed(action, name, e);
                }
            }

            _startUpQuests.Add(new Task(WrappedAction));
        }


        internal static void FinishQuestsAsync()
        {
            _startUpQuests.ForEach(x => x.Start());

            Task.WhenAll(_startUpQuests.ToArray());

            _startUpQuests.Clear();
            _startUpQuests = null;
        }


        private static string BuildLogMessage(bool isFailed, string title, string questName, string additionalInfo)
        {
            string color = isFailed ? "orange" : "green";
            string questPart = string.IsNullOrEmpty(questName)
                ? $"from <color={color}><{title}></color>"
                : $"named <color={color}><{title}>[{questName}]</color>";

            string status = isFailed ? "occurred an exception." : "finished.";
            return $"A start up quest {questPart} {status}\n---> {additionalInfo}";
        }

        private static void LogQuestFailed(Delegate action, string questName, Exception e)
        {
            string title = UILogUtility.GetAssemblyTitle(action.Target);
            string message = BuildLogMessage(isFailed: true, title, questName, additionalInfo: e.ToString());
            "NebulaeFlood's Lib".Error(message);
        }

        private static void LogQuestFinished(Delegate action, string questName)
        {
            var method = action.Method;

            string title = UILogUtility.GetAssemblyTitle(action.Target);
            string additionalInfo = $"{method.DeclaringType}.{method.Name}.";
            string message = BuildLogMessage(isFailed: false, title, questName, additionalInfo);
            "NebulaeFlood's Lib".Message(message);
        }
    }
}
