using Verse;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public static class LogUtility
    {
        /// <summary>
        /// 将 <paramref name="obj"/> 提交为日志
        /// </summary>
        /// <param name="obj">要提交为日志的对象</param>
        public static void Dump(this object obj) => Log.Message(obj);

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交日志
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="obj">日志内容</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void Message(this string logLabel, object obj, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{logLabel}]</color> {obj}");
        }

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交日志
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="message">日志内容</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void Message(this string logLabel, string message, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{logLabel}]</color> {message}");
        }

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交警告
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="message">警告内容</param>
        public static void Warning(this string logLabel, string message)
        {
            Log.Warning($"[{logLabel}] {message}");
        }
    }
}
