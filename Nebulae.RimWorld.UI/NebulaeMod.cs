using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.UI.Windows;
using Nebulae.RimWorld.WeakEventManagers;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 定义一个使用 <see cref="Visual"/> 作为设置窗口的 Mod
    /// </summary>
    public interface INebulaeMod
    {
        /// <summary>
        /// 获取设置窗口
        /// </summary>
        /// <returns>Mod 的设置窗口</returns>
        ModSettingWindow GetSettingWindow();
    }

    /// <summary>
    /// 定义一个 Mod 设置事件的订阅者
    /// </summary>
    /// <typeparam name="T">Mod 设置类的类型</typeparam>
    public interface IModSettingsEventListener<T>
    {
        /// <summary>
        /// 当 Mod 设置更新时调用的方法
        /// </summary>
        /// <param name="mod">设置被更新的 Mod</param>
        /// <param name="settings">新的设置内容</param>
        void HandleModSettingsUpdated(Mod mod, T settings);
    }


    /// <summary>
    /// 使用 <see cref="Visual"/> 作为设置窗口的内容的 Mod 基类
    /// </summary>
    /// <typeparam name="T">Mod 设置类的类型</typeparam>
    public abstract class NebulaeMod<T> : Mod, INebulaeMod, IUIEventListener
        where T : ModSettings, new()
    {
        private static T _settings;
        private static ModSettingWindow _settingWindow;


        /// <summary>
        /// 显示在 Mod 选项的标签名称
        /// </summary>
        public abstract string CategoryLabel { get; }

        /// <summary>
        /// Mod 设置事件管理器
        /// </summary>
        public static readonly ModSettingsEventManager SettingsEvent = new ModSettingsEventManager();

        /// <summary>
        /// Mod 设置的数据
        /// </summary>
        public static T Settings => _settings;

        /// <summary>
        /// Mod 设置窗口
        /// </summary>
        public static ModSettingWindow SettingWindow => _settingWindow;


        /// <summary>
        /// 为 <see cref="NebulaeMod{T}"/> 及其派生类实现基本初始化
        /// </summary>
        /// <param name="content">Mod 内容</param>
        protected NebulaeMod(ModContentPack content) : base(content)
        {
            _settings = GetSettings<T>();

            StartUp.AddQuest(Initialize);
            UIPatch.UIEvent.Manage(this);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 在窗口控件上方绘制的内容
        /// </summary>
        /// <param name="clientRect">允许绘制的区域</param>
        public override void DoSettingsWindowContents(Rect clientRect) { }

        /// <inheritdoc/>
        public ModSettingWindow GetSettingWindow() => _settingWindow;

        /// <inheritdoc/>
        public virtual void HandleUIEvent(UIEventType type)
        {
            if (type is UIEventType.LanguageChanged)
            {
                Initialize();
            }
        }

        /// <summary>
        /// 初始化设置窗口
        /// </summary>
        public void Initialize()
        {
            OnInitializing();

            _settingWindow?.Unbind();
            _settingWindow = CreateSettingWindow();
            _settingWindow.Content = CreateContent();
        }

        /// <summary>
        /// 显示在 Mod 选项的标签名称
        /// </summary>
        public sealed override string SettingsCategory() => CategoryLabel;

        /// <summary>
        /// 保存 Mos 设置
        /// </summary>
        public sealed override void WriteSettings()
        {
            base.WriteSettings();
            SettingsEvent.Invoke(this, _settings);
        }

        #endregion


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
        protected abstract Visual CreateContent();

        /// <summary>
        /// 创建 Mod 设置窗口
        /// </summary>
        /// <returns>Mod 设置窗口</returns>
        protected virtual ModSettingWindow CreateSettingWindow() => new ModSettingWindow(this);

        /// <summary>
        /// 创建 Mod 设置窗口前执行的方法
        /// </summary>
        protected virtual void OnInitializing()
        {
        }

        #endregion


        /// <summary>
        /// Mod 设置事件管理器
        /// </summary>
        public sealed class ModSettingsEventManager : WeakEventManager<IModSettingsEventListener<T>>
        {
            internal ModSettingsEventManager()
            {
            }

            /// <summary>
            /// 调用所有订阅者的事件处理方法
            /// </summary>
            /// <param name="mod">设置被更新的 Mod</param>
            /// <param name="settings">新的设置内容</param>
            internal void Invoke(Mod mod, T settings)
            {
                var subscribers = GetSubcribers();

                for (int i = subscribers.Count - 1; i >= 0; i--)
                {
                    subscribers[i].HandleModSettingsUpdated(mod, settings);
                }
            }
        }
    }
}
