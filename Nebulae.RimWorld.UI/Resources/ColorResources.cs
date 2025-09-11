using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Resources
{
    /// <summary>
    /// 颜色资源
    /// </summary>
    public static class ColorResources
    {
        //------------------------------------------------------
        //
        //  Common Colors
        //
        //------------------------------------------------------

        #region Common Colors

        /// <summary>
        /// 暗灰色
        /// </summary>
        public static readonly Color DarkerGrey = GenColor.FromHex("333333");

        /// <summary>
        /// 深灰色
        /// </summary>
        public static readonly Color DarkGrey = GenColor.FromHex("555555");

        /// <summary>
        /// 亮灰色
        /// </summary>
        public static readonly Color LighterGrey = GenColor.FromHex("CCCCCC");

        /// <summary>
        /// 浅灰色
        /// </summary>
        public static readonly Color LightGrey = GenColor.FromHex("999999");

        /// <summary>
        /// 灰色
        /// </summary>
        public static readonly Color Grey = GenColor.FromHex("777777");

        /// <summary>
        /// 透明
        /// </summary>
        public static readonly Color Transparent = new Color(1f, 1f, 1f, 0f);

        /// <summary>
        /// 白色
        /// </summary>
        public static readonly Color White = new Color(1f, 1f, 1f);

        #endregion


        //------------------------------------------------------
        //
        //  Debug Colors
        //
        //------------------------------------------------------

        #region Debug Colors

        /// <summary>
        /// <see cref="Control.ControlRect"/> 边框颜色
        /// </summary>
        public static readonly Color ControlRectBorder = new Color(0f, 0f, 1f);

        /// <summary>
        /// <see cref="Control.DesiredRect"/> 边框颜色
        /// </summary>
        public static readonly Color DesiredRectBorder = new Color(1f, 0.9215686f, 0.0156862f);

        /// <summary>
        /// <see cref="FrameworkControl.RenderRect"/> 边框颜色
        /// </summary>
        public static readonly Color RederRectBorder = new Color(1f, 1f, 1f);

        /// <summary>
        /// <see cref="FrameworkControl.RegionRect"/> 边框颜色
        /// </summary>
        public static readonly Color RegionRectBorder = new Color(1f, 0.4235294f, 0f);

        /// <summary>
        /// <see cref="Control.VisibleRect"/> 边框颜色
        /// </summary>
        public static readonly Color VisibleRectBorder = new Color(0f, 1f, 0f);

        #endregion


        //------------------------------------------------------
        //
        //  Window Colors
        //
        //------------------------------------------------------

        #region Window Colors

        /// <summary>
        /// 窗口背景颜色
        /// </summary>
        public static readonly Color WindowBackground = Widgets.WindowBGFillColor;

        /// <summary>
        /// 窗口边框颜色
        /// </summary>
        public static readonly Color WindowBorder = new Color(0.3803922f, 0.4235294f, 0.4f);

        #endregion
    }
}
