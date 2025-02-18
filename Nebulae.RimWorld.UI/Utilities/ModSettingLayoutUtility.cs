using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Converters;
using Nebulae.RimWorld.UI.Data.Binding;
using System;
using System.Reflection;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// Mod 设置页面布局帮助类
    /// </summary>
    public static class ModSettingLayoutUtility
    {
        /// <summary>
        /// 为 <see cref="bool"/> 类型的设置选项创建一个 <see cref="CheckBox"/>
        /// </summary>
        /// <typeparam name="T">Mod 设置的类型</typeparam>
        /// <param name="settings">Mod 设置类的实例</param>
        /// <param name="entryName">设置选项对应的字段名</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <returns>指定条目对应的 <see cref="CheckBox"/> 的实例。</returns>
        /// <exception cref="MissingFieldException">当无法在 <typeparamref name="T"/> 中找到名为 <paramref name="entryName"/> 的字段时发生。</exception>
        public static CheckBox CreateBooleanEntry<T>(T settings, string entryName, string label, string tooltip) where T : ModSettings
        {
            if (typeof(T).GetField(entryName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                is FieldInfo field)
            {
                bool value = field.GetValue(settings) is true;

                var checkBox = new CheckBox
                {
                    Status = value ? ToggleStatus.Checked : ToggleStatus.Unchecked,
                    Text = label,
                    Tooltip = tooltip,
                    ShowTooltip = string.IsNullOrEmpty(tooltip)
                };

                BindingManager.Bind(
                    checkBox,
                    ToggleButton.StatusProperty,
                    settings,
                    entryName,
                    ToggleStatusConverter.Instance,
                    BindingMode.OneWay);

                return checkBox;
            }
            else
            {
                throw new MissingFieldException(typeof(T).Name, entryName);
            }
        }

        /// <summary>
        /// 为 <see cref="float"/> 类型的设置选项创建一个带有输入和滑动两种修改方式的布局
        /// </summary>
        /// <typeparam name="T">Mod 设置的类型</typeparam>
        /// <param name="settings">Mod 设置类的实例</param>
        /// <param name="entryName">设置选项对应的字段名</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="minValue">选项的最小值</param>
        /// <param name="maxValue">选项的最大值</param>
        /// <param name="decimalPartDigit">小数部分的位数</param>
        /// <param name="silderStep">最小的相邻可选值的差值</param>
        /// <returns>指定条目对应的带有输入和滑动两种修改方式的布局。</returns>
        /// <exception cref="MissingFieldException">当无法在 <typeparamref name="T"/> 中找到名为 <paramref name="entryName"/> 的字段时发生。</exception>
        public static Grid CreateNumberEntry<T>(T settings, string entryName, string label, string tooltip, float minValue, float maxValue, int decimalPartDigit, float silderStep) where T : ModSettings
        {
            if (typeof(T).GetField(entryName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                is FieldInfo field)
            {
                bool showTooltip = string.IsNullOrEmpty(tooltip);
                float value = (float)field.GetValue(settings);

                var entryLabel = new Label
                {
                    Anchor = TextAnchor.MiddleLeft,
                    Margin = new Thickness(0f, 8f, 0f, 0f),
                    Text = label,
                    Tooltip = tooltip,
                    ShowTooltip = showTooltip
                };

                var numberBox = new NumberBox
                {
                    Margin = new Thickness(0f, 8f, 0f, 0f),
                    DecimalPartDigit = decimalPartDigit,
                    Maximum = maxValue,
                    Minimum = minValue,
                    Value = value,
                    Tooltip = tooltip,
                    ShowTooltip = showTooltip
                };

                var slider = new Slider
                {
                    Maximum = maxValue,
                    Minimum = minValue,
                    Step = silderStep,
                    Value = value,
                    Tooltip = tooltip,
                    ShowTooltip = showTooltip
                };

                BindingManager.Bind(
                    slider,
                    Slider.ValueProperty,
                    numberBox,
                    NumberBox.ValueProperty,
                    BindingMode.TwoWay);

                BindingManager.Bind(
                    slider,
                    Slider.ValueProperty,
                    settings,
                    entryName,
                    BindingMode.OneWay);

                return new Grid().SetSize(new float[] { Grid.Remain, 100f }, new float[] { 32f, 16f })
                    .Set(
                        entryLabel, numberBox,
                        slider, slider
                    );
            }
            else
            {
                throw new MissingFieldException(typeof(T).Name, entryName);
            }
        }
    }
}
