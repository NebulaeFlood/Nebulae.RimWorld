using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 使用 <see cref="Control"/> 作为设置窗口的内容的 Mod 基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NebulaeMod<T> : NebulaeModBase where T : ModSettings, new()
    {
        #region SettingUpdated

        private readonly WeakEvent<Mod, T> _settingUpdated = new WeakEvent<Mod, T>();

        /// <summary>
        /// 当 Mod 设置更新时触发的弱事件
        /// </summary>
        public event WeakEventHandler<Mod, T> SettingUpdated
        {
            add => _settingUpdated.Add(value);
            remove => _settingUpdated.Remove(value);
        }

        #endregion


        private static T _settings;


        /// <summary>
        /// Mod 设置的数据
        /// </summary>
        public static T Settings => _settings;


        /// <summary>
        /// 为 <see cref="NebulaeMod{T}"/> 及其派生类实现基本初始化
        /// </summary>
        /// <param name="content">Mod 内容</param>
        protected NebulaeMod(ModContentPack content) : base(content)
        {
            _settings = GetSettings<T>();
        }


        /// <summary>
        /// 保存 Mos 设置
        /// </summary>
        public override sealed void WriteSettings()
        {
            base.WriteSettings();
            _settingUpdated.Invoke(this, Settings);
        }


        /// <inheritdoc/>
        public override ModSettingWindow CreateSettingWindow()
        {
            return new ModSettingWindow(this) { Content = CreateContent() };
        }


        /// <summary>
        /// 创建 Mod 设置窗口的内容
        /// </summary>
        /// <returns>Mod 设置窗口的内容</returns>
        protected abstract Control CreateContent();
    }
}
