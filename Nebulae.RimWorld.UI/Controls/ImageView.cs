using Nebulae.RimWorld.UI.Data;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 图片控件
    /// </summary>
    public class ImageView : Control
    {
        #region ImageSource
        /// <summary>
        /// 获取或设置图片源
        /// </summary>
        public Texture2D ImageSource
        {
            get { return (Texture2D)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ImageSource"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(Texture2D), typeof(ImageView),
                new ControlPropertyMetadata(null, ControlRelation.Measure));
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
        /// 标识 <see cref="ScaleMode"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ScaleModeProperty =
            DependencyProperty.Register(nameof(ScaleMode), typeof(ScaleMode), typeof(ImageView),
                new ControlPropertyMetadata(ScaleMode.ScaleToFit, ControlRelation.Measure));

        private static void _OnScaleModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion


        /// <summary>
        /// 初始化 <see cref="ImageView"/> 的新实例
        /// </summary>
        public ImageView()
        {
        }


        /// <inheritdoc/>
        protected override Rect DrawCore(Rect renderRect)
        {
            if (ImageSource != null)
            {
                GUI.DrawTexture(renderRect, ImageSource, ScaleMode);
            }
            return renderRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Texture2D imageSource = ImageSource;
            if (imageSource is null) { return new Size(0f); }
            if (ScaleMode is ScaleMode.ScaleToFit)
            {
                float scale;
                if (imageSource.width < availableSize.Width && imageSource.height < availableSize.Height)
                {
                    scale = Mathf.Max(availableSize.Width / imageSource.width, availableSize.Height / imageSource.height);
                }
                else if (imageSource.width <= availableSize.Width && imageSource.height > availableSize.Height)
                {
                    scale = availableSize.Height / imageSource.height;
                }
                else if (imageSource.width > availableSize.Width && imageSource.height <= availableSize.Height)
                {
                    scale = availableSize.Width / imageSource.width;
                }
                else
                {
                    scale = Mathf.Min(availableSize.Width / imageSource.width, availableSize.Height / imageSource.height);
                }
                return new Size(imageSource.width * scale, imageSource.height * scale);
            }
            return new Size(imageSource.width, imageSource.height);
        }
    }
}
