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
        /// 获取或设置 <see cref="ImageView"/> 的图片混合色
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
                new PropertyMetadata(new Color(1f, 1f, 1f, 1f), UpdateDrawer));
        #endregion

        #region ImageSource
        /// <summary>
        /// 获取或设置 <see cref="ImageView"/> 的图片源
        /// </summary>
        public Texture2D ImageSource
        {
            get { return (Texture2D)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ImageSource"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(Texture2D), typeof(ImageView),
                new ControlPropertyMetadata(BaseContent.BadTex, OnImageSourceChanged, ControlRelation.Measure), ValidateImageSource);

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

        private static bool ValidateImageSource(object value)
        {
            return value != null;
        }
        #endregion

        #region ScaleMode
        /// <summary>
        /// 获取或设置 <see cref="ImageView"/> 的图片缩放模式
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
                new ControlPropertyMetadata(ScaleMode.ScaleToFit, UpdateDrawer, ControlRelation.Measure));
        #endregion

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="ImageView"/> 的新实例
        /// </summary>
        public ImageView() { }

        /// <summary>
        /// 初始化 <see cref="ImageView"/> 的新实例
        /// </summary>
        /// <param name="source">图片源</param>
        public ImageView(Texture2D source)
        {
            SetValue(ImageSourceProperty, source);
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
            var image = (Texture2D)GetValue(ImageSourceProperty);
            var scaleMode = (ScaleMode)GetValue(ScaleModeProperty);

            switch (scaleMode)
            {
                case ScaleMode.ScaleAndCrop:
                    return MeasureCrop(availableSize, image);
                case ScaleMode.ScaleToFit:
                    return MeasureFit(availableSize, image);
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

        private static Action<ImageView, ControlState> CreateDrawer(Color composition, Texture2D imageSource, ScaleMode scaleMode)
        {
            void Draw(ImageView view, ControlState states)
            {
                GUI.color *= states.HasState(ControlState.Disabled)
                    ? composition * Widgets.InactiveColor
                    : composition;

                GUI.DrawTexture(view.RenderRect, imageSource, scaleMode);
            }

            return Draw;
        }

        private static Size MeasureCrop(Size availableSize, Texture2D image)
        {
            bool autoWidth = float.IsNaN(availableSize.Width);
            bool autoHeight = float.IsNaN(availableSize.Height);

            if (autoWidth)
            {
                if (autoHeight)
                {
                    return new Size(image.width, image.height);
                }

                return new Size(image.width * availableSize.Height / image.height, availableSize.Height);
            }
            else if (autoHeight)
            {
                return new Size(availableSize.Width, image.height * availableSize.Width / image.width);
            }
            else
            {
                float imageWidth = image.width;
                float imageHeight = image.height;

                if (imageWidth == availableSize.Width && imageHeight == availableSize.Height)
                {
                    return new Size(imageWidth, imageHeight);
                }

                float scale;

                if (imageWidth < availableSize.Width && imageHeight < availableSize.Height)
                {
                    scale = MathF.Max(availableSize.Width / imageWidth, availableSize.Height / imageHeight);
                }
                else if (imageWidth < availableSize.Width && imageHeight > availableSize.Height)
                {
                    scale = availableSize.Width / imageWidth;
                }
                else if (imageWidth > availableSize.Width && imageHeight < availableSize.Height)
                {
                    scale = availableSize.Height / imageHeight;
                }
                else
                {
                    scale = MathF.Max(availableSize.Width / imageWidth, availableSize.Height / imageHeight);
                }

                return new Size(imageWidth * scale, imageHeight * scale);
            }
        }

        private static Size MeasureFit(Size availableSize, Texture2D image)
        {
            bool autoWidth = float.IsNaN(availableSize.Width);
            bool autoHeight = float.IsNaN(availableSize.Height);

            if (autoWidth)
            {
                if (autoHeight)
                {
                    return new Size(image.width, image.height);
                }

                return new Size(image.width * availableSize.Height / image.height, availableSize.Height);
            }
            else if (autoHeight)
            {
                return new Size(availableSize.Width, image.height * availableSize.Width / image.width);
            }
            else
            {
                float imageWidth = image.width;
                float imageHeight = image.height;

                if ((imageWidth == availableSize.Width && imageHeight <= availableSize.Height)
                    || (imageHeight == availableSize.Height && imageWidth <= availableSize.Width))
                {
                    return new Size(imageWidth, imageHeight);
                }

                float scale;

                if (imageWidth < availableSize.Width && imageHeight < availableSize.Height)
                {
                    scale = MathF.Min(availableSize.Width / imageWidth, availableSize.Height / imageHeight);
                }
                else if (imageWidth < availableSize.Width && imageHeight > availableSize.Height)
                {
                    scale = availableSize.Height / imageHeight;
                }
                else if (imageWidth > availableSize.Width && imageHeight < availableSize.Height)
                {
                    scale = availableSize.Width / imageWidth;
                }
                else
                {
                    scale = MathF.Min(availableSize.Width / imageWidth, availableSize.Height / imageHeight);
                }

                return new Size(imageWidth * scale, imageHeight * scale);
            }
        }

        private static void UpdateDrawer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imageView = (ImageView)d;

            if (e is null)
            {
                imageView.InvalidateMeasure();
            }

            imageView._drawer = CreateDrawer(
                (Color)d.GetValue(CompositionProperty),
                (Texture2D)d.GetValue(ImageSourceProperty),
                (ScaleMode)d.GetValue(ScaleModeProperty));
        }

        #endregion


        private static readonly Action<ImageView, ControlState> DefaultDrawer = CreateDrawer(new Color(1f, 1f, 1f), BaseContent.BadTex, ScaleMode.ScaleToFit);


        private Action<ImageView, ControlState> _drawer = DefaultDrawer;
    }
}
