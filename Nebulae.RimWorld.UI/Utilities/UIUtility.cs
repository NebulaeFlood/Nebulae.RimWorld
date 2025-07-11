using Nebulae.RimWorld.UI.Automation.Diagnostics;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Diagnostics;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 界面工具类
    /// </summary>
    [DebuggerStepThrough]
    public static class UIUtility
    {
        /// <summary>
        /// 当前是否为 UI 调试模式
        /// </summary>
        public static bool DebugMode;


        /// <summary>
        /// 检查控件状态是否包含指定状态
        /// </summary>
        /// <param name="state">要检查的状态</param>
        /// <param name="targetState">要判断的状态</param>
        /// <returns>若 <paramref name="state"/> 包含 <paramref name="targetState"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool HasState(this ControlState state, ControlState targetState)
        {
            return (state & targetState) != 0;
        }

        /// <summary>
        /// 检查控件状态是否为指定状态
        /// </summary>
        /// <param name="state">要检查的状态</param>
        /// <param name="targetState">要判断的状态</param>
        /// <returns>若 <paramref name="state"/> 处于指定状态，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool IsState(this ControlState state, ControlState targetState)
        {
            return (state & targetState) == targetState;
        }

        /// <summary>
        /// 调试控件
        /// </summary>
        /// <param name="control">要调试的控件</param>
        public static void Debug(this Control control)
        {
            if (control is null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            LogicalTreeDebugWindow.Window.Show(control);
        }

        /// <summary>
        /// 显示指定的窗口
        /// </summary>
        /// <param name="window">要显示的窗口</param>
        public static void Show(this Window window)
        {
            Find.WindowStack.Add(window);
        }


        //------------------------------------------------------
        //
        //  Draw Geometry
        //
        //------------------------------------------------------

        #region Draw Geometry

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="renderRect">绘制边框的区域</param>
        /// <param name="borderBrush">边框画刷</param>
        public static void DrawBorder(Rect renderRect, Texture2D borderBrush)
        {
            float x = renderRect.x;
            float y = renderRect.y;
            float width = renderRect.width;
            float height = renderRect.height;

            // Left
            GUI.DrawTexture(new Rect(x, y, 1f, height), borderBrush);
            // Top
            GUI.DrawTexture(new Rect(x, y, width, 1f), borderBrush);
            // Right
            GUI.DrawTexture(new Rect(x + width - 1f, y, 1f, height), borderBrush);
            // Bottom
            GUI.DrawTexture(new Rect(x, y + height, width, 1f), borderBrush);
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="renderRect">绘制边框的区域</param>
        /// <param name="borderColor">边框颜色</param>
        public static void DrawBorder(Rect renderRect, Color borderColor)
        {
            Color color = GUI.color;
            GUI.color = borderColor * color;

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
            GUI.DrawTexture(new Rect(x, y + height, width, 1f), BaseContent.WhiteTex);

            GUI.color = color;
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="renderRect">绘制边框的区域</param>
        /// <param name="borderThickness">边框粗细</param>
        /// <param name="borderBrush">边框画刷</param>
        public static void DrawBorder(Rect renderRect, Thickness borderThickness, Texture2D borderBrush)
        {
            float x = renderRect.x;
            float y = renderRect.y;
            float width = renderRect.width;
            float height = renderRect.height;

            if (borderThickness != 0f)
            {
                // Left
                GUI.DrawTexture(new Rect(x, y, borderThickness.Left, height), borderBrush);
                // Top
                GUI.DrawTexture(new Rect(x, y, width, borderThickness.Top), borderBrush);
                // Right
                GUI.DrawTexture(new Rect(x + width - borderThickness.Right, y, borderThickness.Right, height), borderBrush);
                // Bottom
                GUI.DrawTexture(new Rect(x, y + height, width, borderThickness.Bottom), borderBrush);
            }
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="renderRect">绘制边框的区域</param>
        /// <param name="borderThickness">边框粗细</param>
        /// <param name="borderColor">边框颜色</param>
        public static void DrawBorder(Rect renderRect, Thickness borderThickness, Color borderColor)
        {
            Color color = GUI.color;
            GUI.color = borderColor * color;

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
                GUI.DrawTexture(new Rect(x, y + height, width, borderThickness.Bottom), BaseContent.WhiteTex);
            }

            GUI.color = color;
        }

        /// <summary>
        /// 绘制线段
        /// </summary>
        /// <param name="renderRect">绘制线段的位置</param>
        /// <param name="lineColor">线段颜色</param>
        public static void DrawLine(Rect renderRect, Color lineColor)
        {
            Color color = GUI.color;
            GUI.color = lineColor * color;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = color;
        }

        /// <summary>
        /// 绘制多条线段
        /// </summary>
        /// <param name="brush">线段画刷</param>
        /// <param name="renderRects">绘制线段的位置</param>
        public static void DrawLines(Texture2D brush, params Rect[] renderRects)
        {
            for (int i = 0; i < renderRects.Length; i++)
            {
                GUI.DrawTexture(renderRects[i], brush);
            }
        }

        /// <summary>
        /// 绘制多条线段
        /// </summary>
        /// <param name="lineColor">线段颜色</param>
        /// <param name="renderRects">绘制线段的位置</param>
        public static void DrawLines(Color lineColor, params Rect[] renderRects)
        {
            Color color = GUI.color;
            GUI.color = lineColor * color;

            for (int i = 0; i < renderRects.Length; i++)
            {
                GUI.DrawTexture(renderRects[i], BaseContent.WhiteTex);
            }

            GUI.color = color;
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="renderRect">绘制矩形的区域</param>
        /// <param name="fillColor">矩形颜色</param>
        public static void DrawRectangle(Rect renderRect, Color fillColor)
        {
            Color color = GUI.color;
            GUI.color = fillColor * color;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = color;
        }

        /// <summary>
        /// 绘制有边框的矩形
        /// </summary>
        /// <param name="renderRect">绘制矩形的区域</param>
        /// <param name="background">背景画刷</param>
        /// <param name="borderBrush">边框画刷</param>
        public static void DrawRectangle(Rect renderRect, Texture2D background, Texture2D borderBrush)
        {
            GUI.DrawTexture(renderRect, background);

            float x = renderRect.x;
            float y = renderRect.y;
            float width = renderRect.width;
            float height = renderRect.height;

            // Left
            GUI.DrawTexture(new Rect(x, y, 1f, height), borderBrush);
            // Top
            GUI.DrawTexture(new Rect(x, y, width, 1f), borderBrush);
            // Right
            GUI.DrawTexture(new Rect(x + width - 1f, y, 1f, height), borderBrush);
            // Bottom
            GUI.DrawTexture(new Rect(x, y + height, width, 1f), borderBrush);
        }

        /// <summary>
        /// 绘制有边框的矩形
        /// </summary>
        /// <param name="renderRect">绘制矩形的区域</param>
        /// <param name="background">背景画刷</param>
        /// <param name="borderColor">边框颜色</param>
        public static void DrawRectangle(Rect renderRect, Texture2D background, Color borderColor)
        {
            GUI.DrawTexture(renderRect, background);

            Color color = GUI.color;
            GUI.color = borderColor * color;

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
            GUI.DrawTexture(new Rect(x, y + height, width, 1f), BaseContent.WhiteTex);

            GUI.color = color;
        }

        /// <summary>
        /// 绘制有边框的矩形
        /// </summary>
        /// <param name="renderRect">绘制矩形的区域</param>
        /// <param name="fillColor">矩形颜色</param>
        /// <param name="borderColor">边框颜色</param>
        public static void DrawRectangle(Rect renderRect, Color fillColor, Color borderColor)
        {
            Color color = GUI.color;
            GUI.color = fillColor * color;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = borderColor * color;

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
            GUI.DrawTexture(new Rect(x, y + height, width, 1f), BaseContent.WhiteTex);

            GUI.color = color;
        }

        /// <summary>
        /// 绘制有边框的矩形
        /// </summary>
        /// <param name="renderRect">绘制矩形的区域</param>
        /// <param name="background">背景画刷</param>
        /// <param name="borderThickness">边框粗细</param>
        /// <param name="borderBrush">边框画刷</param>
        public static void DrawRectangle(Rect renderRect, Texture2D background, Thickness borderThickness, Texture2D borderBrush)
        {
            GUI.DrawTexture(renderRect, background);

            float x = renderRect.x;
            float y = renderRect.y;
            float width = renderRect.width;
            float height = renderRect.height;

            if (borderThickness != 0f)
            {
                // Left
                GUI.DrawTexture(new Rect(x, y, borderThickness.Left, height), borderBrush);
                // Top
                GUI.DrawTexture(new Rect(x, y, width, borderThickness.Top), borderBrush);
                // Right
                GUI.DrawTexture(new Rect(x + width - borderThickness.Right, y, borderThickness.Right, height), borderBrush);
                // Bottom
                GUI.DrawTexture(new Rect(x, y + height, width, borderThickness.Bottom), borderBrush);
            }
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
            Color color = GUI.color;
            GUI.color = fillColor * color;

            GUI.DrawTexture(renderRect, BaseContent.WhiteTex);

            GUI.color = borderColor * color;

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
                GUI.DrawTexture(new Rect(x, y + height, width, borderThickness.Bottom), BaseContent.WhiteTex);
            }

            GUI.color = color;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Layout
        //
        //------------------------------------------------------

        #region Layout

        /// <summary>
        /// 计算按照指定对齐方式将该尺寸的矩形放置到指定区域后的位置
        /// </summary>
        /// <param name="rectSize">要放置的矩形大小</param>
        /// <param name="availableArea">放置的区域</param>
        /// <param name="horizontalAlignment">水平方向对齐方式</param>
        /// <param name="verticalAlignment">垂直方向对齐方式</param>
        /// <returns>矩形放置到指定区域后的位置。</returns>
        public static Rect AlignToArea(this Size rectSize, Rect availableArea, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            float x = availableArea.x;
            float y = availableArea.y;
            float width;
            float height;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    x += (availableArea.width - rectSize.Width) * 0.5f;
                    width = rectSize.Width;
                    break;
                case HorizontalAlignment.Right:
                    x += availableArea.width - rectSize.Width;
                    width = rectSize.Width;
                    break;
                case HorizontalAlignment.Left:
                    width = rectSize.Width;
                    break;
                default:    // Stretch
                    width = availableArea.width;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    y += (availableArea.height - rectSize.Height) * 0.5f;
                    height = rectSize.Height;
                    break;
                case VerticalAlignment.Bottom:
                    y += availableArea.height - rectSize.Height;
                    height = rectSize.Height;
                    break;
                case VerticalAlignment.Top:
                    height = rectSize.Height;
                    break;
                default:    // Stretch
                    height = availableArea.height;
                    break;
            }

            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// 计算两个矩形相交的区域
        /// </summary>
        /// <param name="rect">第一个矩形</param>
        /// <param name="anotherRect">第二个矩形</param>
        /// <returns>俩个矩形的交集。</returns>
        public static Size IntersectSizeWith(this Rect rect, Rect anotherRect)
        {
            float width = Mathf.Min(rect.xMax, anotherRect.xMax) - Mathf.Max(rect.x, anotherRect.x);
            float height = Mathf.Min(rect.yMax, anotherRect.yMax) - Mathf.Max(rect.y, anotherRect.y);

            return new Size(width > 0 ? width : 0, height > 0 ? height : 0);
        }

        /// <summary>
        /// 计算矩形的交集
        /// </summary>
        /// <param name="rect">第一个矩形</param>
        /// <param name="anotherRect">第二个矩形</param>
        /// <returns>两个矩形的交集。</returns>
        public static Rect IntersectWith(this Rect rect, Rect anotherRect)
        {
            float left = Mathf.Max(rect.x, anotherRect.x);
            float top = Mathf.Max(rect.y, anotherRect.y);
            float width = Mathf.Min(rect.xMax, anotherRect.xMax) - left;
            float height = Mathf.Min(rect.yMax, anotherRect.yMax) - top;

            return new Rect(left, top, width > 0 ? width : 0, height > 0 ? height : 0);
        }

        /// <summary>
        /// 将矩形偏移指定的偏移量
        /// </summary>
        /// <param name="rect">要偏移的矩形</param>
        /// <param name="offsetX">矩形在 X 轴的偏移量</param>
        /// <param name="offsetY">矩形在 Y 轴的偏移量</param>
        /// <returns>偏移后的矩形。</returns>
        public static Rect Offset(this Rect rect, float offsetX, float offsetY)
        {
            return new Rect(rect.x + offsetX, rect.y + offsetY, rect.width, rect.height);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        internal static Event CurrentEvent;
        internal static EventType CurrentEventType;

        #endregion
    }
}
