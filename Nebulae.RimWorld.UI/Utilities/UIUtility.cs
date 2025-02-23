﻿using Nebulae.RimWorld.UI.Controls;
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
        /// <see cref="Control.ContentRect"/> 边框颜色
        /// </summary>
        public static readonly Color ContentRectBorderColor = new Color(0f, 1f, 0f, 1f);

        /// <summary>
        /// <see cref="Control.DesiredRect"/> 边框颜色
        /// </summary>
        public static readonly Color DesiredRectBorderColor = new Color(1f, 0.9215686f, 0.0156862f, 1f);

        /// <summary>
        /// 可交互区域边框颜色
        /// </summary>
        public static readonly Color HitBoxRectBorderColor = new Color(0f, 0f, 1f, 1f);

        /// <summary>
        /// <see cref="Control.RenderRect"/> 边框颜色
        /// </summary>
        public static readonly Color RederRectBorderColor = new Color(1f, 1f, 1f, 1f);


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
        /// 绘制边框
        /// </summary>
        /// <param name="renderRect">绘制边框的区域</param>
        /// <param name="borderColor">边框颜色</param>
        public static void DrawBorder(Rect renderRect, Color borderColor)
        {
            Color currentColor = GUI.color;
            GUI.color = borderColor;

            float x = renderRect.x;
            float y = renderRect.y;
            float width = renderRect.width;
            float height = renderRect.height;

            // Left
            GUI.DrawTexture(new Rect(x, y, 1f, height), BaseContent.WhiteTex);
            // Top
            GUI.DrawTexture(new Rect(x, y, width, 1f), BaseContent.WhiteTex);
            // Right
            GUI.DrawTexture(new Rect(x + width - 1f, y, 1f, height), BaseContent.WhiteTex);
            // Bottom
            GUI.DrawTexture(new Rect(x, y + height - 1f, width, 1f), BaseContent.WhiteTex);

            GUI.color = currentColor;
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="renderRect">绘制边框的区域</param>
        /// <param name="borderThickness">边框粗细</param>
        /// <param name="borderColor">边框颜色</param>
        public static void DrawBorder(Rect renderRect, Thickness borderThickness, Color borderColor)
        {
            Color currentColor = GUI.color;
            GUI.color = borderColor;

            float x = renderRect.x;
            float y = renderRect.y;
            float width = renderRect.width;
            float height = renderRect.height;

            if (borderThickness != 0f)
            {
                // Left
                GUI.DrawTexture(new Rect(x, y, borderThickness.Left, height), BaseContent.WhiteTex);
                // Top
                GUI.DrawTexture(new Rect(x, y, width, borderThickness.Top), BaseContent.WhiteTex);
                // Right
                GUI.DrawTexture(new Rect(x + width - borderThickness.Right, y, borderThickness.Right, height), BaseContent.WhiteTex);
                // Bottom
                GUI.DrawTexture(new Rect(x, y + height - borderThickness.Bottom, width, borderThickness.Bottom), BaseContent.WhiteTex);
            }

            GUI.color = currentColor;
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="renderRect">绘制矩形的区域</param>
        /// <param name="fillColor">矩形颜色</param>
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
        /// <param name="renderRect">绘制矩形的区域</param>
        /// <param name="fillColor">矩形颜色</param>
        /// <param name="borderColor">边框颜色</param>
        public static void DrawRectangle(Rect renderRect, Color fillColor, Color borderColor)
        {
            Color currentColor = GUI.color;
            GUI.color = fillColor;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = borderColor;

            float x = renderRect.x;
            float y = renderRect.y;
            float width = renderRect.width;
            float height = renderRect.height;

            // Left
            GUI.DrawTexture(new Rect(x, y, 1f, height), BaseContent.WhiteTex);
            // Top
            GUI.DrawTexture(new Rect(x, y, width, 1f), BaseContent.WhiteTex);
            // Right
            GUI.DrawTexture(new Rect(x + width - 1f, y, 1f, height), BaseContent.WhiteTex);
            // Bottom
            GUI.DrawTexture(new Rect(x, y + height - 1f, width, 1f), BaseContent.WhiteTex);

            GUI.color = currentColor;
        }

        /// <summary>
        /// 绘制有边框的矩形
        /// </summary>
        /// <param name="renderRect">绘制矩形的区域</param>
        /// <param name="fillColor">矩形颜色</param>
        /// <param name="borderThickness">边框粗细</param>
        /// <param name="borderColor">边框颜色</param>
        public static void DrawRectangle(Rect renderRect, Color fillColor, Thickness borderThickness, Color borderColor)
        {
            Color currentColor = GUI.color;
            GUI.color = fillColor;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = borderColor;

            float x = renderRect.x;
            float y = renderRect.y;
            float width = renderRect.width;
            float height = renderRect.height;

            if (borderThickness != 0f)
            {
                // Left
                GUI.DrawTexture(new Rect(x, y, borderThickness.Left, height), BaseContent.WhiteTex);
                // Top
                GUI.DrawTexture(new Rect(x, y, width, borderThickness.Top), BaseContent.WhiteTex);
                // Right
                GUI.DrawTexture(new Rect(x + width - borderThickness.Right, y, borderThickness.Right, height), BaseContent.WhiteTex);
                // Bottom
                GUI.DrawTexture(new Rect(x, y + height - borderThickness.Bottom, width, borderThickness.Bottom), BaseContent.WhiteTex);
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
