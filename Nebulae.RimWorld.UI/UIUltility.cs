using Nebulae.RimWorld.UI.Controls;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 界面帮助类
    /// </summary>
    public static class UIUltility
    {

        /// <summary>
        /// 计算按照指定对齐方式放置到指定区域后的矩形
        /// </summary>
        /// <param name="rect">要放置的矩形</param>
        /// <param name="availableArea">放置的区域</param>
        /// <param name="horizontalAlignment">水平方向对齐方式</param>
        /// <param name="verticalAlignment">垂直方向对齐方式</param>
        /// <returns>放置后的矩形。</returns>
        public static Rect AlignRectToArea(this Rect rect, Rect availableArea, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
            => AlignRectToArea(new Size(rect.width, rect.height), availableArea, horizontalAlignment, verticalAlignment);

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
        /// 获取控件的可显示区域
        /// </summary>
        /// <param name="control">要获取可显示区域的控件</param>
        /// <param name="renderRect">控件绘制的区域</param>
        /// <returns>控件的可显示区域</returns>
        public static Rect CalculateVisiableRect(this Control control, Rect renderRect)
        {
            if (control.IsHolded)
            {
                return control.Container.Segment().IntersectWith(renderRect);
            }
            else
            {
                return renderRect;
            }
        }

        /// <summary>
        /// 获取控件的可显示区域
        /// </summary>
        /// <param name="control">要获取可显示区域的控件</param>
        /// <returns>控件的可显示区域</returns>
        public static Rect CalculateVisiableRect(this Control control)
        {
            if (control.IsHolded)
            {
                return control.Container.Segment().IntersectWith(control.RenderRect);
            }
            else
            {
                return control.RenderRect;
            }
        }

        /// <summary>
        /// 将两个矩形合并
        /// </summary>
        /// <param name="rect">要合并的矩形</param>
        /// <param name="anotherRect">被合并的矩形</param>
        /// <returns>合并后的矩形。</returns>
        public static Rect CombineWith(this Rect rect, Rect anotherRect)
        {
            float left = Mathf.Min(rect.x, anotherRect.x);
            float top = Mathf.Min(rect.y, anotherRect.y);
            float right = Mathf.Max(rect.xMax, anotherRect.xMax);
            float bottom = Mathf.Max(rect.yMax, anotherRect.yMax);
            return new Rect(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// 获取光标在屏幕上的 UI 坐标
        /// </summary>
        /// <returns>光标在屏幕上的 UI 坐标</returns>
        public static Vector2 GetScreenMousePosition()
        {
            Vector2 mousePos = Input.mousePosition / Prefs.UIScale;
            mousePos.y = Verse.UI.screenHeight - mousePos.y;
            return mousePos;
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
            float right = Mathf.Min(rect.xMax, anotherRect.xMax);
            float bottom = Mathf.Min(rect.yMax, anotherRect.yMax);
            return new Rect(left, top, right - left, bottom - top);
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
        /// 判断按钮交互结果是否能触发按钮
        /// </summary>
        /// <param name="result">按钮交互结果</param>
        /// <returns>按钮交互结果是否能触发按钮。</returns>
        public static bool IsPressed(this Widgets.DraggableResult result)
        {
            return result is Widgets.DraggableResult.Pressed
                || result is Widgets.DraggableResult.DraggedThenPressed;
        }

        /// <summary>
        /// 判断矩形是否仅有尺寸发生变化
        /// </summary>
        /// <param name="rect">要判断的矩形</param>
        /// <param name="previousRect">要比较的矩形</param>
        /// <returns>矩形是否仅有尺寸发生变化。</returns>
        public static bool IsSizeOnlyChanged(this Rect rect, Rect previousRect)
        {
            return rect.x == previousRect.x
                && rect.y == previousRect.y
                && (rect.width != previousRect.width || rect.height != previousRect.height);
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
        /// 将 <see cref="bool"/> 类型的值转化为 <see cref="ToggleStatus"/>
        /// </summary>
        /// <param name="isChecked">要转换的值，指示状态是否为 <see cref="ToggleStatus"/></param>
        /// <returns><paramref name="isChecked"/> 转化的 <see cref="ToggleStatus"/></returns>
        public static ToggleStatus ToStatus(this bool isChecked)
        {
            return isChecked
                ? ToggleStatus.Checked
                : ToggleStatus.Unchecked;
        }
    }
}
