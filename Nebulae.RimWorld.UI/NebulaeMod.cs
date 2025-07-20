using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Windows;
using System;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// NebulaeFlood's Lib 可修改设置窗口的对 <see cref="Mod"/> 的实现
    /// </summary>
    /// <typeparam name="T">Mod 设置类型</typeparam>
    public abstract class NebulaeMod<T> : NebulaeModBase where T : NebulaeModSettings<T>, new()
    {
        //------------------------------------------------------
        //
        //  Public Static Properties
        //
        //------------------------------------------------------

        #region Public Static Properties

        /// <summary>
        /// 获取当前 Mod 的设置内容
        /// </summary>
        public static T Settings => _settings;

        /// <summary>
        /// 获取当前 Mod 的 <see cref="System.Type"/>
        /// </summary>
        public static Type Type => _type;

        /// <summary>
        /// 获取当前 Mod 的设置窗口
        /// </summary>
        public static ModSettingWindow Window => _settingWindow;

        #endregion


        /// <summary>
        /// 为 <see cref="NebulaeMod{T}"/> 及其派生类实现基本初始化
        /// </summary>
        /// <param name="content">Mod 内容</param>
        protected NebulaeMod(ModContentPack content) : base(content)
        {
            _mod = this;
            _settings = GetSettings<T>();
            _type = GetType();

            if (_settings is null)
            {
                return;
            }

            _settings.OnCheckIntegrity();

            StartUp.AddQuest(Initialize, content, $"Initialize Setting Window");
        }


        /// <summary>
        /// 初始化设置窗口
        /// </summary>
        public static void ResetSettingWindow()
        {
            _settingWindow.Content = _mod.CreateContent();
        }


        /// <summary>
        /// 保存设置内容
        /// </summary>
        public override void WriteSettings()
        {
            base.WriteSettings();
            _settings.savedEvent.Invoke(_settings, EventArgs.Empty);
        }


        /// <inheritdoc/>
        protected internal override sealed ModSettingWindow GetSettingWindow() => _settingWindow;


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 创建 Mod 设置窗口的内容
        /// </summary>
        /// <returns>Mod 设置窗口的内容</returns>
        protected abstract Control CreateContent();

        /// <summary>
        /// Mod 设置窗口创建后执行的方法
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// Mod 设置窗口即将创建时执行的方法
        /// </summary>
        protected virtual void OnInitializing() { }

        #endregion


        private void Initialize()
        {
            OnInitializing();

            _settingWindow = new ModSettingWindow(this)
            {
                Content = CreateContent()
            };

            OnInitialized();
        }


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static NebulaeMod<T> _mod;
        private static T _settings;
        private static ModSettingWindow _settingWindow;
        private static Type _type;

        #endregion
    }
}
