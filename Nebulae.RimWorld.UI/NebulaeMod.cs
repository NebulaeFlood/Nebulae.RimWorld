using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Windows;
using Nebulae.RimWorld.WeakEventManagers;
using System;
using Verse;

namespace Nebulae.RimWorld.UI
{
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
    /// 使用 <see cref="Control"/> 作为设置窗口的内容的 Mod 基类
    /// </summary>
    /// <typeparam name="T">Mod 设置类的类型</typeparam>
    public abstract class NebulaeMod<T> : NebulaeModBase where T : ModSettings, new()
    {
        private static T _settings;


        /// <summary>
        /// Mod 设置事件管理器
        /// </summary>
        public static readonly ModSettingsEventManager SettingsEvent = new ModSettingsEventManager();

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
            SettingsEvent.Invoke(this, _settings);
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
            public void Invoke(Mod mod, T settings)
            {
                Purge();

                Subscribers.ForEach(x =>
                {
                    if (x.TryGetTarget(out var listener))
                    {
                        listener.HandleModSettingsUpdated(mod, settings);
                    }
                });
            }
        }
    }
}
