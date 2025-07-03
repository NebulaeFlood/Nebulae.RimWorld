using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Converters;
using Nebulae.RimWorld.UI.Core.Data.Bindings;
using Nebulae.RimWorld.Utilities;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Reflection;
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
        /// <summary>
        /// 添加要在资源加载完毕时执行的初始化任务
        /// </summary>
        /// <param name="action">初始化方法</param>
        /// <param name="mod">添加任务的 Mod</param>
        /// <param name="questName">任务名</param>
        public static void AddQuest(Action action, ModContentPack mod, string questName = null)
        {
            if (_startUpQuests is null)
            {
                return;
            }

            if (mod is null)
            {
                throw new ArgumentNullException(nameof(mod));
            }

            void WrappedAction()
            {
                try
                {
                    action.Invoke();
                    LogQuestFinished(mod.Name, questName, action.Method);
                }
                catch (Exception e)
                {
                    LogQuestFailed(mod.Name, questName, e);
                }
            }

            _startUpQuests.Add(new Task(WrappedAction));
        }


        internal static void FinishQuests()
        {
            // foreach (var item in typeof(DependencyObject).AllLeafSubclasses())
            // {
            //     RuntimeHelpers.RunClassConstructor(item.TypeHandle);
            // }

            _startUpQuests.Add(new Task(LoadDefaultConverters));

            var mre = new ManualResetEvent(false);

            async void FinishQuestsAsync()
            {
                _startUpQuests.ForEach(x => x.Start());

                await Task.WhenAll(_startUpQuests);

                mre.Set();
                mre.Dispose();
            }

            Task.Run(FinishQuestsAsync);

            mre.WaitOne();

            _startUpQuests.Clear();
            _startUpQuests = null;
        }


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static string BuildLogMessage(bool isFailed, string title, string questName, string additionalInfo)
        {
            string color = isFailed ? "orange" : "green";
            string questPart = string.IsNullOrEmpty(questName)
                ? $"from <color={color}><{title}></color>"
                : $"named <color={color}><{title}>[{questName}]</color>";

            string status = isFailed ? "occurred an exception." : "finished.";
            return $"A start up quest {questPart} {status}\n---> {additionalInfo}";
        }

        private static void LoadDefaultConverters()
        {
            try
            {
                BindingBase.AddDefaultConverter(typeof(ToggleState), typeof(bool), ToggleStateToBoolean.Instance);
                BindingBase.AddDefaultConverter(typeof(ToggleState), typeof(Visibility), ToggleStateToVisibility.Instance);
                LogQuestFinished("NebulaeFlood's Lib", "Load Default Converters", MethodBase.GetCurrentMethod());
            }
            catch (Exception e)
            {
                LogQuestFailed("NebulaeFlood's Lib", "Load Default Converters", e);
            }
        }

        private static void LogQuestFailed(string modName, string questName, Exception e)
        {
            string message = BuildLogMessage(isFailed: true, modName, questName, additionalInfo: e.ToString());
            "NebulaeFlood's Lib".Error(message);
        }

        private static void LogQuestFinished(string modName, string questName, MethodBase method)
        {
            string additionalInfo = $"{method.DeclaringType}.{method.Name}.";
            string message = BuildLogMessage(isFailed: false, modName, questName, additionalInfo);
            "NebulaeFlood's Lib".Message(message);
        }

        #endregion


        private static List<Task> _startUpQuests = new List<Task>();
    }
}
