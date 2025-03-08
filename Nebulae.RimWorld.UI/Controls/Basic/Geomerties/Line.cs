using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Data;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic.Geomerties
{
    /// <summary>
    /// 线条图案
    /// </summary>
    public class Line : FrameworkControl
    {
        private Color _color = Color.white;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 线条的颜色
        /// </summary>
        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        #region Thickness
        /// <summary>
        /// 获取或设置线条的粗细
        /// </summary>
        public float Thickness
        {
            get { return (float)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Thickness"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(float), typeof(Line),
                new ControlPropertyMetadata(1f, ControlRelation.Measure));
        #endregion

        #region Orientation
        /// <summary>
        /// 获取或设置线条的方向
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Orientation"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Line),
                new ControlPropertyMetadata(Orientation.Horizontal, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="Line"/> 的新实例
        /// </summary>
        public Line()
        {
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            Color color = GUI.color;
            GUI.color = _color * color;

            GUI.DrawTexture(RenderRect, BaseContent.WhiteTex);

            GUI.color = color;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            if (Orientation is Orientation.Horizontal)
            {
                return new Size(availableSize.Width, Thickness);
            }
            else
            {
                return new Size(Thickness, availableSize.Height);
            }
        }
    }
}
