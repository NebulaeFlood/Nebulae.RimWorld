using Nebulae.RimWorld.UI.Automation.Attributes;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Converters;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Data.Bindings;
using Nebulae.RimWorld.UI.Core.Data.Bindings.Converters;
using Nebulae.RimWorld.UI.Core.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 自动布局系统
    /// </summary>
    public static class AutoLayoutSystem
    {
        /// <summary>
        /// 标准自动布局控件高度
        /// </summary>
        public const float StandardRowHeight = 34f;


        #region Create Boolean Entry
        /// <summary>
        /// 为可转化为 <see cref="ToggleState"/> 的设置选项创建一个 <see cref="CheckBox"/>
        /// </summary>
        /// <typeparam name="T">保存设置数据的类型</typeparam>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的成员的名称</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="mode"><see cref="CheckBox"/> 与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的 <see cref="CheckBox"/> 的实例。</returns>
        public static CheckBox CreateBooleanEntry<T>(T settings, string entryName, string label, string tooltip, BindingMode mode = BindingMode.OneWay)
        {
            return CreateBooleanEntry(settings, entryName, typeof(T).GetValue<bool>(entryName), label, tooltip, mode);
        }

        /// <summary>
        /// 为可转化为 <see cref="ToggleState"/> 的设置选项创建一个 <see cref="CheckBox"/>
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的成员的名称</param>
        /// <param name="entryValue">设置选项对应的值</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="mode"><see cref="CheckBox"/> 与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的 <see cref="CheckBox"/> 的实例。</returns>
        public static CheckBox CreateBooleanEntry(object settings, string entryName, bool entryValue, string label, string tooltip, BindingMode mode = BindingMode.OneWay)
        {
            var checkBox = new CheckBox
            {
                Padding = new Thickness(12f, 0f, 12f, 0f),
                State = entryValue ? ToggleState.On : ToggleState.Off,
                Text = label,
                Tooltip = tooltip
            };

            Binding.Create(
                checkBox,
                ToggleButton.StateProperty,
                settings,
                entryName,
                mode,
                ToggleStateToBoolean.Instance);

            return checkBox;
        }

        /// <summary>
        /// 为可转化为 <see cref="ToggleState"/> 的设置选项创建一个 <see cref="CheckBox"/>
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entry">设置选项对应的依赖属性标识</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="mode"><see cref="CheckBox"/> 与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的 <see cref="CheckBox"/> 的实例。</returns>
        public static CheckBox CreateBooleanEntry(DependencyObject settings, DependencyProperty entry, string label, string tooltip, BindingMode mode = BindingMode.TwoWay)
        {
            var converter = CreateConverter<bool>(entry.ValueType, out bool requireConversion);
            var checkBox = new CheckBox
            {
                Padding = new Thickness(12f, 0f, 12f, 0f),
                Text = label,
                Tooltip = tooltip
            };

            if (requireConversion)
            {
                checkBox.State = (ToggleState)converter.ConvertBack(settings.GetValue(entry), typeof(ToggleState), CultureInfo.CurrentCulture);

                Binding.Create(
                    checkBox,
                    ToggleButton.StateProperty,
                    settings,
                    entry,
                    mode,
                    converter);
            }
            else
            {
                checkBox.State = (ToggleState)settings.GetValue(entry);

                Binding.Create(
                    checkBox,
                    ToggleButton.StateProperty,
                    settings,
                    entry,
                    mode);
            }

            return checkBox;
        }
        #endregion

        #region Create Number Entry
        /// <summary>
        /// 为可转化为 <see cref="float"/> 类型的设置选项创建一个带有输入和滑动两种修改方式的布局
        /// </summary>
        /// <typeparam name="T">保存设置数据的类型</typeparam>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的成员的名称</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="minValue">选项的最小值</param>
        /// <param name="maxValue">选项的最大值</param>
        /// <param name="decimals">小数部分的位数</param>
        /// <param name="isPercentage">是否以小数形式显示值</param>
        /// <param name="mode">设置控件与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的带有输入和滑动两种修改方式的布局。</returns>
        public static Grid CreateNumberEntry<T>(T settings, string entryName, string label, string tooltip, float minValue, float maxValue, ushort decimals, bool isPercentage = false, BindingMode mode = BindingMode.OneWay)
        {
            return CreateNumberEntry(settings, entryName, typeof(T).GetValue<float>(entryName, settings), label, tooltip, minValue, maxValue, decimals, isPercentage, mode);
        }

        /// <summary>
        /// 为可转化为 <see cref="float"/> 类型的设置选项创建一个设置控件
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的成员的名称</param>
        /// <param name="entryValue">设置选项的当前值</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="minValue">选项的最小值</param>
        /// <param name="maxValue">选项的最大值</param>
        /// <param name="decimals">小数部分的位数</param>
        /// <param name="isPercentage">是否以小数形式显示值</param>
        /// <param name="mode">设置控件与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的设置控件。</returns>
        public static Grid CreateNumberEntry(object settings, string entryName, float entryValue, string label, string tooltip, float minValue, float maxValue, ushort decimals, bool isPercentage = false, BindingMode mode = BindingMode.OneWay)
        {
            var entryLabel = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Text = label,
                Tooltip = tooltip
            };

            var numberBox = new NumberBox
            {
                Decimals = decimals,
                IsPercentage = isPercentage,
                MaxValue = maxValue,
                MinValue = minValue,
                Tooltip = tooltip,
                Value = entryValue
            };

            var slider = new Slider
            {
                Decimals = decimals,
                HorizontalAlignment = HorizontalAlignment.Right,
                IsPercentage = isPercentage,
                MaxWidth = 500f,
                MaxValue = maxValue,
                MinValue = minValue,
                Tooltip = tooltip,
                ShowLabels = true,
                Value = entryValue
            };

            Binding.Create(
                slider,
                Slider.ValueProperty,
                numberBox,
                NumberBox.ValueProperty,
                BindingMode.TwoWay);

            Binding.Create(
                slider,
                Slider.ValueProperty,
                settings,
                entryName,
                mode);

            return new Grid { Padding = new Thickness(12f, 0f, 12f, 0f) }.DefineColumns(Grid.Auto, Grid.Remain, 100f).DefineRows(StandardRowHeight)
                .Set(entryLabel, slider, numberBox);
        }

        /// <summary>
        /// 为可转化为 <see cref="float"/> 类型的设置选项创建一个设置控件
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entry">设置选项对应的依赖属性标识</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="minValue">选项的最小值</param>
        /// <param name="maxValue">选项的最大值</param>
        /// <param name="decimals">小数部分的位数</param>
        /// <param name="isPercentage">是否以小数形式显示值</param>
        /// <param name="mode">设置控件与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的设置控件。</returns>
        public static Grid CreateNumberEntry(DependencyObject settings, DependencyProperty entry, string label, string tooltip, float minValue, float maxValue, ushort decimals, bool isPercentage = false, BindingMode mode = BindingMode.TwoWay)
        {
            var entryLabel = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Text = label,
                Tooltip = tooltip
            };

            var converter = CreateConverter<float>(entry.ValueType, out bool requireConversion);

            var numberBox = new NumberBox
            {
                Decimals = decimals,
                IsPercentage = isPercentage,
                MaxValue = maxValue,
                MinValue = minValue,
                Tooltip = tooltip
            };

            var slider = new Slider
            {
                Decimals = decimals,
                HorizontalAlignment = HorizontalAlignment.Right,
                IsPercentage = isPercentage,
                MaxWidth = 500f,
                MaxValue = maxValue,
                MinValue = minValue,
                Tooltip = tooltip,
                ShowLabels = true
            };

            if (requireConversion)
            {
                numberBox.Value = (float)converter.ConvertBack(settings.GetValue(entry), typeof(float), CultureInfo.CurrentCulture);
                slider.Value = (float)converter.ConvertBack(settings.GetValue(entry), typeof(float), CultureInfo.CurrentCulture);
            }
            else
            {
                numberBox.Value = (float)settings.GetValue(entry);
                slider.Value = (float)settings.GetValue(entry);
            }

            Binding.Create(
                slider,
                Slider.ValueProperty,
                numberBox,
                NumberBox.ValueProperty,
                BindingMode.TwoWay,
                converter);

            Binding.Create(
                slider,
                Slider.ValueProperty,
                settings,
                entry,
                mode,
                converter);

            return new Grid { Padding = new Thickness(12f, 0f, 12f, 0f) }.DefineColumns(Grid.Auto, Grid.Remain, 100f).DefineRows(StandardRowHeight)
                .Set(entryLabel, slider, numberBox);
        }
        #endregion

        #region Create Text Entry
        /// <summary>
        /// 为可转化为 <see cref="string"/> 类型的设置选项创建一个 <see cref="TextBox"/>
        /// </summary>
        /// <typeparam name="T">保存设置数据的类型</typeparam>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的成员的名称</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="isReadOnly">输入框是否为只读状态</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="inputValidator">输入字符的正则表达式</param>
        /// <param name="mode">设置控件与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的 <see cref="TextBox"/>。</returns>
        public static Grid CreateTextEntry<T>(T settings, string entryName, string label, string tooltip, bool isReadOnly = false, bool wrapText = false, GameFont fontSize = GameFont.Small, Regex inputValidator = null, BindingMode mode = BindingMode.OneWay)
        {
            return CreateTextEntry(settings, entryName, typeof(T).GetValue<string>(entryName, settings), label, tooltip, isReadOnly, wrapText, fontSize, inputValidator, mode);
        }

        /// <summary>
        /// 为可转化为 <see cref="string"/> 类型的设置选项创建一个 <see cref="TextBox"/>
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entryName">设置选项对应的成员的名称</param>
        /// <param name="entryValue">设置选项对应的值</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="isReadOnly">输入框是否为只读状态</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="inputValidator">输入字符的正则表达式</param>
        /// <param name="mode">设置控件与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的 <see cref="TextBox"/>。</returns>
        public static Grid CreateTextEntry(object settings, string entryName, string entryValue, string label, string tooltip, bool isReadOnly = false, bool wrapText = false, GameFont fontSize = GameFont.Small, Regex inputValidator = null, BindingMode mode = BindingMode.OneWay)
        {
            var entryLabel = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Text = label,
                Tooltip = tooltip,
            };

            var textBox = new TextBox
            {
                FontSize = fontSize,
                InputValidator = inputValidator,
                IsReadOnly = isReadOnly,
                Text = entryValue,
                Tooltip = tooltip,
                WrapText = wrapText
            };

            Binding.Create(
                textBox,
                TextBox.TextProperty,
                settings,
                entryName,
                mode);

            return new Grid { Padding = new Thickness(12f, 0f, 12f, 0f) }
                .DefineColumns(Grid.Auto, Grid.Remain, 260f)
                .DefineRows(Grid.Auto)
                .Set(entryLabel, null, textBox);
        }

        /// <summary>
        /// 为可转化为 <see cref="string"/> 类型的设置选项创建一个 <see cref="TextBox"/>
        /// </summary>
        /// <param name="settings">保存设置数据的实例</param>
        /// <param name="entry">设置选项对应的依赖属性标识</param>
        /// <param name="label">选项的标签文字</param>
        /// <param name="tooltip">选项的提示框文字</param>
        /// <param name="isReadOnly">输入框是否为只读状态</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="inputValidator">输入字符的正则表达式</param>
        /// <param name="mode">设置控件与对应设置数据的绑定模式</param>
        /// <returns>指定条目对应的 <see cref="TextBox"/>。</returns>
        public static Grid CreateTextEntry(DependencyObject settings, DependencyProperty entry, string label, string tooltip, bool isReadOnly = false, bool wrapText = false, GameFont fontSize = GameFont.Small, Regex inputValidator = null, BindingMode mode = BindingMode.TwoWay)
        {
            var entryLabel = new Label
            {
                Anchor = TextAnchor.MiddleLeft,
                Text = label,
                Tooltip = tooltip
            };

            var converter = CreateConverter<string>(entry.ValueType, out bool convertOperation);

            var textBox = new TextBox
            {
                FontSize = fontSize,
                InputValidator = inputValidator,
                IsReadOnly = isReadOnly,
                Text = convertOperation ? (string)converter.Convert(settings.GetValue(entry), entry.ValueType, CultureInfo.InvariantCulture) : (string)settings.GetValue(entry),
                Tooltip = tooltip,
                WrapText = wrapText
            };

            Binding.Create(
                textBox,
                TextBox.TextProperty,
                settings,
                entry,
                mode,
                converter);

            return new Grid { Padding = new Thickness(12f, 0f, 12f, 0f) }
                .DefineColumns(Grid.Auto, Grid.Remain, 260f)
                .DefineRows(Grid.Auto)
                .Set(entryLabel, null, textBox);
        }
        #endregion


        /// <summary>
        /// 生成一个布局面板，包含所有设置条目。
        /// </summary>
        /// <typeparam name="T">保存设置数据的类型</typeparam>
        /// <param name="settings">保存设置数据的对象</param>
        /// <returns>包含所有设置条目的 <see cref="StackPanel"/>。</returns>
        public static StackPanel GenerateLayout<T>(this T settings) where T : class
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            var type = typeof(T);
            var settingAttribute = type.GetCustomAttribute<SettingsAttribute>(false)
                ?? throw new InvalidOperationException($"Failed to generate layout for type \"{type.FullName}\": a [{typeof(SettingsAttribute)}] is required on the type definition.");

            return new StackPanel { VerticalAlignment = VerticalAlignment.Top }
                .Set(
                    PrecessEntryInfos(
                        ProcessMembers(settings, settingAttribute.TraslationKeyPrefix, type)));
        }


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static IValueConverter CreateConverter<T>(Type sourceType, out bool requireConversion)
        {
            var targetType = typeof(T);

            if (sourceType == targetType)
            {
                requireConversion = false;
                return null;
            }

            requireConversion = true;
            return BindingBase.GetDefaultConverter(sourceType, targetType);
        }

        private static IEnumerable<Control> PrecessEntryInfos(IEnumerable<SettingEntryInfo> entryInfos)
        {
            foreach (var info in entryInfos)
            {
                switch (info.EntryType)
                {
                    case SettingEntryType.Boolean:
                        yield return CreateBooleanEntry(info.Owner, info.Name, (bool)info.Value, info.Label, info.Tooltip, info.EntryInfo.BindingMode);
                        break;
                    case SettingEntryType.Number:
                        var numberEntry = (NumberSettingEntryAttribute)info.EntryInfo;
                        yield return CreateNumberEntry(info.Owner, info.Name, (float)info.Value, info.Label, info.Tooltip, numberEntry.MinValue, numberEntry.MaxValue, numberEntry.Decimals, numberEntry.IsPercentage, numberEntry.BindingMode);
                        break;
                    case SettingEntryType.String:
                        var textEntry = (TextSettingEntryAttribute)info.EntryInfo;
                        yield return CreateTextEntry(info.Owner, info.Name, (string)info.Value, info.Label, info.Tooltip, textEntry.IsReadOnly, textEntry.WrapText, textEntry.FontSize, null, textEntry.BindingMode);
                        break;
                    default:
                        break;
                }
            }
        }

        private static IEnumerable<SettingEntryInfo> ProcessMembers(object owner, string translationKeyPrefix, Type type)
        {
            var members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            var count = members.Length;

            if (count < 1)
            {
                yield break;
            }

            SettingEntryInfo.KeyBuilder.Capacity = 64;

            for (int i = 0; i < count; i++)
            {
                var member = members[i];

                if (member.GetCustomAttribute(typeof(SettingEntryBaseAttribute)) is SettingEntryBaseAttribute entryInfo)
                {
                    yield return new SettingEntryInfo(translationKeyPrefix, entryInfo, member, owner);
                }
            }

            SettingEntryInfo.KeyBuilder.Clear();
            SettingEntryInfo.KeyBuilder.Capacity = 0;
        }

        #endregion
    }
}
