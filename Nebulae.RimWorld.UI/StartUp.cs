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
        /// 此动态链接库的名称
        /// </summary>
        public const string Lib = "NebulaeFlood's Lib";


        /// <summary>
        /// 添加要在资源加载完毕时执行的初始化任务
        /// </summary>
        /// <param name="action">初始化方法</param>
        /// <param name="mod">添加任务的 Mod</param>
        /// <param name="questName">任务名</param>
        public static void AddQuest(Action action, ModContentPack mod, string questName)
        {
            if (_startUpQuests is null)
            {
                return;
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (mod is null)
            {
                throw new ArgumentNullException(nameof(mod));
            }

            if (string.IsNullOrWhiteSpace(questName))
            {
                throw new InvalidOperationException("Quest name cannot be null or white space.");
            }

            _startUpQuests.Add(new Task(new Quest(action, mod, questName).Start));
        }


        internal static void FinishQuests()
        {
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
            string color = isFailed ? "orange" : "cyan";
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
                LogQuestFinished(Lib, "Load Default Converters", MethodBase.GetCurrentMethod());
            }
            catch (Exception e)
            {
                LogQuestFailed(Lib, "Load Default Converters", e);
            }
        }

        private static void LogQuestFailed(string modName, string questName, Exception e)
        {
            string message = BuildLogMessage(isFailed: true, modName, questName, additionalInfo: e.ToString());
            Lib.Error(message);
        }

        private static void LogQuestFinished(string modName, string questName, MethodBase method)
        {
            string additionalInfo = $"{method.DeclaringType}.{method.Name}.";
            string message = BuildLogMessage(isFailed: false, modName, questName, additionalInfo);
            Lib.Succeed(message);
        }

        #endregion


        private static List<Task> _startUpQuests = new List<Task>();


        private readonly struct Quest
        {
            //------------------------------------------------------
            //
            //  Public Fields
            //
            //------------------------------------------------------

            #region Public Fields

            public readonly Action Action;
            public readonly string ModName;
            public readonly string QuestName;

            #endregion


            public Quest(Action action, ModContentPack mod, string questName)
            {
                Action = action;
                ModName = mod.Name;
                QuestName = questName;
            }


            public void Start()
            {
                try
                {
                    Action.Invoke();
                    LogQuestFinished(ModName, QuestName, Action.Method);
                }
                catch (Exception e)
                {
                    LogQuestFailed(ModName, QuestName, e);
                }
            }
        }
    }
}
