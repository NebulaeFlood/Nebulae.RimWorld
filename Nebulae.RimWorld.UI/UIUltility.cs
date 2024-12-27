using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 界面帮助类
    /// </summary>
    public static class UIUltility
    {
        //------------------------------------------------------
        //
        //  Global Variables
        //
        //------------------------------------------------------

        #region Global Variables

        /// <summary>
        /// 游戏窗口客户区高度
        /// </summary>
        public static float ScreenHeight => Verse.UI.screenHeight;

        /// <summary>
        /// 游戏窗口客户区宽度
        /// </summary>
        public static float ScreenWidth => Verse.UI.screenWidth;

        #endregion


        /// <summary>
        /// 计算按照指定对齐方式放置到指定区域后的矩形
        /// </summary>
        /// <param name="rect">要放置的矩形</param>
        /// <param name="availableArea">放置的区域</param>
        /// <param name="horizontalAlignment">水平方向对齐方式</param>
        /// <param name="verticalAlignment">垂直方向对齐方式</param>
        /// <returns>放置后的矩形。</returns>
        public static Rect AlignRectToArea(this Rect rect, Rect availableArea, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
            => AlignRectToArea(rect, availableArea, horizontalAlignment, verticalAlignment);

        /// <summary>
        /// 计算按照指定对齐方式放置到指定区域后的矩形
        /// </summary>
        /// <param name="rectSize">要放置的矩形大小</param>
        /// <param name="availableArea">放置的区域</param>
        /// <param name="horizontalAlignment">水平方向对齐方式</param>
        /// <param name="verticalAlignment">垂直方向对齐方式</param>
        /// <returns>放置后的矩形。</returns>
        public static Rect AlignRectToArea(this Size rectSize, Rect availableArea, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
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
        /// 将两个矩形合并
        /// </summary>
        /// <param name="rect">要合并的矩形</param>
        /// <param name="anotherRect">被合并的矩形</param>
        /// <returns>合并后的矩形。</returns>
        public static Rect CombineWith(this Rect rect, Rect anotherRect)
        {
            rect.x = Mathf.Min(rect.x, anotherRect.x);
            rect.y = Mathf.Min(rect.y, anotherRect.y);
            rect.width = Mathf.Max(rect.x + rect.width, anotherRect.x + anotherRect.width) - rect.x;
            rect.height = Mathf.Max(rect.y + rect.height, anotherRect.y + anotherRect.height) - rect.y;
            return rect;
        }

        /// <summary>
        /// 将 <paramref name="rect"/> 限定在 <paramref name="bounds"/> 内部
        /// </summary>
        /// <param name="rect">要限定的矩形</param>
        /// <param name="bounds">被限定矩形的边界</param>
        /// <returns>被限定后的矩形。</returns>
        public static Rect ConstrainToBounds(this Rect rect, Rect bounds)
        {
            rect.x = Mathf.Max(rect.x, bounds.x);
            rect.y = Mathf.Max(rect.y, bounds.y);
            rect.width = Mathf.Min(rect.width, bounds.width);
            rect.height = Mathf.Min(rect.height, bounds.height);
            return rect;
        }

        /// <summary>
        /// 判断矩形是否仅有位置发生变化
        /// </summary>
        /// <param name="rect">要判断的矩形</param>
        /// <param name="previousRect">要比较的矩形</param>
        /// <returns>矩形是否仅有位置发生变化。</returns>
        public static bool IsPositionOnlyChanged(this Rect rect, Rect previousRect)
        {
            return rect.width == previousRect.width && rect.height == previousRect.height && (rect.x != previousRect.x || rect.y != previousRect.y);
        }

        /// <summary>
        /// 判断按钮交互结果是否能触发按钮
        /// </summary>
        /// <param name="result">按钮交互结果</param>
        /// <returns>按钮交互结果是否能触发按钮。</returns>
        public static bool IsPressed(this Widgets.DraggableResult result)
        {
            return result is Widgets.DraggableResult.Pressed || result is Widgets.DraggableResult.DraggedThenPressed;
        }

        /// <summary>
        /// 判断矩形是否仅有尺寸发生变化
        /// </summary>
        /// <param name="rect">要判断的矩形</param>
        /// <param name="previousRect">要比较的矩形</param>
        /// <returns>矩形是否仅有尺寸发生变化。</returns>
        public static bool IsSizeOnlyChanged(this Rect rect, Rect previousRect)
        {
            return rect.x == previousRect.x && rect.y == previousRect.y && (rect.width != previousRect.width || rect.height != previousRect.height);
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
            rect.x += offsetX;
            rect.y += offsetY;
            return rect;
        }

        /// <summary>
        /// 对矩形 X 坐标应用偏移
        /// </summary>
        /// <param name="rect">要偏移坐标的矩形</param>
        /// <param name="offsetValue">X 坐标偏移量</param>
        /// <returns>坐标偏移后的矩形。</returns>
        public static Rect OffsetX(this Rect rect, float offsetValue)
        {
            rect.x += offsetValue;
            return rect;
        }

        /// <summary>
        /// 对矩形 Y 坐标应用偏移
        /// </summary>
        /// <param name="rect">要偏移坐标的矩形</param>
        /// <param name="offsetValue">Y 坐标偏移量</param>
        /// <returns>坐标偏移后的矩形。</returns>
        public static Rect OffsetY(this Rect rect, float offsetValue)
        {
            rect.y += offsetValue;
            return rect;
        }
    }
}
