using Nebulae.RimWorld.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 使用 <see cref="Control"/> 进行内容呈现的 Mod 设置窗口的基类
    /// </summary>
    public class ModSettingWindow : ControlWindow
    {
        /// <summary>
        /// 与窗口关联的 Mod
        /// </summary>
        public Mod AssociatedMod { get; internal set; }


        /// <summary>
        /// 初始化 <see cref="ModSettingWindow"/> 的新实例
        /// </summary>
        /// <param name="associatedMod">与窗口关联的 Mod</param>
        public ModSettingWindow(Mod associatedMod)
        {
            AssociatedMod = associatedMod;

            forcePause = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
        }


        /// <summary>
        /// 当窗口将要关闭时执行的操作
        /// </summary>
        public sealed override void PreClose() => AssociatedMod.WriteSettings();
    }
}
