using System;
using System.Linq;
using Verse;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public static class LogUtility
    {
        /// <summary>
        /// 将 <paramref name="obj"/> 转化为日志字符
        /// </summary>
        /// <param name="obj">要转化为字符的对象</param>
        /// <returns>由 <paramref name="obj"/> 转化后的字符。</returns>
        public static string AsLog(this object obj)
        {
            if (obj is null)
            {
                return $"{typeof(object)}.Null";
            }

            if (obj is string str && str.Length < 1)
            {
                return $"{typeof(string)}.Empty";
            }

            if (obj is Type type)
            {
                return type.AsLog(type.Namespace);
            }

            return obj.ToString();
        }

        /// <summary>
        /// 将 <paramref name="type"/> 转化为日志字符
        /// </summary>
        /// <param name="type">要转化为字符的对象</param>
        /// <returns>由 <paramref name="type"/> 转化后的字符。</returns>
        public static string AsLog(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.AsLog(type.Namespace);
        }

        private static string AsLog(this Type type, string @namespace)
        {
            if (type.IsArray)
            {
                var rank = type.GetArrayRank();

                return type.GetElementType().AsLog(@namespace) + (rank is 1 ? "[]" : $"[{new string(',', rank - 1)}]");
            }

            if (type.IsByRef)
            {
                return type.GetElementType().AsLog(@namespace) + '&';
            }

            if (type.IsPointer)
            {
                return type.GetElementType().AsLog(@namespace) + '*';
            }

            if (!type.IsGenericType)
            {
                return $"{@namespace}.{type.Format(@namespace)}";
            }

            if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                return $"{Nullable.GetUnderlyingType(type).AsLog(@namespace)}?";
            }

            return $"{@namespace}.{type.Format(@namespace).TrimEnd(new char[] { '`', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' })}<{string.Join(", ", type.GetGenericArguments().Select(x => x.AsLog(x.Namespace)))}>";
        }

        private static string Format(this Type type)
        {
            if (type.DeclaringType is null)
            {
                return type.Name;
            }

            return $"{type.DeclaringType.Format()}+{type.Name}";
        }

        private static string Format(this Type type, string @namespace)
        {
            var typeNamespace = type.Namespace;

            if (string.IsNullOrEmpty(@namespace))
            {
                return string.IsNullOrEmpty(typeNamespace)
                    ? type.Format()
                    : $"{typeNamespace}.{type.Format()}";
            }

            if (string.IsNullOrEmpty(typeNamespace) || @namespace.Equals(typeNamespace))
            {
                return type.Format();
            }

            return $"{typeNamespace}.{type.Format()}";
        }

        /// <summary>
        /// 将 <paramref name="obj"/> 提交为日志
        /// </summary>
        /// <param name="obj">要提交为日志的对象</param>
        public static void Dump(this object obj) => Log.Message(obj.AsLog());

        /// <summary>
        /// 将 <paramref name="obj"/> 提交为日志
        /// </summary>
        /// <param name="obj">要提交为日志的对象</param>
        /// <param name="title">日志的标题</param>
        public static void Dump(this object obj, string title) => Log.Message($"[{title}] {obj.AsLog()}");

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交错误
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="message">错误内容</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void Error(this string logLabel, string message, string color = "3F48CCFF")
        {
            Log.Error($"<color=#{color}>[{logLabel}]</color> {message}");
        }

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交错误
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="exception">错误内容</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void Error(this string logLabel, Exception exception, string color = "3F48CCFF")
        {
            Log.Error($"<color=#{color}>[{logLabel}]</color> {exception}");
        }

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交日志
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="obj">日志内容</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void Message(this string logLabel, object obj, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{logLabel}]</color> {obj.AsLog()}");
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
        /// 以 <paramref name="logLabel"/> 为主语，提交成功信息
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="message">日志内容</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void Succeed(this string logLabel, string message, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{logLabel}]</color> <color=lime>{message}</color>");
        }

        /// <summary>
        /// 以 <paramref name="logLabel"/> 为主语，提交警告
        /// </summary>
        /// <param name="logLabel">主语</param>
        /// <param name="message">警告内容</param>
        /// <param name="color"><paramref name="logLabel"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        public static void Warning(this string logLabel, string message, string color = "3F48CCFF")
        {
            Log.Warning($"<color=#{color}>[{logLabel}]</color> {message}");
        }
    }
}
