using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Geomerties
{
    /// <summary>
    /// 矩形图案
    /// </summary>
    public class Rectangle : FrameworkControl
    {
        private Color _borderColor = Color.white;
        private Thickness _borderThickness = 1f;
        private Color _fillColor = new ColorInt(10, 12, 14).ToColor;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        /// <summary>
        /// 边框粗细
        /// </summary>
        public Thickness BorderThickness
        {
            get => _borderThickness;
            set => _borderThickness = value;
        }

        /// <summary>
        /// 填充颜色
        /// </summary>
        public Color FillColor
        {
            get => _fillColor;
            set => _fillColor = value;
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="Rectangle"/> 的新实例
        /// </summary>
        public Rectangle()
        {
        }


        /// <inheritdoc/>
        protected override void DrawCore()
        {
            Rect renderRect = RenderRect;

            Color currentColor = GUI.color;
            GUI.color = _fillColor;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = _borderColor;

            if (_borderThickness != 0f)
            {
                // Left
                GUI.DrawTexture(new Rect(renderRect.x, renderRect.y, _borderThickness.Left, renderRect.height), BaseContent.WhiteTex);
                // Top
                GUI.DrawTexture(new Rect(renderRect.x, renderRect.y, renderRect.width, _borderThickness.Top), BaseContent.WhiteTex);
                // Right
                GUI.DrawTexture(new Rect(renderRect.x + renderRect.width - _borderThickness.Right, renderRect.y, _borderThickness.Right, renderRect.height), BaseContent.WhiteTex);
                // Bottom
                GUI.DrawTexture(new Rect(renderRect.x, renderRect.y + renderRect.height - _borderThickness.Bottom, renderRect.width, _borderThickness.Bottom), BaseContent.WhiteTex);
            }

            GUI.color = currentColor;
        }
    }
}
