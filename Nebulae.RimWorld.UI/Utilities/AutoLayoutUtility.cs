using Nebulae.RimWorld.UI.Automation;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Converters;
using Nebulae.RimWorld.UI.Data.Binding;
using Nebulae.RimWorld.UI.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 自动布局帮助类
    /// </summary>
    public static class AutoLayoutUtility
    {
        /// <summary>
        /// 标准自动布局控件高度
        /// </summary>
        public const float StandardRowHeight = 34f;


        /// <summary>
        /// 为 <see cref="bool"/> 类型的设置选项创建一个 <see cref="CheckBox"/>
        /// </summary>
        /// <typeparam name="T">保存设置数据的类型</typeparam>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的字段的名称</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="height">选项的高度</param>
        /// <returns>指定条目对应的 <see cref="CheckBox"/> 的实例。</returns>
        public static CheckBox CreateBooleanEntry<T>(
            T settings,
            string entryName,
            string label,
            string tooltip,
            float height = StandardRowHeight)
        {
            return CreateBooleanEntry(
                settings,
                entryName,
                typeof(T).GetValue<bool>(entryName),
                label,
                tooltip,
                height);
        }

        /// <summary>
        /// 为 <see cref="bool"/> 类型的设置选项创建一个 <see cref="CheckBox"/>
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的字段的名称</param>
        /// <param name="entryValue">设置选项的当前值</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="height">选项的高度</param>
        /// <returns>指定条目对应的 <see cref="CheckBox"/> 的实例。</returns>
        public static CheckBox CreateBooleanEntry(
            object settings,
            string entryName,
            bool entryValue,
            string label,
            string tooltip,
            float height = StandardRowHeight)
        {
            var checkBox = new CheckBox
            {
                Height = height,
                Status = entryValue ? ToggleStatus.Checked : ToggleStatus.Unchecked,
                Text = label,
                Tooltip = tooltip
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

        /// <summary>
        /// 为预设的设置条目创建对应的控件
        /// </summary>
        /// <param name="settings">预设了条目的设置类</param>
        /// <param name="virtualize">使用虚拟化面板</param>
        /// <returns>如果 <paramref name="virtualize"/> 为 <see langword="true"/>，返回类型为 <see cref="VirtualizingStackPanel"/> 的包含了设置控件的面板控件；反之则返回类型为 <see cref="StackPanel"/> 的面板。</returns>
        public static Panel CreateEntries<T>(this T settings, bool virtualize)
        {
            Type type = typeof(T);
            var attribute = type.GetCustomAttribute<StandardTranslationKeyAttribute>(false)
                ?? throw new InvalidOperationException($"No entries are identified in {type}");

            FieldInfo[] fields = type.GetFields(
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var entries = new List<SettingEntry>();
            string standardKey = attribute.Key;

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                if (field.GetCustomAttribute<SettingEntryBaseAttribute>() is SettingEntryBaseAttribute entry)
                {
                    entries.Add(new SettingEntry(standardKey, entry, field, settings));
                }
            }

            var controls = new Visual[entries.Count];

            for (int i = 0; i < controls.Length; i++)
            {
                var entry = entries[i];

                switch (entry.EntryType)
                {
                    case SettingEntryType.Boolean:
                        controls[i] = CreateBooleanEntry(settings, entry);
                        continue;
                    case SettingEntryType.Number:
                        controls[i] = CreateNumberEntry(settings, entry);
                        continue;
                    case SettingEntryType.String:
                        controls[i] = CreateTextEntry(settings, entry);
                        continue;
                    default:
                        continue;
                }
            }

            if (virtualize)
            {
                return new VirtualizingStackPanel()
                    .Set(controls);
            }
            else
            {
                return new StackPanel()
                    .Set(controls);
            }
        }

        /// <summary>
        /// 为 <see cref="float"/> 类型的设置选项创建一个带有输入和滑动两种修改方式的布局
        /// </summary>
        /// <typeparam name="T">保存设置数据的类型</typeparam>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的字段的名称</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="minValue">选项的最小值</param>
        /// <param name="maxValue">选项的最大值</param>
        /// <param name="decimalPartDigit">小数部分的位数</param>
        /// <param name="silderStep">最小的相邻可选值的差值</param>
        /// <param name="displayAsPercent">是否以小数形式显示值</param>
        /// <returns>指定条目对应的带有输入和滑动两种修改方式的布局。</returns>
        public static Grid CreateNumberEntry<T>(
            T settings,
            string entryName,
            string label,
            string tooltip,
            float minValue,
            float maxValue,
            int decimalPartDigit,
            float silderStep,
            bool displayAsPercent = false)
        {
            return CreateNumberEntry(
                settings,
                entryName,
                typeof(T).GetValue<float>(entryName, settings),
                label,
                tooltip,
                minValue,
                maxValue,
                decimalPartDigit,
                silderStep,
                displayAsPercent);
        }

        /// <summary>
        /// 为 <see cref="float"/> 类型的设置选项创建一个带有输入和滑动两种修改方式的布局
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的字段的名称</param>
        /// <param name="entryValue">设置选项的当前值</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="minValue">选项的最小值</param>
        /// <param name="maxValue">选项的最大值</param>
        /// <param name="decimalPartDigit">小数部分的位数</param>
        /// <param name="silderStep">最小的相邻可选值的差值</param>
        /// <param name="displayAsPercent">是否以小数形式显示值</param>
        /// <returns>指定条目对应的带有输入和滑动两种修改方式的布局。</returns>
        public static Grid CreateNumberEntry(
            object settings,
            string entryName,
            float entryValue,
            string label,
            string tooltip,
            float minValue,
            float maxValue,
            int decimalPartDigit,
            float silderStep,
            bool displayAsPercent = false)
        {
            var entryLabel = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Text = label,
                Tooltip = tooltip
            };

            var numberBox = new NumberBox
            {
                DecimalPartDigit = decimalPartDigit,
                DisplayAsPercent = displayAsPercent,
                Maximum = maxValue,
                Minimum = minValue,
                Tooltip = tooltip,
                Value = entryValue
            };

            var slider = new Slider
            {
                DrawExtremeValues = true,
                DisplayAsPercent = displayAsPercent,
                Maximum = maxValue,
                Minimum = minValue,
                Tooltip = tooltip,
                Step = silderStep,
                Value = entryValue
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

            return new Grid().DefineColumns(Grid.Remain, 220f).DefineRows(24f, StandardRowHeight)
                .Set(
                        entryLabel, entryLabel,
                        slider, numberBox
                    );
        }

        /// <summary>
        /// 为 <see cref="string"/> 类型的设置选项创建一个输入框
        /// </summary>
        /// <typeparam name="T">保存设置数据的类型</typeparam>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的字段的名称</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="isReadOnly">输入框是否为只读状态</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="inputValidator">输入字符的正则表达式</param>
        /// <returns>指定条目对应的输入框布局。</returns>
        public static Grid CreateTextEntry<T>(
            T settings,
            string entryName,
            string label,
            string tooltip,
            bool isReadOnly = false,
            bool wrapText = false,
            GameFont fontSize = GameFont.Small,
            Regex inputValidator = null)
        {
            return CreateTextEntry(
                settings,
                entryName,
                typeof(T).GetValue<string>(entryName),
                label,
                tooltip,
                isReadOnly,
                wrapText,
                fontSize,
                inputValidator);
        }

        /// <summary>
        /// 为 <see cref="string"/> 类型的设置选项创建一个输入框
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的字段的名称</param>
        /// <param name="entryValue">设置选项的当前值</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="isReadOnly">输入框是否为只读状态</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="inputValidator">输入字符的正则表达式</param>
        /// <returns>指定条目对应的输入框布局。</returns>
        public static Grid CreateTextEntry(
            object settings,
            string entryName,
            string entryValue,
            string label,
            string tooltip,
            bool isReadOnly = false,
            bool wrapText = false,
            GameFont fontSize = GameFont.Small,
            Regex inputValidator = null)
        {
            var entryLabel = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Height = StandardRowHeight,
                Text = label,
                Tooltip = tooltip,
                VerticalAlignment = VerticalAlignment.Top
            };

            var textBox = new TextBox
            {
                FontSize = fontSize,
                HorizontalAlignment = HorizontalAlignment.Right,
                InputValidator = inputValidator,
                IsReadOnly = isReadOnly,
                Text = entryValue,
                Tooltip = tooltip,
                WrapText = wrapText
            };

            BindingManager.Bind(
                textBox,
                TextBox.TextProperty,
                settings,
                entryName,
                BindingMode.OneWay);

            return new Grid().DefineColumns(0.35f, 0.65f).DefineRows(Grid.Auto)
                .Set(entryLabel, textBox);
        }



        internal static CheckBox CreateBooleanEntry(object settings, SettingEntry entry)
        {
            var entryInfo = (BooleanSettingEntryAttribute)entry.EntryInfo;
            return CreateBooleanEntry(
                settings,
                entry.Name,
                (bool)entry.Value,
                entry.Label,
                entry.Tooltip,
                entryInfo.Height);
        }

        internal static Grid CreateNumberEntry(object settings, SettingEntry entry)
        {
            var slideInfo = (NumberSettingEntryAttribute)entry.EntryInfo;
            return CreateNumberEntry(
                settings,
                entry.Name,
                (float)entry.Value,
                entry.Label,
                entry.Tooltip,
                slideInfo.MinValue,
                slideInfo.MaxValue,
                slideInfo.DecimalPartDigit,
                slideInfo.SliderStep,
                slideInfo.DisplayAsPercent);
        }

        internal static Grid CreateTextEntry(object settings, SettingEntry entry)
        {
            var entryInfo = (TextSettingEntryAttribute)entry.EntryInfo;
            return CreateTextEntry(
                settings,
                entry.Name,
                (string)entry.Value,
                entry.Label,
                entry.Tooltip,
                entryInfo.IsReadOnly,
                entryInfo.WrapText,
                entryInfo.FontSize,
                entryInfo.InputValidator);
        }
    }
}
