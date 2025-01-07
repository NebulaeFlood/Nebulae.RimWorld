using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Windows;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 使用 <see cref="Control"/> 作为设置窗口的内容的 Mod 基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NebulaeMod<T> : NebulaeModBase where T : ModSettings, new()
    {
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

            if (_settings is NotifiableModSettings notifiableSettings)
            {
                notifiableSettings.updated.Invoke(this, notifiableSettings);
            }
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
