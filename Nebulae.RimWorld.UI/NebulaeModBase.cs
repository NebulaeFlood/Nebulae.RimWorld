using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.UI.Windows;
using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 使用 <see cref="Control"/> 作为设置窗口的内容的 Mod 基类
    /// </summary>
    public abstract class NebulaeModBase : Mod, IUIEventListener
    {
        private ModSettingWindow _settingWindow;


        /// <summary>
        /// 显示在 Mod 选项的标签名称
        /// </summary>
        public abstract string CategoryLabel { get; }

        /// <summary>
        /// Mod 设置窗口
        /// </summary>
        public ModSettingWindow SettingWindow
        {
            get
            {
                if (_settingWindow is null)
                {
                    throw new InvalidOperationException($"Trying to get mod setting window of {this} before it is created.");
                }

                return _settingWindow;
            }
        }


        /// <summary>
        /// 为 <see cref="NebulaeModBase"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="content">Mod 内容</param>
        protected NebulaeModBase(ModContentPack content) : base(content)
        {
            StartUpQuestManager.AddQuest(() => _settingWindow = CreateSettingWindow());

            UIPatch.UIEvent.Manage(this);
        }


        /// <summary>
        /// 创建 Mod 设置窗口
        /// </summary>
        /// <returns>Mod 设置窗口</returns>
        public abstract ModSettingWindow CreateSettingWindow();

        /// <summary>
        /// 在窗口控件上方绘制的内容
        /// </summary>
        /// <param name="clientRect">允许绘制的区域</param>
        public override void DoSettingsWindowContents(Rect clientRect) { }

        /// <summary>
        /// 显示在 Mod 选项的标签名称
        /// </summary>
        public sealed override string SettingsCategory() => CategoryLabel;

        /// <inheritdoc/>
        public void UIEventHandler(UIEventType type)
        {
            if (type is UIEventType.LanguageChanged)
            {
                _settingWindow.Content?.Unbind();
                // 使窗口在下次打开时重新生成，完成本地化
                _settingWindow = CreateSettingWindow();
            }
        }
    }
}
