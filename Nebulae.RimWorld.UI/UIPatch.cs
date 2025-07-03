using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 原版 UI 补丁
    /// </summary>
    public static class UIPatch
    {
        /// <summary>
        /// <see cref="Harmony"/> 的唯一标识符
        /// </summary>
        public const string UniqueId = "Nebulae.RimWorld.UI";


        #region ScaleChanged
        internal static readonly WeakEvent<Harmony, ScaleChangedEventArgs> ScaleChangedEvent = new WeakEvent<Harmony, ScaleChangedEventArgs>();

        /// <summary>
        /// UI 缩放比例改变时发生
        /// </summary>
        public static event ScaleChangedEventHandler ScaleChanged
        {
            add => ScaleChangedEvent.AddHandler(value);
            remove => ScaleChangedEvent.RemoveHandler(value);
        }
        #endregion


        /// <summary>
        /// <see cref="Harmony"/> 实例
        /// </summary>
        public static readonly Harmony HarmonyInstance = new Harmony(UniqueId);


        internal static void PatchAll()
        {
            HarmonyInstance.PatchAll();
            HarmonyInstance.Patch(AccessTools.Method(typeof(Root), nameof(Root.OnGUI)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Dispatcher), nameof(Dispatcher.Update))));
        }
    }


    /// <summary>
    /// <see cref="UIPatch.ScaleChanged"/> 的事件处理程序
    /// </summary>
    /// <param name="sender">发起事件的 <see cref="Harmony"/></param>
    /// <param name="args">事件数据</param>
    public delegate void ScaleChangedEventHandler(Harmony sender, ScaleChangedEventArgs args);


    /// <summary>
    /// <see cref="UIPatch.ScaleChanged"/> 的事件数据
    /// </summary>
    public sealed class ScaleChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 新的 UI 缩放比例
        /// </summary>
        public readonly float NewScale;


        /// <summary>
        /// 初始化 <see cref="ScaleChangedEventArgs"/> 的新实例
        /// </summary>
        /// <param name="newScale">新的 UI 缩放比例</param>
        public ScaleChangedEventArgs(float newScale)
        {
            NewScale = newScale;
        }
    }
}
