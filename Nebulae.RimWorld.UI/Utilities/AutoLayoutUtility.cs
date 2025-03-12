using Nebulae.RimWorld.UI.Automation;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Converters;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Data.Binding;
using Nebulae.RimWorld.UI.Data.Binding.Converters;
using Nebulae.RimWorld.UI.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using static RimWorld.TransferableUIUtility;
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
        /// 为可转化为 <see cref="ToggleStatus"/> 类型的设置选项创建一个 <see cref="CheckBox"/>
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
        /// 为<see cref="bool"/> 类型的设置选项创建一个 <see cref="CheckBox"/>
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
        /// 可与 <see cref="ToggleStatus"/> 互相转换的设置选项创建一个 <see cref="CheckBox"/>
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entry">设置选项对应的依赖属性</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="height">选项的高度</param>
        /// <returns>指定条目对应的 <see cref="CheckBox"/> 的实例。</returns>
        public static CheckBox CreateBooleanEntry(
            DependencyObject settings,
            DependencyProperty entry,
            string label,
            string tooltip,
            float height = StandardRowHeight)
        {
            IValueConverter converter = CreateConverter<ToggleStatus>(entry.ValueType, out var convertOperation);

            var checkBox = new CheckBox
            {
                Height = height,
                Text = label,
                Tooltip = tooltip
            };

            if (convertOperation)
            {
                checkBox.Status = (ToggleStatus)converter.Convert(settings.GetValue(entry), CultureInfo.InvariantCulture);

                BindingManager.Bind(
                    checkBox,
                    ToggleButton.StatusProperty,
                    settings,
                    entry,
                    converter,
                    BindingMode.TwoWay);
            }
            else
            {
                checkBox.Status = (ToggleStatus)settings.GetValue(entry);

                BindingManager.Bind(
                checkBox,
                ToggleButton.StatusProperty,
                settings,
                entry,
                BindingMode.TwoWay);
            }

            return checkBox;
        }

        /// <summary>
        /// 为预设的设置条目创建对应的控件
        /// </summary>
        /// <typeparam name="T">保存设置数据的类型</typeparam>
        /// <param name="settings">预设了条目的设置类</param>
        /// <param name="additionalControls">附加的控件</param>
        /// <returns>返回类型为 <see cref="VirtualizingStackPanel"/> 的包含了设置控件的面板控件。</returns>
        public static VirtualizingStackPanel CreateEntries<T>(this T settings, params ControlOrder[] additionalControls)
        {
            Type type = typeof(T);
            var attribute = type.GetCustomAttribute<StandardTranslationKeyAttribute>(false)
                ?? throw new InvalidOperationException($"No entries are identified in {type}");

            var members = type.GetMembers(
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var entries = new List<SettingEntry>();
            string standardKey = attribute.Key;

            for (int i = 0; i < members.Length; i++)
            {
                var member = members[i];

                if (member.GetCustomAttribute<SettingEntryBaseAttribute>() is SettingEntryBaseAttribute entry)
                {
                    entries.Add(new SettingEntry(standardKey, entry, member, type, settings));
                }
            }

            if (attribute.CustomOrder)
            {
                entries.Sort(SettingEntry.Comparison);

                var controls = new List<ControlOrder>(additionalControls);

                for (int i = 0; i < controls.Count; i++)
                {
                    controls.Add(new ControlOrder(entries[i].EntryInfo.Order, Convert(entries[i])));
                }

                controls.Sort(ControlOrder.Comparison);

                return new VirtualizingStackPanel().Set(controls.Select(x => x.Control).ToArray());
            }
            else
            {
                var controls = new List<Visual>();

                for (int i = 0; i < entries.Count; i++)
                {
                    controls.Add(Convert(entries[i]));
                }

                for (int i = 0; i < additionalControls.Length; i++)
                {
                    controls.Add(additionalControls[i].Control);
                }

                return new VirtualizingStackPanel().Set(controls.ToArray());
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
                Anchor = TextAnchor.LowerLeft,
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

            return new Grid().DefineColumns(200f, Grid.Remain, 160f).DefineRows(StandardRowHeight)
                .Set(entryLabel, slider, numberBox);
        }

        /// <summary>
        /// 为可与 <see cref="float"/> 互相转换的设置选项创建一个带有输入和滑动两种修改方式的布局
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entry">设置选项对应的依赖属性</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="minValue">选项的最小值</param>
        /// <param name="maxValue">选项的最大值</param>
        /// <param name="decimalPartDigit">小数部分的位数</param>
        /// <param name="silderStep">最小的相邻可选值的差值</param>
        /// <param name="displayAsPercent">是否以小数形式显示值</param>
        /// <returns>指定条目对应的带有输入和滑动两种修改方式的布局。</returns>
        public static Grid CreateNumberEntry(
            DependencyObject settings,
            DependencyProperty entry,
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
                Anchor = TextAnchor.LowerLeft,
                Text = label,
                Tooltip = tooltip
            };

            IValueConverter converter = CreateConverter<float>(entry.ValueType, out bool convertOperation);

            var numberBox = new NumberBox
            {
                DecimalPartDigit = decimalPartDigit,
                DisplayAsPercent = displayAsPercent,
                Maximum = maxValue,
                Minimum = minValue,
                Tooltip = tooltip,
            };

            var slider = new Slider
            {
                DrawExtremeValues = true,
                DisplayAsPercent = displayAsPercent,
                Maximum = maxValue,
                Minimum = minValue,
                Tooltip = tooltip,
                Step = silderStep,
            };

            if (convertOperation)
            {
                float entryValue = (float)converter.Convert(settings.GetValue(entry), CultureInfo.InvariantCulture);

                numberBox.Value = entryValue;
                slider.Value = entryValue;

                BindingManager.Bind(
                    slider,
                    Slider.ValueProperty,
                    settings,
                    entry,
                    converter,
                    BindingMode.TwoWay);
            }
            else
            {
                float entryValue = (float)settings.GetValue(entry);

                numberBox.Value = entryValue;
                slider.Value = entryValue;

                BindingManager.Bind(
                    slider,
                    Slider.ValueProperty,
                    settings,
                    entry,
                    BindingMode.TwoWay);
            }

            BindingManager.Bind(
                slider,
                Slider.ValueProperty,
                numberBox,
                NumberBox.ValueProperty,
                BindingMode.TwoWay);

            return new Grid().DefineColumns(200f, Grid.Remain, 160f).DefineRows(StandardRowHeight)
                .Set(entryLabel, slider, numberBox);
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
        /// 为可与 <see cref="string"/> 互相转换的设置选项创建一个输入框
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entry">设置选项对应的依赖属性</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="isReadOnly">输入框是否为只读状态</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="inputValidator">输入字符的正则表达式</param>
        /// <returns>指定条目对应的输入框布局。</returns>
        public static Grid CreateTextEntry(
            DependencyObject settings,
            DependencyProperty entry,
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

            IValueConverter converter = CreateConverter<string>(entry.ValueType, out bool convertOperation);

            var textBox = new TextBox
            {
                FontSize = fontSize,
                HorizontalAlignment = HorizontalAlignment.Right,
                InputValidator = inputValidator,
                IsReadOnly = isReadOnly,
                Tooltip = tooltip,
                WrapText = wrapText
            };

            if (convertOperation)
            {
                textBox.Text = (string)converter.Convert(settings.GetValue(entry), CultureInfo.InvariantCulture);

                BindingManager.Bind(
                    textBox,
                    TextBox.TextProperty,
                    settings,
                    entry,
                    converter,
                    BindingMode.TwoWay);
            }
            else
            {
                textBox.Text = (string)settings.GetValue(entry);

                BindingManager.Bind(
                    textBox,
                    TextBox.TextProperty,
                    settings,
                    entry,
                    converter,
                    BindingMode.TwoWay);
            }

            return new Grid().DefineColumns(0.35f, 0.65f).DefineRows(Grid.Auto)
                .Set(entryLabel, textBox);
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


        internal static Visual Convert(SettingEntry entry)
        {
            switch (entry.EntryType)
            {
                case SettingEntryType.Boolean:
                    return CreateBooleanEntry(entry);
                case SettingEntryType.Number:
                    return CreateNumberEntry(entry);
                case SettingEntryType.String:
                    return CreateTextEntry(entry);
                default:
                    throw new InvalidOperationException($"{entry.EntryType} is a undefined value for auto layout type.");
            }
        }

        internal static CheckBox CreateBooleanEntry(SettingEntry entry)
        {
            var entryInfo = (BooleanSettingEntryAttribute)entry.EntryInfo;

            if (entryInfo.IsDependencyProperty)
            {
                return CreateBooleanEntry(
                    (DependencyObject)entry.Owner,
                    entry.Property,
                    entry.Label,
                    entry.Tooltip,
                    entryInfo.Height);
            }
            else
            {
                return CreateBooleanEntry(
                    entry.Owner,
                    entry.Name,
                    (bool)entry.Value,
                    entry.Label,
                    entry.Tooltip,
                    entryInfo.Height);
            }
        }

        internal static Grid CreateNumberEntry(SettingEntry entry)
        {
            var slideInfo = (NumberSettingEntryAttribute)entry.EntryInfo;

            if (slideInfo.IsDependencyProperty)
            {
                return CreateNumberEntry(
                    (DependencyObject)entry.Owner,
                    entry.Property,
                    entry.Label,
                    entry.Tooltip,
                    slideInfo.MinValue,
                    slideInfo.MaxValue,
                    slideInfo.DecimalPartDigit,
                    slideInfo.SliderStep,
                    slideInfo.DisplayAsPercent);
            }
            else
            {
                return CreateNumberEntry(
                    entry.Owner,
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
        }

        internal static Grid CreateTextEntry(SettingEntry entry)
        {
            var entryInfo = (TextSettingEntryAttribute)entry.EntryInfo;

            if (entryInfo.IsDependencyProperty)
            {
                return CreateTextEntry(
                    (DependencyObject)entry.Owner,
                    entry.Property,
                    entry.Label,
                    entry.Tooltip,
                    entryInfo.IsReadOnly,
                    entryInfo.WrapText,
                    entryInfo.FontSize);
            }
            else
            {
                return CreateTextEntry(
                    entry.Owner,
                    entry.Name,
                    (string)entry.Value,
                    entry.Label,
                    entry.Tooltip,
                    entryInfo.IsReadOnly,
                    entryInfo.WrapText,
                    entryInfo.FontSize);
            }
        }

        internal static IValueConverter CreateConverter<T>(Type sourceType, out bool convertOperation)
        {
            Type targetType = typeof(T);

            if (sourceType == targetType)
            {
                convertOperation = false;
                return null;
            }

            convertOperation = true;
            return BindingManager.CreateDefaultConverter(sourceType, targetType);
        }
    }
}
