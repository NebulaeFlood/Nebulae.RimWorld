using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Windows;
using System;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 使用 <see cref="Control"/> 作为设置窗口的内容的 Mod 基类
    /// </summary>
    /// <typeparam name="T">Mod 设置类</typeparam>
    public abstract class NebulaeMod<T> : NebulaeModBase where T : ModSettings, new()
    {
        #region SettingsUpdated

        private static readonly WeakEvent<Mod, T> _settingsUpdated = new WeakEvent<Mod, T>();

        /// <summary>
        /// 当 Mod 设置更新时触发的弱事件
        /// </summary>
        public static event Action<Mod, T> SettingsUpdated
        {
            add => _settingsUpdated.Add(value, value.Invoke);
            remove => _settingsUpdated.Remove(value);
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
        public sealed override void WriteSettings()
        {
            base.WriteSettings();
            _settingsUpdated.Invoke(this, _settings);
        }


        /// <inheritdoc/>
        public sealed override ModSettingWindow CreateSettingWindow()
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
