using Nebulae.RimWorld.UI.Controls.Basic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Resources
{
    /// <summary>
    /// 画刷工具类
    /// </summary>
    [StaticConstructorOnStartup]
    public static class BrushResources
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
        public static readonly Texture2D DarkerGrey = SolidColorMaterials.NewSolidColorTexture(ColorResources.DarkerGrey);

        /// <summary>
        /// 深灰色画刷
        /// </summary>
        public static readonly Texture2D DarkGrey = SolidColorMaterials.NewSolidColorTexture(ColorResources.DarkGrey);

        /// <summary>
        /// 亮灰色画刷
        /// </summary>
        public static readonly Texture2D LighterGrey = SolidColorMaterials.NewSolidColorTexture(ColorResources.LighterGrey);

        /// <summary>
        /// 浅灰色画刷
        /// </summary>
        public static readonly Texture2D LightGrey = SolidColorMaterials.NewSolidColorTexture(ColorResources.LightGrey);

        /// <summary>
        /// 灰色画刷
        /// </summary>
        public static readonly Texture2D Grey = SolidColorMaterials.NewSolidColorTexture(ColorResources.Grey);

        /// <summary>
        /// 透明画刷
        /// </summary>
        public static readonly Texture2D Transparent = SolidColorMaterials.NewSolidColorTexture(ColorResources.Transparent);

        /// <summary>
        /// 白色画刷
        /// </summary>
        public static readonly Texture2D White = SolidColorMaterials.NewSolidColorTexture(ColorResources.White);

        #endregion


        //------------------------------------------------------
        //
        //  Debug Brushes
        //
        //------------------------------------------------------

        #region Debug Brushes

        /// <summary>
        /// <see cref="Control.ControlRect"/> 边框画刷
        /// </summary>
        public static readonly Texture2D ControlRectBorder = SolidColorMaterials.NewSolidColorTexture(ColorResources.ControlRectBorder);

        /// <summary>
        /// <see cref="Control.DesiredRect"/> 边框画刷
        /// </summary>
        public static readonly Texture2D DesiredRectBorder = SolidColorMaterials.NewSolidColorTexture(ColorResources.DesiredRectBorder);

        /// <summary>
        /// <see cref="FrameworkControl.RenderRect"/> 边框画刷
        /// </summary>
        public static readonly Texture2D RederRectBorder = SolidColorMaterials.NewSolidColorTexture(ColorResources.RederRectBorder);

        /// <summary>
        /// <see cref="FrameworkControl.RegionRect"/> 边框画刷
        /// </summary>
        public static readonly Texture2D RegionRectBorder = SolidColorMaterials.NewSolidColorTexture(ColorResources.RederRectBorder);

        /// <summary>
        /// <see cref="Control.VisibleRect"/> 边框画刷
        /// </summary>
        public static readonly Texture2D VisibleRectBorder = SolidColorMaterials.NewSolidColorTexture(ColorResources.VisibleRectBorder);

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
        public static readonly Texture2D WindowBackground = SolidColorMaterials.NewSolidColorTexture(ColorResources.WindowBackground);

        /// <summary>
        /// 窗口边框画刷
        /// </summary>
        public static readonly Texture2D WindowBorder = SolidColorMaterials.NewSolidColorTexture(ColorResources.WindowBorder);

        #endregion
    }
}
