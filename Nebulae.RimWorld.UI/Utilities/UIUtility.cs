using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.Utilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 界面工具类
    /// </summary>
    [StaticConstructorOnStartup]
    public static class UIUtility
    {
        /// <summary>
        /// <see cref="Visual.ContentRect"/> 边框颜色
        /// </summary>
        public static readonly Texture2D ContentRectBorderBrush = new Color(0f, 1f, 0f, 1f).ToBrush();

        /// <summary>
        /// <see cref="Visual.DesiredRect"/> 边框颜色
        /// </summary>
        public static readonly Texture2D DesiredRectBorderBrush = new Color(1f, 0.9215686f, 0.0156862f, 1f).ToBrush();

        /// <summary>
        /// 可交互区域边框颜色
        /// </summary>
        public static readonly Texture2D ControlRectBorderBrush = new Color(0f, 0f, 1f, 1f).ToBrush();

        /// <summary>
        /// <see cref="Visual.RenderRect"/> 边框颜色
        /// </summary>
        public static readonly Texture2D RederRectBorderBrush = new Color(1f, 1f, 1f, 1f).ToBrush();


        private static readonly GUIStyle[] _inputBoxStyles = new GUIStyle[12];


        /// <summary>
        /// UI 界面资源是否已完全加载
        /// </summary>
        public static bool SourceInitialized { get; internal set; }


        static UIUtility()
        {
            InitializeStyles();
        }

        /// <summary>
        /// 计算按照指定对齐方式将该尺寸的矩形放置到指定区域后的位置
        /// </summary>
        /// <param name="rectSize">要放置的矩形大小</param>
        /// <param name="availableArea">放置的区域</param>
        /// <param name="anchor">对齐方式</param>
        /// <returns>矩形放置到指定区域后的位置。</returns>
        public static Rect AlignToArea(
            this Size rectSize,
            Rect availableArea,
            TextAnchor anchor)
        {
            int index = (int)anchor;

            HorizontalAlignment horizontalAlignment;

            if (index % 3 == 0)
            {
                horizontalAlignment = HorizontalAlignment.Left;
            }
            else if (index % 3 == 1)
            {
                horizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                horizontalAlignment = HorizontalAlignment.Right;
            }

            VerticalAlignment verticalAlignment;

            if (index < 3)
            {
                verticalAlignment = VerticalAlignment.Top;
            }
            else if (index < 6)
            {
                verticalAlignment = VerticalAlignment.Center;
            }
            else
            {
                verticalAlignment = VerticalAlignment.Bottom;
            }

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
        /// 计算按照指定对齐方式将该尺寸的矩形放置到指定区域后的位置
        /// </summary>
        /// <param name="rectSize">要放置的矩形大小</param>
        /// <param name="availableArea">放置的区域</param>
        /// <param name="horizontalAlignment">水平方向对齐方式</param>
        /// <param name="verticalAlignment">垂直方向对齐方式</param>
        /// <returns>矩形放置到指定区域后的位置。</returns>
        public static Rect AlignToArea(
            this Size rectSize,
            Rect availableArea,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment)
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
        /// 计算输入框尺寸
        /// </summary>
        /// <param name="text">输入框内的文字</param>
        /// <param name="renderWidth">输入框绘制宽度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="isReadOnly">输入框是否为只读状态</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <returns>输入框尺寸。</returns>
        public static Size CalculateInputBoxSize(string text, float renderWidth, GameFont fontSize, bool isReadOnly, bool wrapText)
        {
            int index = (int)fontSize;

            if (isReadOnly)
            {
                index += 3;
            }

            GUIStyle style = wrapText
                ? _inputBoxStyles[index]
                : _inputBoxStyles[index + 6];

            return new Size(renderWidth, style.CalcHeight(new GUIContent(text), renderWidth));
        }

        /// <summary>
        /// 计算文字排成一行需要的尺寸
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文字排成一行需要的尺寸。</returns>
        public static Size CalculateLineSize(this string text, GameFont fontSize)
        {
            return new Size(text.CalculateLength(fontSize), Text.LineHeightOf(fontSize));
        }

        /// <summary>
        /// 计算文字的尺寸
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="availableLength">文字要排成的指定长度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文字的尺寸。</returns>
        public static Size CalculateSize(this string text, float availableLength, GameFont fontSize)
        {
            return new Size(availableLength, text.CalculateHeight(availableLength, fontSize));
        }

        /// <summary>
        /// 在一个新位置绘制控件的副本
        /// </summary>
        /// <param name="control">要绘制的控件</param>
        /// <param name="renderRect">绘制副本的位置</param>
        /// <param name="opacity">副本的不透明度</param>
        public static void DrawAt(this Visual control, Rect renderRect, float opacity = 1f)
        {
            float x = control.RenderRect.x;
            float y = control.RenderRect.y;
            float width = x + renderRect.width;
            float height = y + renderRect.height;

            GUI.BeginClip(renderRect);
            GUI.BeginGroup(new Rect(-x, -y, width, height));

            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            Color opacityColor = new Color(1f, 1f, 1f, opacity);

            GUI.color = opacityColor * color;
            GUI.contentColor = opacityColor * contentColor;
            control.Draw();
            GUI.color = color;
            GUI.contentColor = contentColor;

            GUI.EndGroup();
            GUI.EndClip();
        }

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
            GUI.DrawTexture(new Rect(x, y + height - 1f, width, 1f), borderBrush);
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
            GUI.DrawTexture(new Rect(x, y + height - 1f, width, 1f), BaseContent.WhiteTex);

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
                GUI.DrawTexture(new Rect(x, y + height - borderThickness.Bottom, width, borderThickness.Bottom), borderBrush);
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
                GUI.DrawTexture(new Rect(x, y + height - borderThickness.Bottom, width, borderThickness.Bottom), BaseContent.WhiteTex);
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
            GUI.DrawTexture(new Rect(x, y + height - 1f, width, 1f), borderBrush);
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
            GUI.DrawTexture(new Rect(x, y + height - 1f, width, 1f), BaseContent.WhiteTex);

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
            GUI.DrawTexture(new Rect(x, y + height - 1f, width, 1f), BaseContent.WhiteTex);

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
                GUI.DrawTexture(new Rect(x, y + height - borderThickness.Bottom, width, borderThickness.Bottom), borderBrush);
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
                GUI.DrawTexture(new Rect(x, y + height - borderThickness.Bottom, width, borderThickness.Bottom), BaseContent.WhiteTex);
            }

            GUI.color = color;
        }

        /// <summary>
        /// 绘制输入框
        /// </summary>
        /// <param name="renderRect">绘制控件的位置</param>
        /// <param name="text">输入框内的文字</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="isReadOnly">输入框是否为只读状态</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <returns>用户输入后的文字。</returns>
        public static string DrawInputBox(Rect renderRect, string text, GameFont fontSize, bool isReadOnly, bool wrapText)
        {
            int index = (int)fontSize;

            if (isReadOnly)
            {
                index += 3;
            }

            return wrapText
                ? GUI.TextArea(renderRect, text, _inputBoxStyles[index])
                : GUI.TextField(renderRect, text, _inputBoxStyles[index + 6]);
        }

        /// <summary>
        /// 格式化控件尺寸
        /// </summary>
        /// <param name="size">要格式化的尺寸</param>
        /// <returns>格式化后的尺寸</returns>
        public static float FormatControlSize(this float size)
        {
            if (size < 0f || float.IsInfinity(size) || float.IsNaN(size))
            {
                return 0f;
            }

            return size;
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

        /// <summary>
        /// 显示控件预览
        /// </summary>
        /// <param name="control">要显示预览的控件</param>
        /// <param name="xOffset">预览窗口在 x 坐标的偏移量</param>
        /// <param name="yOffset">预览窗口在 y 坐标的偏移量</param>
        /// <param name="opacity">预览的不透明度</param>
        public static void ShowPreview(
            this Visual control,
            float xOffset = 16f,
            float yOffset = 16f,
            float opacity = 0.8f)
        {
            ShowPreview(control, control.RenderRect, xOffset, yOffset, opacity);
        }

        /// <summary>
        /// 显示控件预览
        /// </summary>
        /// <param name="control">要显示预览的控件</param>
        /// <param name="bounds">预览窗口大小</param>
        /// <param name="xOffset">预览窗口在 x 坐标的偏移量</param>
        /// <param name="yOffset">预览窗口在 y 坐标的偏移量</param>
        /// <param name="opacity">预览的不透明度</param>
        public static void ShowPreview(
            this Visual control,
            Size bounds,
            float xOffset = 16f,
            float yOffset = 16f,
            float opacity = 0.8f)
        {
            Find.WindowStack.ImmediateWindow(
                2351 + control.GetHashCode(),
                new Rect(
                    MouseUtility.CursorPosition.x + xOffset + bounds.Width > Verse.UI.screenWidth
                        ? MouseUtility.CursorPosition.x - bounds.Width
                        : MouseUtility.CursorPosition.x + xOffset,
                    MouseUtility.CursorPosition.y + yOffset,
                    bounds.Width,
                    bounds.Height),
                WindowLayer.Super,
                () => DrawPreviewControl(control, bounds, opacity),
                doBackground: false,
                shadowAlpha: 0f);
        }


        internal static void InitializeStyles()
        {
            GUIStyle style;

            for (int i = 0; i < 3; i++)
            {
                style = new GUIStyle(Text.textAreaStyles[i]) { richText = true };
                _inputBoxStyles[i] = style;
                _inputBoxStyles[i + 3] = new GUIStyle(style)
                {
                    focused = style.normal,
                    hover = style.normal
                };
            }

            for (int i = 0; i < 3; i++)
            {
                style = new GUIStyle(Text.textFieldStyles[i]) { richText = true };
                _inputBoxStyles[i + 6] = style;
                _inputBoxStyles[i + 9] = new GUIStyle(style)
                {
                    focused = style.normal,
                    hover = style.normal
                };
            }
        }


        private static void DrawPreviewControl(Visual control, Size bounds, float opacity)
        {
            float x = control.RenderRect.x;
            float y = control.RenderRect.y;
            float width = x + bounds.Width;
            float height = y + bounds.Height;

            GUI.BeginGroup(new Rect(-x, -y, width, height));

            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            Color opacityColor = new Color(1f, 1f, 1f, opacity);

            Visual.PreviewDraw = true;
            GUI.color = opacityColor * color;
            GUI.contentColor = opacityColor * contentColor;
            control.Draw();
            GUI.color = color;
            GUI.contentColor = contentColor;
            Visual.PreviewDraw = false;

            GUI.EndGroup();
        }
    }
}
