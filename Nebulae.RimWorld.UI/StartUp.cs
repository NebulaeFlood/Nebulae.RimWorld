using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Converters;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Data.Binding;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 初始化任务管理器
    /// </summary>
    public static class StartUp
    {
        private static List<Task> _startUpQuests = new List<Task>();


        static StartUp()
        {
            AddQuest(() =>
            {
                BindingManager.AddDefaultConverter(ToggleStatusConverter.Instance, typeof(ToggleStatus), typeof(bool));
                BindingManager.AddDefaultConverter(VisibilityConverter.Instance, typeof(ToggleStatus), typeof(Visibility));
            }, typeof(StartUp), "Init Value Converters");
        }


        /// <summary>
        /// 添加要在资源加载完毕时执行的初始化任务
        /// </summary>
        /// <param name="action">初始化方法</param>
        /// <param name="owner">拥有任务的类型</param>
        /// <param name="questName">任务名</param>
        public static void AddQuest(Action action, Type owner, string questName = null)
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
                    LogQuestFinished(owner, questName, action.Method);
                }
                catch (Exception e)
                {
                    LogQuestFailed(owner, questName, e);
                }
            }

            _startUpQuests.Add(new Task(WrappedAction));
        }

        /// <summary>
        /// 添加要在资源加载完毕时执行的初始化任务
        /// </summary>
        /// <param name="action">初始化方法</param>
        /// <param name="owner">拥有任务的对象</param>
        /// <param name="questName">任务名</param>
        public static void AddQuest(Action action, object owner, string questName = null)
        {
            if (owner is null)
            {
                throw new ArgumentNullException("owner");
            }

            AddQuest(action, owner.GetType(), questName);
        }


        internal static void FinishQuests()
        {
            foreach (var item in typeof(DependencyObject).AllLeafSubclasses())
            {
                RuntimeHelpers.RunClassConstructor(item.TypeHandle);
            }

            var mre = new ManualResetEvent(false);

            async void FinishQuestsAsync()
            {
                try
                {
                    _startUpQuests.ForEach(x =>
                    {
                        x.ConfigureAwait(false);
                        x.Start();
                    });

                    await Task.WhenAll(_startUpQuests.ToArray()).ConfigureAwait(false);
                }
                finally
                {
                    mre.Set();
                }
            }

            Task.Run(FinishQuestsAsync);

            mre.WaitOne();
            mre.Dispose();

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

        private static void LogQuestFailed(Type owner, string questName, Exception e)
        {
            string message = BuildLogMessage(isFailed: true, UILogUtility.GetAssemblyTitle(owner), questName, additionalInfo: e.ToString());
            "NebulaeFlood's Lib".Error(message);
        }

        private static void LogQuestFinished(Type owner, string questName, MethodInfo method)
        {
            string additionalInfo = $"{method.DeclaringType}.{method.Name}.";
            string message = BuildLogMessage(isFailed: false, UILogUtility.GetAssemblyTitle(owner), questName, additionalInfo);
            "NebulaeFlood's Lib".Message(message);
        }
    }
}
