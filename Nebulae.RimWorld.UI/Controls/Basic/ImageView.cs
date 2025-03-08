using Nebulae.RimWorld.UI.Data;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 图片控件
    /// </summary>
    public class ImageView : FrameworkControl
    {
        private Color _compositionColor = Color.white;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 图片混合色
        /// </summary>
        public Color CompositionColor
        {
            get => _compositionColor;
            set => _compositionColor = value;
        }

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
        #endregion

        #endregion



        /// <summary>
        /// 初始化 <see cref="ImageView"/> 的新实例
        /// </summary>
        public ImageView()
        {
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            if (ImageSource is Texture2D image)
            {
                Color color = GUI.color;
                Color compositionColor = new Color(
                    _compositionColor.r,
                    _compositionColor.g,
                    _compositionColor.b,
                    _compositionColor.a * color.a);

                GUI.color = compositionColor;
                GUI.DrawTexture(RenderRect, image, ScaleMode);
                GUI.color = color;
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            availableSize = base.MeasureCore(availableSize);

            if (!(ImageSource is Texture2D imageSource))
            {
                return Size.Empty;
            }

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

        #endregion
    }
}
