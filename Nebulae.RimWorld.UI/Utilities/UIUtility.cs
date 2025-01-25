using Nebulae.RimWorld.UI.Controls;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 界面工具类
    /// </summary>
    public static class UIUtility
    {
        /// <summary>
        /// 计算按照指定对齐方式放置到指定区域后的矩形
        /// </summary>
        /// <param name="rectSize">要放置的矩形大小</param>
        /// <param name="availableArea">放置的区域</param>
        /// <param name="horizontalAlignment">水平方向对齐方式</param>
        /// <param name="verticalAlignment">垂直方向对齐方式</param>
        /// <returns>放置后的矩形。</returns>
        public static Rect AlignToArea(this Size rectSize, Rect availableArea, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            Rect alignedRect = new Rect(availableArea.x, availableArea.y, 0f, 0f);

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    alignedRect.x += (availableArea.width - rectSize.Width) * 0.5f;
                    alignedRect.width = rectSize.Width;
                    break;
                case HorizontalAlignment.Right:
                    alignedRect.x += availableArea.width - rectSize.Width;
                    alignedRect.width = rectSize.Width;
                    break;
                case HorizontalAlignment.Left:
                    alignedRect.width = rectSize.Width;
                    break;
                default:    // Stretch
                    alignedRect.width = availableArea.width;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    alignedRect.y += (availableArea.height - rectSize.Height) * 0.5f;
                    alignedRect.height = rectSize.Height;
                    break;
                case VerticalAlignment.Bottom:
                    alignedRect.y += availableArea.height - rectSize.Height;
                    alignedRect.height = rectSize.Height;
                    break;
                case VerticalAlignment.Top:
                    alignedRect.height = rectSize.Height;
                    break;
                default:    // Stretch
                    alignedRect.height = availableArea.height;
                    break;
            }

            return alignedRect;
        }

        /// <summary>
        /// 计算文字排成一行需要的尺寸
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文字排成一行需要的尺寸。</returns>
        public static Size CalculateLineSize(this string text, GameFont fontSize)
        {
            GameFont currentFont = Text.Font;

            Text.Font = fontSize;

            float width = Text.CalcSize(text).x;
            float height = Text.LineHeightOf(fontSize);

            Text.Font = currentFont;
            return new Size(width, height);
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="renderRect"></param>
        /// <param name="fillColor"></param>
        public static void DrawRectangle(Rect renderRect, Color fillColor)
        {
            Color currentColor = GUI.color;
            GUI.color = fillColor;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = currentColor;
        }


        /// <summary>
        /// 绘制有边框的矩形
        /// </summary>
        /// <param name="renderRect"></param>
        /// <param name="fillColor"></param>
        /// <param name="borderThickness"></param>
        /// <param name="borderColor"></param>
        public static void DrawRectangle(Rect renderRect, Color fillColor, Thickness borderThickness, Color borderColor)
        {
            Color currentColor = GUI.color;
            GUI.color = fillColor;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = borderColor;

            if (borderThickness != 0f)
            {
                // Left
                GUI.DrawTexture(new Rect(renderRect.x, renderRect.y, borderThickness.Left, renderRect.height), BaseContent.WhiteTex);
                // Top
                GUI.DrawTexture(new Rect(renderRect.x, renderRect.y, renderRect.width, borderThickness.Top), BaseContent.WhiteTex);
                // Right
                GUI.DrawTexture(new Rect(renderRect.x + renderRect.width - borderThickness.Right, renderRect.y, borderThickness.Right, renderRect.height), BaseContent.WhiteTex);
                // Bottom
                GUI.DrawTexture(new Rect(renderRect.x, renderRect.y + renderRect.height - borderThickness.Bottom, renderRect.width, borderThickness.Bottom), BaseContent.WhiteTex);
            }

            GUI.color = currentColor;
        }


        /// <summary>
        /// 计算两个矩形的交集
        /// </summary>
        /// <param name="rect">第一个矩形</param>
        /// <param name="anotherRect">第二个矩形</param>
        /// <returns>俩个矩形的交集。</returns>
        public static Rect IntersectWith(this Rect rect, Rect anotherRect)
        {
            float left = Mathf.Max(rect.x, anotherRect.x);
            float top = Mathf.Max(rect.y, anotherRect.y);
            float width = Mathf.Min(rect.xMax, anotherRect.xMax) - left;
            float height = Mathf.Min(rect.yMax, anotherRect.yMax) - top;

            return new Rect(left, top, width > 0 ? width : 0, height > 0 ? height : 0);
        }

        /// <summary>
        /// 判断矩形是否仅有位置发生变化
        /// </summary>
        /// <param name="rect">要判断的矩形</param>
        /// <param name="previousRect">要比较的矩形</param>
        /// <returns>矩形是否仅有位置发生变化。</returns>
        public static bool IsPositionOnlyChanged(this Rect rect, Rect previousRect)
        {
            return rect.width == previousRect.width
                && rect.height == previousRect.height
                && (rect.x != previousRect.x || rect.y != previousRect.y);
        }

        /// <summary>
        /// 对矩形坐标应用偏移
        /// </summary>
        /// <param name="rect">要偏移坐标的矩形</param>
        /// <param name="offsetX">X 坐标偏移量</param>
        /// <param name="offsetY">Y 坐标偏移量</param>
        /// <returns>坐标偏移后的矩形。</returns>
        public static Rect Offset(this Rect rect, float offsetX, float offsetY)
        {
            return new Rect(
                rect.x + offsetX,
                rect.y + offsetY,
                rect.width,
                rect.height);
        }

        /// <summary>
        /// 翻转水平排布方式
        /// </summary>
        /// <param name="align">要翻转的排布方式</param>
        /// <returns>翻转后的排布方式</returns>
        public static HorizontalAlignment Reverse(this HorizontalAlignment align)
        {
            switch (align)
            {
                case HorizontalAlignment.Left:
                    return HorizontalAlignment.Right;
                case HorizontalAlignment.Right:
                    return HorizontalAlignment.Left;
                default:
                    return align;
            }
        }

        /// <summary>
        /// 翻转垂直排布方式
        /// </summary>
        /// <param name="align">要翻转的排布方式</param>
        /// <returns>翻转后的排布方式</returns>
        public static VerticalAlignment Reverse(this VerticalAlignment align)
        {
            switch (align)
            {
                case VerticalAlignment.Top:
                    return VerticalAlignment.Bottom;
                case VerticalAlignment.Bottom:
                    return VerticalAlignment.Top;
                default:
                    return align;
            }
        }

        /// <summary>
        /// 如果满足条件，翻转水平排布方式
        /// </summary>
        /// <param name="align">要翻转的排布方式</param>
        /// <param name="condition">是否满足条件</param>
        /// <returns>翻转后的排布方式</returns>
        public static HorizontalAlignment ReverseIf(this HorizontalAlignment align, bool condition)
        {
            if (condition)
            {
                switch (align)
                {
                    case HorizontalAlignment.Left:
                        return HorizontalAlignment.Right;
                    case HorizontalAlignment.Right:
                        return HorizontalAlignment.Left;
                    default:
                        return align;
                }
            }
            else
            {
                return align;
            }
        }

        /// <summary>
        /// 如果满足条件，翻转垂直排布方式
        /// </summary>
        /// <param name="align">要翻转的排布方式</param>
        /// <param name="condition">是否满足条件</param>
        /// <returns>翻转后的排布方式</returns>
        public static VerticalAlignment ReverseIf(this VerticalAlignment align, bool condition)
        {
            if (condition)
            {
                switch (align)
                {
                    case VerticalAlignment.Top:
                        return VerticalAlignment.Bottom;
                    case VerticalAlignment.Bottom:
                        return VerticalAlignment.Top;
                    default:
                        return align;
                }
            }
            else
            {
                return align;
            }
        }
    }
}
