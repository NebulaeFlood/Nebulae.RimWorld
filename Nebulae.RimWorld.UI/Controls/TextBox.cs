﻿using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 文本框控件
    /// </summary>
    public class TextBox : FocusableControl
    {
        private bool _isReadOnly = false;
        private bool _wrapText = true;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        #region FontSize
        /// <summary>
        /// 获取或设置字体大小
        /// </summary>
        public GameFont FontSize
        {
            get { return (GameFont)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="FontSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(TextBox),
                new PropertyMetadata(GameFont.Small));
        #endregion

        #region InputValidator
        /// <summary>
        /// 获取或设置验证输入文字的正则表达式
        /// </summary>
        public Regex InputValidator
        {
            get { return (Regex)GetValue(InputValidatorProperty); }
            set { SetValue(InputValidatorProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="InputValidator"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty InputValidatorProperty =
            DependencyProperty.Register(nameof(InputValidator), typeof(Regex), typeof(TextBox),
                new PropertyMetadata(null, OnInputValidatorChanged));

        private static void OnInputValidatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBox)d).Text = string.Empty;
        }
        #endregion

        /// <summary>
        /// 用户是否可编辑文字内容
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => _isReadOnly = value;
        }

        #region Text
        /// <summary>
        /// 获取或设置控件文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Text"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBox),
                new PropertyMetadata(string.Empty));
        #endregion

        /// <summary>
        /// 是否自动换行
        /// </summary>
        public bool WrapText
        {
            get => _wrapText;
            set => _wrapText = value;
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="TextBlock"/> 的新实例
        /// </summary>
        public TextBox()
        {
        }


        /// <inheritdoc/>
        protected override void DrawControl()
        {
            Color color = GUI.color;
            GameFont controlFont = FontSize;
            GameFont font = GameText.Font;
            GameText.Font = controlFont;

            bool isDisabled = !IsEnabled;

            if (isDisabled)
            {
                GUI.color = color * Widgets.InactiveColor;
            }

            if (_isReadOnly || isDisabled)
            {
                UIUtility.DrawInputBox(RenderRect, Text, controlFont, true, _wrapText);
            }
            else
            {
                string text = Text;
                string buffer = UIUtility.DrawInputBox(RenderRect, text, controlFont, false, _wrapText);

                if (text != buffer && (InputValidator is null || InputValidator.IsMatch(buffer)))
                {
                    Text = buffer;
                }
            }

            GUI.color = color;
            GameText.Font = font;
        }
    }
}
