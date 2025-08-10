using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 图标按钮
    /// </summary>
    public sealed class IconButton : ButtonBase
    {
        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region Composition
        /// <summary>
        /// 获取或设置 <see cref="Button"/> 的混合色
        /// </summary>
        public Color Composition
        {
            get { return (Color)GetValue(CompositionProperty); }
            set { SetValue(CompositionProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Composition"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty CompositionProperty =
            DependencyProperty.Register(nameof(Composition), typeof(Color), typeof(IconButton),
                new PropertyMetadata(new Color(1f, 1f, 1f), UpdateDrawer));
        #endregion

        #region Highlight
        /// <summary>
        /// 获取或设置 <see cref="IconButton"/> 的高亮颜色
        /// </summary>
        public Color Highlight
        {
            get { return (Color)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Highlight"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register(nameof(Highlight), typeof(Color), typeof(IconButton),
                new PropertyMetadata(GenUI.MouseoverColor, UpdateDrawer));
        #endregion

        #region Icon
        /// <summary>
        /// 获取或设置 <see cref="IconButton"/> 的图标
        /// </summary>
        public Texture2D Icon
        {
            get { return (Texture2D)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Icon"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Texture2D), typeof(IconButton),
                new ControlPropertyMetadata(BaseContent.BadTex, OnIconChanged), ValidateIcon);

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetValue(ScaleModeProperty) is ScaleMode.StretchToFill)
            {
                UpdateDrawer(d, e);
            }
            else
            {
                UpdateDrawer(d, null);
            }
        }

        private static bool ValidateIcon(object value)
        {
            return value != null;
        }
        #endregion

        #region ScaleMode
        /// <summary>
        /// 获取或设置 <see cref="IconButton"/> 的图标缩放模式
        /// </summary>
        public ScaleMode ScaleMode
        {
            get { return (ScaleMode)GetValue(ScaleModeProperty); }
            set { SetValue(ScaleModeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ScaleMode"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ScaleModeProperty =
            DependencyProperty.Register(nameof(ScaleMode), typeof(ScaleMode), typeof(IconButton),
                new ControlPropertyMetadata(ScaleMode.ScaleToFit, UpdateDrawer, ControlRelation.Measure));
        #endregion

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        static IconButton()
        {
            ClickSoundProperty.OverrideMetadata(typeof(IconButton),
                new PropertyMetadata(SoundDefOf.Click));
            CursorEnterSoundProperty.OverrideMetadata(typeof(IconButton),
                new PropertyMetadata(SoundDefOf.Mouseover_Standard));
        }

        /// <summary>
        /// 初始化 <see cref="IconButton"/> 的新实例
        /// </summary>
        public IconButton() { }

        /// <summary>
        /// 初始化 <see cref="IconButton"/> 的新实例
        /// </summary>
        /// <param name="icon">按钮图标</param>
        public IconButton(Texture2D icon)
        {
            SetValue(IconProperty, icon);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            var icon = (Texture2D)GetValue(IconProperty);
            var scaleMode = (ScaleMode)GetValue(ScaleModeProperty);

            switch (scaleMode)
            {
                case ScaleMode.ScaleAndCrop:
                    return MeasureCrop(availableSize, icon);
                case ScaleMode.ScaleToFit:
                    return MeasureFit(availableSize, icon);
                default:
                    return availableSize;
            }
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
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

        private static Action<IconButton, ControlState> CreateDrawer(Color composition, Color highlight, Texture2D icon, ScaleMode scaleMode)
        {
            void Draw(IconButton button, ControlState states)
            {
                if (states.HasState(ControlState.Disabled))
                {
                    GUI.color *= composition * Widgets.InactiveColor;
                }
                else if (states.HasState(ControlState.CursorDirectlyOver))
                {
                    GUI.color *= composition * highlight;
                }
                else
                {
                    GUI.color *= composition;
                }

                GUI.DrawTexture(button.RenderRect, icon, scaleMode);
            }

            return Draw;
        }

        private static Size MeasureCrop(Size availableSize, Texture2D icon)
        {
            bool autoWidth = float.IsNaN(availableSize.Width);
            bool autoHeight = float.IsNaN(availableSize.Height);

            if (autoWidth)
            {
                if (autoHeight)
                {
                    return new Size(icon.width, icon.height);
                }

                return new Size(icon.width * availableSize.Height / icon.height, availableSize.Height);
            }
            else if (autoHeight)
            {
                return new Size(availableSize.Width, icon.height * availableSize.Width / icon.width);
            }
            else
            {
                float iconWidth = icon.width;
                float iconHeight = icon.height;

                if (iconWidth == availableSize.Width && iconHeight == availableSize.Height)
                {
                    return new Size(iconWidth, iconHeight);
                }

                float scale;

                if (iconWidth < availableSize.Width && iconHeight < availableSize.Height)
                {
                    scale = MathF.Max(availableSize.Width / iconWidth, availableSize.Height / iconHeight);
                }
                else if (iconWidth < availableSize.Width && iconHeight > availableSize.Height)
                {
                    scale = availableSize.Width / iconWidth;
                }
                else if (iconWidth > availableSize.Width && iconHeight < availableSize.Height)
                {
                    scale = availableSize.Height / iconHeight;
                }
                else
                {
                    scale = MathF.Max(availableSize.Width / iconWidth, availableSize.Height / iconHeight);
                }

                return new Size(iconWidth * scale, iconHeight * scale);
            }
        }

        private static Size MeasureFit(Size availableSize, Texture2D icon)
        {
            bool autoWidth = float.IsNaN(availableSize.Width);
            bool autoHeight = float.IsNaN(availableSize.Height);

            if (autoWidth)
            {
                if (autoHeight)
                {
                    return new Size(icon.width, icon.height);
                }

                return new Size(icon.width * availableSize.Height / icon.height, availableSize.Height);
            }
            else if (autoHeight)
            {
                return new Size(availableSize.Width, icon.height * availableSize.Width / icon.width);
            }
            else
            {
                float iconWidth = icon.width;
                float iconHeight = icon.height;

                if ((iconWidth == availableSize.Width && iconHeight <= availableSize.Height)
                    || (iconHeight == availableSize.Height && iconWidth <= availableSize.Width))
                {
                    return new Size(iconWidth, iconHeight);
                }

                float scale;

                if (iconWidth < availableSize.Width && iconHeight < availableSize.Height)
                {
                    scale = MathF.Min(availableSize.Width / iconWidth, availableSize.Height / iconHeight);
                }
                else if (iconWidth < availableSize.Width && iconHeight > availableSize.Height)
                {
                    scale = availableSize.Height / iconHeight;
                }
                else if (iconWidth > availableSize.Width && iconHeight < availableSize.Height)
                {
                    scale = availableSize.Width / iconWidth;
                }
                else
                {
                    scale = MathF.Min(availableSize.Width / iconWidth, availableSize.Height / iconHeight);
                }

                return new Size(iconWidth * scale, iconHeight * scale);
            }
        }

        private static void UpdateDrawer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var iconButton = (IconButton)d;

            if (e is null)
            {
                iconButton.InvalidateMeasure();
            }

            iconButton._drawer = CreateDrawer(
                (Color)d.GetValue(CompositionProperty),
                (Color)d.GetValue(HighlightProperty),
                (Texture2D)d.GetValue(IconProperty),
                (ScaleMode)d.GetValue(ScaleModeProperty));
        }

        #endregion


        private static readonly Action<IconButton, ControlState> DefaultDrawer = CreateDrawer(new Color(1f, 1f, 1f), GenUI.MouseoverColor, BaseContent.BadTex, ScaleMode.ScaleToFit);


        private Action<IconButton, ControlState> _drawer = DefaultDrawer;
    }
}
