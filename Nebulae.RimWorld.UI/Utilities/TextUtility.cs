using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 文本标签绘制程序缓存
    /// </summary>
    /// <param name="text">标签文本</param>
    /// <param name="renderRect">绘制区域</param>
    public delegate void LabelCache(string text, Rect renderRect);


    /// <summary>
    /// 文本帮助类
    /// </summary>
    [StaticConstructorOnStartup]
    public static class TextUtility
    {
        static TextUtility()
        {
            GUIStyle style;

            for (int i = 0; i < 3; i++)
            {
                style = Text.textAreaStyles[i];

                InputBoxStyles[i] = new GUIStyle(style)
                {
                    hover = style.focused,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal = style.focused,
                    richText = true,
                    wordWrap = true
                };

                InputBoxStyles[i + 3] = new GUIStyle(style)
                {
                    focused = style.normal,
                    hover = style.normal,
                    margin = new RectOffset(0, 0, 0, 0),
                    richText = true,
                    wordWrap = true
                };
            }

            for (int i = 0; i < 3; i++)
            {
                style = Text.textFieldStyles[i];

                InputBoxStyles[i + 6] = new GUIStyle(style)
                {
                    hover = style.focused,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal = style.focused,
                    richText = true,
                    wordWrap = false
                };

                InputBoxStyles[i + 9] = new GUIStyle(style)
                {
                    focused = style.normal,
                    hover = style.normal,
                    margin = new RectOffset(0, 0, 0, 0),
                    richText = true,
                    wordWrap = false
                };
            }
        }


        /// <summary>
        /// 计算文本在一定长度下排布后的高度
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="availableLength">文本允许的最大长度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="anchor">文本锚点</param>
        /// <returns>文本排列后的高度</returns>
        /// <remarks>应在主线程调用。</remarks>
        public static float CalculateHeight(this string text, float availableLength, GameFont fontSize, TextAnchor anchor)
        {
            fontSize = CoerceFontSize(fontSize);

            ContentCache.text = text.StripTags();

            var fontStyle = Text.fontStyles[(int)fontSize];
            fontStyle.wordWrap = true;

            return fontStyle.CalcHeight(ContentCache, availableLength);
        }

        /// <summary>
        ///  计算文本的长度
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文本的长度。</returns>
        /// <remarks>应在主线程调用。</remarks>
        public static float CalculateLength(this string text, GameFont fontSize)
        {
            fontSize = CoerceFontSize(fontSize);

            ContentCache.text = text.StripTags();

            var fontStyle = Text.fontStyles[(int)fontSize];
            fontStyle.wordWrap = false;

            return fontStyle.CalcSize(ContentCache).x;
        }

        /// <summary>
        /// 计算文本排成一行的尺寸
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文本排成一行的尺寸。</returns>
        /// <remarks>应在主线程调用。</remarks>
        public static Size CalculateLineSize(this string text, GameFont fontSize)
        {
            fontSize = CoerceFontSize(fontSize);

            ContentCache.text = text.StripTags();

            var fontStyle = Text.fontStyles[(int)fontSize];
            fontStyle.wordWrap = false;

            return new Size(fontStyle.CalcSize(ContentCache).x, fontSize.GetHeight());
        }

        /// <summary>
        /// 计算文本在一定长度下排布后的尺寸
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="availableLength">文本允许的最大长度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文本排列后的尺寸。</returns>
        /// <remarks>应在主线程调用。</remarks>
        public static Size CalculateSize(this string text, float availableLength, GameFont fontSize)
        {
            fontSize = CoerceFontSize(fontSize);

            ContentCache.text = text.StripTags();

            var fontStyle = Text.fontStyles[(int)fontSize];
            fontStyle.wordWrap = true;

            return new Size(availableLength, fontStyle.CalcHeight(ContentCache, availableLength));
        }

        /// <summary>
        /// 计算文本输入框在一定长度下排布后的高度
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="availableLength">文本允许的最大长度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <returns>文本输入框的高度。</returns>
        public static float CalculateTextBoxHeight(this string text, float availableLength, GameFont fontSize, bool wrapText)
        {
            fontSize = CoerceFontSize(fontSize);

            GUIStyle style = wrapText
                ? InputBoxStyles[(int)fontSize]
                : InputBoxStyles[(int)fontSize + 6];

            ContentCache.text = text;

            return style.CalcHeight(ContentCache, availableLength);
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
            fontSize = CoerceFontSize(fontSize);

            GUIStyle style = wrapText
                ? InputBoxStyles[(int)fontSize]
                : InputBoxStyles[(int)fontSize + 6];

            ContentCache.text = text;

            return new Size(availableLength, style.CalcHeight(ContentCache, availableLength));
        }

        /// <summary>
        /// 强制转换字体大小
        /// </summary>
        /// <param name="font">字体大小</param>
        /// <returns>转换后的字体大小。</returns>
        public static GameFont CoerceFontSize(this GameFont font)
        {
            if (font is GameFont.Tiny && !Text.TinyFontSupported)
            {
                return GameFont.Small;
            }

            return font;
        }

        /// <summary>
        /// 强制转换字体大小
        /// </summary>
        /// <param name="d">字体大小将要更改的依赖对象</param>
        /// <param name="baseValue">将要设置的字体大小</param>
        /// <returns>强制转换后的字体大小。</returns>
        /// <remarks>为 <see cref="GameFont"/> 类型的 <see cref="DependencyProperty"/> 的 <see cref="PropertyMetadata"/> 准备的 <see cref="CoerceValueCallback"/>。</remarks>
#pragma warning disable IDE0060 // 删除未使用的参数
        public static object CoerceFontSize(DependencyObject d, object baseValue)
#pragma warning restore IDE0060 // 删除未使用的参数
        {
            if (baseValue is GameFont.Tiny && !Text.TinyFontSupported)
            {
                return GameFont.Small;
            }

            return baseValue;
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="text">要设置的文本</param>
        /// <param name="color">要设置的颜色。格式详见 Unity 富文本。</param>
        /// <returns>设置好颜色的文本</returns>
        public static string Colorize(this string text, string color) => string.Format("<color=#{0}>{1}</color>", color, text);

        /// <summary>
        /// 创建一个文本标签绘制程序缓存
        /// </summary>
        /// <param name="anchor">文本锚点</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns>指定的绘制程序缓存。</returns>
        public static LabelCache CreateLabelDrawer(TextAnchor anchor, GameFont fontSize)
        {
            void Draw(string text, Rect renderRect)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                float scale = Prefs.UIScale;

                if (scale > 1f)
                {
                    var halfScale = scale * 0.5f;

                    if (Mathf.Abs(halfScale - Mathf.Floor(halfScale)) > float.Epsilon)
                    {
                        float Adjust(float coord, float s)
                        {
                            double dCoord = s * coord;
                            return coord - (float)((dCoord - Math.Floor(dCoord)) / s);
                        }

                        renderRect.xMin = Adjust(renderRect.xMin, scale);
                        renderRect.yMin = Adjust(renderRect.yMin, scale);
                        renderRect.xMax = Adjust(renderRect.xMax, scale);
                        renderRect.yMax = Adjust(renderRect.yMax, scale);
                    }
                }

                var fontStyle = Text.fontStyles[(int)fontSize];
                fontStyle.alignment = anchor;
                fontStyle.wordWrap = true;

                GUI.Label(renderRect, text, fontStyle);
            }

            return Draw;
        }

        /// <summary>
        /// 绘制文本标签
        /// </summary>
        /// <param name="text">标签文本</param>
        /// <param name="renderRect">绘制区域</param>
        /// <param name="anchor">文本锚点</param>
        /// <param name="fontSize">字体大小</param>
        public static void DrawLabel(this string text, Rect renderRect, TextAnchor anchor = TextAnchor.MiddleCenter, GameFont fontSize = GameFont.Small)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            float scale = Prefs.UIScale;

            if (scale > 1f)
            {
                var halfScale = scale * 0.5f;

                if (Mathf.Abs(halfScale - Mathf.Floor(halfScale)) > float.Epsilon)
                {
                    float Adjust(float coord, float s)
                    {
                        double dCoord = s * coord;
                        return coord - (float)((dCoord - Math.Floor(dCoord)) / s);
                    }

                    renderRect.xMin = Adjust(renderRect.xMin, scale);
                    renderRect.yMin = Adjust(renderRect.yMin, scale);
                    renderRect.xMax = Adjust(renderRect.xMax, scale);
                    renderRect.yMax = Adjust(renderRect.yMax, scale);
                }
            }

            var fontStyle = Text.fontStyles[(int)fontSize];
            fontStyle.alignment = anchor;
            fontStyle.wordWrap = true;

            GUI.Label(renderRect, text, fontStyle);
        }

        /// <summary>
        /// 绘制文本输入框
        /// </summary>
        /// <param name="text">输入框内的文字</param>
        /// <param name="renderRect">输入框绘制区域</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="drawHighlight">是否绘制高亮效果</param>
        /// <param name="wrapText">输入框文字是否自动换行</param>
        /// <returns>用户输入后的文字。</returns>
        public static string DrawTextBox(this string text, Rect renderRect, GameFont fontSize, bool drawHighlight, bool wrapText)
        {
            int index = (int)fontSize;

            if (!drawHighlight)
            {
                index += 3;
            }

            return wrapText
                ? GUI.TextArea(renderRect, text, InputBoxStyles[index])
                : GUI.TextField(renderRect, text, InputBoxStyles[index + 6]);
        }

        /// <summary>
        /// 获取字体的高度
        /// </summary>
        /// <param name="fontSize">要获取高度的字体</param>
        /// <returns>字体的高度。</returns>
        public static float GetHeight(this GameFont fontSize)
        {
            return (fontSize is GameFont.Tiny && !Text.TinyFontSupported) ? Text.LineHeightOf(GameFont.Small) : Text.LineHeightOf(fontSize);
        }

        /// <summary>
        /// 根据允许的长度截断字符串，被截断的部分将设置为省略号
        /// </summary>
        /// <param name="rawStr">要截断的字符串</param>
        /// <param name="availableLength">允许的长度</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns>截断后的字符串。</returns>
        /// <remarks>应在主线程调用。</remarks>
        public static string Truncate(this string rawStr, float availableLength, GameFont fontSize)
        {
            if (string.IsNullOrEmpty(rawStr))
            {
                return rawStr;
            }

            fontSize = CoerceFontSize(fontSize);

            var fontStyle = Text.fontStyles[(int)fontSize];
            fontStyle.wordWrap = false;

            ContentCache.text = rawStr;

            if (fontStyle.CalcSize(ContentCache).x <= availableLength)
            {
                return rawStr;
            }

            var sb = new StringBuilder();
            var tagStack = new Stack<string>();

            var xmlMatch = XmlRegex.Match(rawStr, 0);
            var tagMatch = TagRegex.Match(rawStr, 0);

            int index = 0;
            var length = rawStr.Length;

            while (index < length)
            {
                if (xmlMatch.Success && xmlMatch.Index == index)
                {
                    var xmlTag = xmlMatch.Value;
                    sb.Append(xmlTag);

                    if (xmlTag.StartsWith("</"))
                    {
                        if (tagStack.Count > 0)
                        {
                            tagStack.Pop();
                        }
                    }
                    else if (xmlTag.StartsWith('<') && !xmlTag.EndsWith("/>"))
                    {
                        // 提取标签名，正则匹配结果类似 <b
                        tagStack.Push(TagNameRegex.Match(xmlTag).Value.Substring(1));
                    }

                    index += xmlTag.Length;
                    xmlMatch = XmlRegex.Match(rawStr, index);
                    continue;
                }
                else if (tagMatch.Success && tagMatch.Index == index)
                {
                    var tag = tagMatch.Value;
                    sb.Append(tag);

                    index += tag.Length;
                    tagMatch = TagRegex.Match(rawStr, index);
                    continue;
                }

                int charLength = char.IsHighSurrogate(rawStr[index]) && index + 1 < length && char.IsLowSurrogate(rawStr[index + 1]) ? 2 : 1;
                ContentCache.text = rawStr.Substring(0, index + charLength) + "...";

                if (fontStyle.CalcSize(ContentCache).x >= availableLength)
                {
                    break;
                }

                sb.Append(rawStr.Substring(index, charLength));

                index += charLength;
            }

            while (tagStack.Count > 0)
            {
                sb.Append("</").Append(tagStack.Pop()).Append('>');
            }

            return sb.Append("...").ToString();
        }

        /// <summary>
        /// 根据允许的尺寸截断字符串，被截断的部分将设置为省略号
        /// </summary>
        /// <param name="rawStr">要截断的字符串</param>
        /// <param name="availableSize">允许的尺寸</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns>截断后的字符串。</returns>
        /// <remarks>应在主线程调用。</remarks>
        public static string Truncate(this string rawStr, Size availableSize, GameFont fontSize)
        {
            if (string.IsNullOrEmpty(rawStr))
            {
                return rawStr;
            }

            fontSize = CoerceFontSize(fontSize);

            var fontStyle = Text.fontStyles[(int)fontSize];
            fontStyle.wordWrap = true;

            ContentCache.text = rawStr;

            if (fontStyle.CalcHeight(ContentCache, availableSize.Width) <= availableSize.Height)
            {
                return rawStr;
            }

            var sb = new StringBuilder();
            var tagStack = new Stack<string>();

            var xmlMatch = XmlRegex.Match(rawStr, 0);
            var tagMatch = TagRegex.Match(rawStr, 0);

            int index = 0;
            var length = rawStr.Length;

            while (index < length)
            {
                if (xmlMatch.Success && xmlMatch.Index == index)
                {
                    var xmlTag = xmlMatch.Value;
                    sb.Append(xmlTag);

                    if (xmlTag.StartsWith("</"))
                    {
                        if (tagStack.Count > 0)
                        {
                            tagStack.Pop();
                        }
                    }
                    else if (xmlTag.StartsWith('<') && !xmlTag.EndsWith("/>"))
                    {
                        // 提取标签名，正则匹配结果类似 <b
                        tagStack.Push(TagNameRegex.Match(xmlTag).Value.Substring(1));
                    }

                    index += xmlTag.Length;
                    xmlMatch = XmlRegex.Match(rawStr, index);
                    continue;
                }
                else if (tagMatch.Success && tagMatch.Index == index)
                {
                    var tag = tagMatch.Value;
                    sb.Append(tag);

                    index += tag.Length;
                    tagMatch = TagRegex.Match(rawStr, index);
                    continue;
                }

                int charLength = char.IsHighSurrogate(rawStr[index]) && index + 1 < length && char.IsLowSurrogate(rawStr[index + 1]) ? 2 : 1;
                ContentCache.text = rawStr.Substring(0, index + charLength) + "...";

                if (fontStyle.CalcHeight(ContentCache, availableSize.Width) >= availableSize.Height)
                {
                    break;
                }

                sb.Append(rawStr.Substring(index, charLength));

                index += charLength;
            }

            while (tagStack.Count > 0)
            {
                sb.Append("</").Append(tagStack.Pop()).Append('>');
            }

            return sb.Append("...").ToString();
        }


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static readonly GUIContent ContentCache = new GUIContent();
        private static readonly GUIStyle[] InputBoxStyles = new GUIStyle[12];

        private static readonly Regex XmlRegex = new Regex("<[^>]*>");
        private static readonly Regex TagRegex = new Regex(@"\([\*\/][^\)]*\)");
        private static readonly Regex TagNameRegex = new Regex(@"<([\w]+)");

        #endregion
    }
}
