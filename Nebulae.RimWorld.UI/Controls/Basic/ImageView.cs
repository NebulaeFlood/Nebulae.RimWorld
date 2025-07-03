using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 图片控件
    /// </summary>
    public sealed class ImageView : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region Composition
        /// <summary>
        /// 获取或设置图片混合色
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
            DependencyProperty.Register(nameof(Composition), typeof(Color), typeof(ImageView),
                new PropertyMetadata(new Color(1f, 1f, 1f, 1f)));
        #endregion

        #region ImageSource
        /// <summary>
        /// 获取或设置图片源
        /// </summary>
        public Texture2D ImageSource
        {
            get { return (Texture2D)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value ?? throw new InvalidOperationException($"Cannot assign {Type}.{nameof(ImageSource)} to a null value.")); }
        }

        /// <summary>
        /// 标识 <see cref="ImageSource"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(Texture2D), typeof(ImageView),
                new ControlPropertyMetadata(ControlRelation.Measure));
        #endregion

        #region ScaleMode
        /// <summary>
        /// 获取或设置图片缩放模式
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
            DependencyProperty.Register(nameof(ScaleMode), typeof(ScaleMode), typeof(ImageView),
                new ControlPropertyMetadata(ScaleMode.ScaleToFit, ControlRelation.Measure));
        #endregion

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
            var image = ImageSource;
            var scaleMode = ScaleMode;

            UpdateDrawer(image, scaleMode);

            if (scaleMode is ScaleMode.ScaleToFit)
            {
                float scale;

                if (image.width < availableSize.Width && image.height < availableSize.Height)
                {
                    scale = Mathf.Max(availableSize.Width / image.width, availableSize.Height / image.height);
                }
                else if (image.width <= availableSize.Width && image.height > availableSize.Height)
                {
                    scale = availableSize.Height / image.height;
                }
                else if (image.width > availableSize.Width && image.height <= availableSize.Height)
                {
                    scale = availableSize.Width / image.width;
                }
                else
                {
                    scale = Mathf.Min(availableSize.Width / image.width, availableSize.Height / image.height);
                }

                return new Size(image.width * scale, image.height * scale);
            }

            return new Size(image.width, image.height);
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            _drawer(states);
        }

        #endregion


        private void UpdateDrawer(Texture2D image, ScaleMode scaleMode)
        {
            var composition = Composition;

            void Draw(ControlState states)
            {
                GUI.color *= states.HasState(ControlState.Disabled)
                    ? composition * Widgets.InactiveColor
                    : composition;

                GUI.DrawTexture(RenderRect, image, scaleMode);
            }

            _drawer = Draw;
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Action<ControlState> _drawer;

        #endregion
    }
}
