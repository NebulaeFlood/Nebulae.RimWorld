using Nebulae.RimWorld.UI.Windows;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// NebulaeFlood's Lib 对 <see cref="Mod"/> 的基础实现
    /// </summary>
    public abstract class NebulaeModBase : Mod
    {
        static NebulaeModBase()
        {
            UIPatch.PatchAll();
        }


        /// <summary>
        /// 为 <see cref="NebulaeModBase"/> 及其派生类实现基本初始化
        /// </summary>
        /// <param name="content">Mod 内容</param>
        protected NebulaeModBase(ModContentPack content) : base(content) { }


        /// <summary>
        /// 获取当前 Mod 的设置选项名
        /// </summary>
        /// <returns>当前 Mod 的设置选项名。</returns>
        public override string SettingsCategory() => Content.Name;


        /// <summary>
        /// 获取当前 Mod 的设置窗口
        /// </summary>
        /// <returns>当前 Mod 的设置窗口。</returns>
        protected internal abstract ModSettingWindow GetSettingWindow();
    }
}
