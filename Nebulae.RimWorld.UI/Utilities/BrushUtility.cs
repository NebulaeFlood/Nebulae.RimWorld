using Nebulae.RimWorld.UI.Controls.Basic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 画刷工具类
    /// </summary>
    [StaticConstructorOnStartup]
    public static class BrushUtility
    {
        //------------------------------------------------------
        //
        //  Common Brushes
        //
        //------------------------------------------------------

        #region Common Brushes

        /// <summary>
        /// 暗灰色画刷
        /// </summary>
        public static readonly Texture2D DarkGrey = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f, 1f));

        /// <summary>
        /// 灰色画刷
        /// </summary>
        public static readonly Texture2D Grey = SolidColorMaterials.NewSolidColorTexture(ColorLibrary.Grey);

        /// <summary>
        /// 透明画刷
        /// </summary>
        public static readonly Texture2D Transparent = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0f));

        /// <summary>
        /// 白色画刷
        /// </summary>
        public static readonly Texture2D White = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 1f));

        #endregion


        //------------------------------------------------------
        //
        //  Debug Brushes
        //
        //------------------------------------------------------

        #region Debug Brushes

        /// <summary>
        /// <see cref="Control.ControlRect"/> 边框颜色
        /// </summary>
        public static readonly Texture2D ControlRectBorderBrush = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0f, 1f, 1f));

        /// <summary>
        /// <see cref="Control.DesiredRect"/> 边框颜色
        /// </summary>
        public static readonly Texture2D DesiredRectBorderBrush = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0.9215686f, 0.0156862f, 1f));

        /// <summary>
        /// <see cref="FrameworkControl.RenderRect"/> 边框颜色
        /// </summary>
        public static readonly Texture2D RederRectBorderBrush = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 1f));

        /// <summary>
        /// <see cref="Control.VisibleRect"/> 边框颜色
        /// </summary>
        public static readonly Texture2D VisibleRectBorderBrush = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 1f, 0f, 1f));

        #endregion


        //------------------------------------------------------
        //
        //  Window Brushes
        //
        //------------------------------------------------------

        #region Window Brushes

        /// <summary>
        /// 窗口背景画刷
        /// </summary>
        public static readonly Texture2D WindowBackground = SolidColorMaterials.NewSolidColorTexture(Widgets.WindowBGFillColor);

        /// <summary>
        /// 窗口边框画刷
        /// </summary>
        public static readonly Texture2D WindowBorder = SolidColorMaterials.NewSolidColorTexture(new Color(0.3803922f, 0.4235294f, 0.4f));

        #endregion


        /// <summary>
        /// 将 <see cref="Color"/> 转化为纯色画刷
        /// </summary>
        /// <param name="color">要转化的颜色</param>
        /// <returns><paramref name="color"/> 对应颜色的纯色画刷。</returns>
        public static Texture2D ToBrush(this Color color)
        {
            Texture2D CreateBrush()
            {
                return SolidColorMaterials.NewSolidColorTexture(color);
            }

            return Dispatcher.Invoke(CreateBrush);
        }
    }
}
