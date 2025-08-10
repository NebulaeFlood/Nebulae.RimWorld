using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Converters;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Data.Bindings;
using Nebulae.RimWorld.Utilities;
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


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

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

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交转译信息
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="succeeded">是否转译成功</param>
        /// <param name="type">方法被转译的类</param>
        /// <param name="methodName">转译方法的名称</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void TranspileMessage(this string logLabel, bool succeeded, Type type, string methodName, string color = "3F48CCFF")
        {
            if (succeeded)
            {
                logLabel.Succeed($"Succeeded to transpile method.\n---> <color=cyan>{type.FullName}.{methodName}</color>", color);
            }
            else
            {
                logLabel.Error($"Failed to transpile method.\n---> <color=cyan>{type.FullName}.{methodName}</color>", color);
            }
        }

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交转译信息
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="succeeded">是否转译成功</param>
        /// <param name="typeFullName">方法被转译的类的全名</param>
        /// <param name="methodName">转译方法的名称</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void TranspileMessage(this string logLabel, bool succeeded, string typeFullName, string methodName, string color = "3F48CCFF")
        {
            if (succeeded)
            {
                logLabel.Succeed($"Succeeded to transpile method.\n---> <color=cyan>{typeFullName}.{methodName}</color>", color);
            }
            else
            {
                logLabel.Error($"Failed to transpile method.\n---> <color=cyan>{typeFullName}.{methodName}</color>", color);
            }
        }

        #endregion


        internal static void FinishQuests()
        {
            if (_startUpQuests is null)
            {
                return;
            }

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

#if DEBUG
            Utilities.UIUtility.DebugMode = true;

            foreach (var type in GenTypes.AllSubclasses(typeof(DependencyObject)))
            {
                try
                {
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
                catch (Exception e)
                {
                    throw new TypeInitializationException(type.FullName, e);
                }
            }
#endif
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
