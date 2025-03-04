using Verse;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 文本帮助类
    /// </summary>
    public static class TextUtility
    {
        /// <summary>
        /// 计算文本排成一行的长度
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文本排成一行的长度</returns>
        public static float CalculateLength(this string text, GameFont fontSize)
        {
            GameFont currentFont = Text.Font;

            Text.Font = fontSize;
            float width = Text.CalcSize(text).x;
            Text.Font = currentFont;

            return width;
        }

        /// <summary>
        /// 计算文本在一定长度下排布后的高度
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="availableLength">文本允许的最大长度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文本排列后的高度</returns>
        public static float CalculateHeight(this string text, float availableLength, GameFont fontSize)
        {
            GameFont currentFont = Text.Font;

            Text.Font = fontSize;
            float height = Text.CalcHeight(text, availableLength);
            Text.Font = currentFont;

            return height;
        }

        /// <summary>
        /// 计算文本在一定长度下排布后的行数
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="availableLength">文本允许的最大长度</param>
        /// <param name="fontSize">字体尺寸</param>
        /// <returns>文本的行数</returns>
        public static float CalculateLineCount(this string text, float availableLength, GameFont fontSize)
        {
            GameFont currentFont = Text.Font;

            Text.Font = fontSize;
            float fontHeight = Text.LineHeightOf(fontSize);
            float height = Text.CalcHeight(text, availableLength);
            Text.Font = currentFont;

            return (height / fontHeight).Ceiling();
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="text">要设置的文本</param>
        /// <param name="color">要设置的颜色。格式详见 Unity 富文本。</param>
        /// <returns>设置好颜色的文本</returns>
        public static string Colorize(this string text, string color) => string.Format("<color=#{0}>{1}</color>", color, text);

        /// <summary>
        /// 获取字体的高度
        /// </summary>
        /// <param name="fontSize">要获取高度的字体</param>
        /// <returns>字体的高度</returns>
        public static float GetHeight(this GameFont fontSize) => Text.LineHeightOf(fontSize);

        /// <summary>
        /// 翻译键并修复富文本
        /// </summary>
        /// <returns>翻译后的文字</returns>
        public static string TranslateAndResolve(this string key)
        {
            return key.Translate().Resolve();
        }
    }
}
