using Nebulae.RimWorld.UI.Controls;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 文本帮助类
    /// </summary>
    public static class TextUtility
    {
        private static readonly GUIContent _contentCache = new GUIContent();
        private static readonly GUIStyle[] _inputBoxStyles = new GUIStyle[12];


        static TextUtility()
        {
            GUIStyle style;

            for (int i = 0; i < 3; i++)
            {
                style = Text.textAreaStyles[i];

                _inputBoxStyles[i] = new GUIStyle(style)
                {
                    hover = style.focused,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal = style.focused,
                    richText = true
                };

                _inputBoxStyles[i + 3] = new GUIStyle(style)
                {
                    focused = style.normal,
                    hover = style.normal,
                    margin = new RectOffset(0, 0, 0, 0),
                    richText = true
                };
            }

            for (int i = 0; i < 3; i++)
            {
                style = Text.textFieldStyles[i];

                _inputBoxStyles[i + 6] = new GUIStyle(style)
                {
                    hover = style.focused,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal = style.focused,
                    richText = true
                };

                _inputBoxStyles[i + 9] = new GUIStyle(style)
                {
                    focused = style.normal,
                    hover = style.normal,
                    margin = new RectOffset(0, 0, 0, 0),
                    richText = true
                };
            }
        }


        /// <summary>
        /// 计算文本排成一行的长度
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文本排成一行的长度</returns>
        public static float CalculateLength(this string text, GameFont fontSize)
        {
            _contentCache.text = text.StripTags();
            return Text.fontStyles[(int)fontSize].CalcSize(_contentCache).x;
        }

        /// <summary>
        /// 计算文本在一定长度下排布后的高度
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="availableLength">文本允许的最大长度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文本排列后的高度</returns>
        public static Size CalculateSize(this string text, float availableLength, GameFont fontSize)
        {
            _contentCache.text = text.StripTags();
            return new Size(availableLength,
                Text.fontStyles[(int)fontSize].CalcHeight(_contentCache, availableLength));
        }

        /// <summary>
        /// 计算文本输入框在一定长度下排布后的尺寸
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="availableLength">文本允许的最大长度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <returns>文本输入框的尺寸。</returns>
        public static Size CalculateTextBoxSize(this string text, float availableLength, GameFont fontSize, bool wrapText)
        {
            GUIStyle style = wrapText
                ? _inputBoxStyles[(int)fontSize]
                : _inputBoxStyles[(int)fontSize + 6];

            return new Size(availableLength, style.CalcHeight(new GUIContent(text), availableLength));
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="text">要设置的文本</param>
        /// <param name="color">要设置的颜色。格式详见 Unity 富文本。</param>
        /// <returns>设置好颜色的文本</returns>
        public static string Colorize(this string text, string color) => string.Format("<color=#{0}>{1}</color>", color, text);

        /// <summary>
        /// 绘制文本输入框
        /// </summary>
        /// <param name="text">输入框内的文字</param>
        /// <param name="renderRect">输入框绘制区域</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="drawHighlight">是否绘制高亮效果</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <returns>用户输入后的文字。</returns>
        public static string DrawTextBox(
            this string text,
            Rect renderRect,
            GameFont fontSize,
            bool drawHighlight,
            bool wrapText)
        {
            int index = (int)fontSize;

            if (!drawHighlight)
            {
                index += 3;
            }

            return wrapText
                ? GUI.TextArea(renderRect, text, _inputBoxStyles[index])
                : GUI.TextField(renderRect, text, _inputBoxStyles[index + 6]);
        }

        /// <summary>
        /// 获取字体的高度
        /// </summary>
        /// <param name="fontSize">要获取高度的字体</param>
        /// <returns>字体的高度</returns>
        public static float GetHeight(this GameFont fontSize) => Text.LineHeightOf(fontSize);
    }
}
