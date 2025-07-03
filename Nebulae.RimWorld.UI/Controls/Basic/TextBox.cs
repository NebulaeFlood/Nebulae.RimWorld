using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 输入框控件
    /// </summary>
    public sealed class TextBox : InputControl
    {
        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region AutoHeight
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="TextBox"/> 是否根据内容调整高度
        /// </summary>
        public bool AutoHeight
        {
            get { return (bool)GetValue(AutoHeightProperty); }
            set { SetValue(AutoHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="AutoHeight"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty AutoHeightProperty =
            DependencyProperty.Register(nameof(AutoHeight), typeof(bool), typeof(TextBox),
                new ControlPropertyMetadata(true, ControlRelation.Measure));
        #endregion

        #region FontSize
        /// <summary>
        /// 获取或设置 <see cref="TextBox"/> 的字体大小
        /// </summary>
        public GameFont FontSize
        {
            get { return (GameFont)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="FontSize"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(TextBox),
                new ControlPropertyMetadata(GameFont.Small, TextUtility.CoerceFontSize, UpdateDrawer, ControlRelation.Measure));
        #endregion

        #region InputValidator
        /// <summary>
        /// 获取或设置 <see cref="TextBox"/> 验证输入文字的正则表达式
        /// </summary>
        public Regex InputValidator
        {
            get { return (Regex)GetValue(InputValidatorProperty); }
            set { SetValue(InputValidatorProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="InputValidator"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty InputValidatorProperty =
            DependencyProperty.Register(nameof(InputValidator), typeof(Regex), typeof(TextBox),
                new ControlPropertyMetadata(null, OnInputValidatorChanged, ControlRelation.Arrange));

        private static void OnInputValidatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)d;

            if (e.NewValue is Regex regex && !regex.IsMatch(textBox.Text))
            {
                textBox.Text = string.Empty;
            }

            UpdateDrawer(d, e);
        }
        #endregion

        #region IsReadOnly
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="TextBox"/> 的文本是否为只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsReadOnly"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(TextBox),
                new PropertyMetadata(false, UpdateDrawer));
        #endregion

        #region Text
        /// <summary>
        /// 获取或设置 <see cref="TextBox"/> 的文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Text"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBox),
                new PropertyMetadata(string.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)d;
            textBox._cache = (string)e.NewValue;

            if (textBox.WrapText)
            {
                textBox.InvalidateMeasure();
            }
        }
        #endregion

        #region WrapText
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="TextBox"/> 是否自动换行
        /// </summary>
        public bool WrapText
        {
            get { return (bool)GetValue(WrapTextProperty); }
            set { SetValue(WrapTextProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="WrapText"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty WrapTextProperty =
            DependencyProperty.Register(nameof(WrapText), typeof(bool), typeof(TextBox),
                new ControlPropertyMetadata(true, ControlRelation.Measure));
        #endregion

        #endregion


        static TextBox()
        {
            HeightProperty.OverrideMetadata(typeof(TextBox),
                new ControlPropertyMetadata(34f, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="TextBox"/> 的新实例
        /// </summary>
        public TextBox() { }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            var height = _cache.CalculateTextBoxHeight(
                availableSize.Width,
                (GameFont)GetValue(FontSizeProperty),
                (bool)GetValue(WrapTextProperty));

            if (height > availableSize.Height || AutoHeight)
            {
                return new Size(availableSize.Width, height);
            }

            return availableSize;
        }

        /// <inheritdoc/>
        protected override void DrawControl(ControlState states)
        {
            _drawer(this, states);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static Action<TextBox, ControlState> CreateDrawer(GameFont fontSize, Regex inputValidator, bool isReadOnly, bool wrapText)
        {
            var shouldValidate = inputValidator != null;

            void Draw(TextBox textBox, ControlState states)
            {
                var isDisabled = states.HasState(ControlState.Disabled);

                if (isDisabled)
                {
                    GUI.color *= Widgets.InactiveColor;
                }

                if (isReadOnly || isDisabled)
                {
                    textBox._cache.DrawTextBox(textBox.RenderRect, fontSize, false, wrapText);
                }
                else
                {
                    var buffer = textBox._cache.DrawTextBox(textBox.RenderRect, fontSize, states.HasState(ControlState.Focused | ControlState.CursorDirectlyOver | ControlState.Pressing), wrapText);

                    if (!buffer.Equals(textBox._cache) && (!shouldValidate || inputValidator.IsMatch(buffer)))
                    {
                        textBox.SetValue(TextProperty, buffer);
                    }
                }
            }

            return Draw;
        }

        private static void UpdateDrawer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBox)d)._drawer = CreateDrawer(
                (GameFont)d.GetValue(FontSizeProperty),
                (Regex)d.GetValue(InputValidatorProperty),
                (bool)d.GetValue(IsReadOnlyProperty),
                (bool)d.GetValue(WrapTextProperty));
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly Action<TextBox, ControlState> DefaultDrawer = CreateDrawer(GameFont.Small, null, false, false);

        private string _cache = string.Empty;
        private Action<TextBox, ControlState> _drawer = DefaultDrawer;

        #endregion
    }
}
