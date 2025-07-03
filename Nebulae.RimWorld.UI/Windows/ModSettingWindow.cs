using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 使用 <see cref="Control"/> 进行内容呈现的 Mod 设置窗口
    /// </summary>
    public class ModSettingWindow : ControlWindow
    {
        /// <summary>
        /// 与窗口关联的 Mod
        /// </summary>
        public readonly Mod AssociatedMod;


        /// <summary>
        /// 初始化 <see cref="ModSettingWindow"/> 的新实例
        /// </summary>
        /// <param name="associatedMod">与窗口关联的 Mod</param>
        public ModSettingWindow(Mod associatedMod)
        {
            AssociatedMod = associatedMod;

            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseButton = true;
            doCloseX = true;
            forcePause = true;
        }


        /// <summary>
        /// 当窗口将要关闭时执行的操作
        /// </summary>
        public sealed override void PreClose() => AssociatedMod.WriteSettings();


        /// <summary>
        /// 在 <see cref="ControlWindow.Content"/> 上方进行绘制
        /// </summary>
        /// <param name="inRect">允许绘制的区域</param>
        protected override void LateWindowOnGUI(Rect inRect)
        {
            AssociatedMod.DoSettingsWindowContents(inRect);
        }
    }
}
