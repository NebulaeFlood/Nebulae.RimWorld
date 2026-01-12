using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsLog(this object obj)
        {
            if (obj is null)
            {
                return "Null";
            }

            if (obj is string str && str.Length < 1)
            {
                return "System.String.Empty";
            }

            if (obj is Type type)
            {
                return type.AsLog(type.Namespace);
            }

            if (obj is Delegate @delegate)
            {
                var delegateType = @delegate.GetType();
                return $"{delegateType.AsLog(delegateType.Namespace)}({@delegate.Method.Format()})";
            }

            if (obj is MethodInfo method)
            {
                return method.Format();
            }

            if (obj is MemberInfo member)
            {
                var declaringType = member.DeclaringType;
                return $"{declaringType.AsLog(declaringType.Namespace)}.{member.Name}";
            }

            return obj.ToString();
        }

        /// <summary>
        /// 将 <paramref name="type"/> 转化为日志字符
        /// </summary>
        /// <param name="type">要转化为字符的对象</param>
        /// <returns>由 <paramref name="type"/> 转化后的字符。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsLog(this Type type)
        {
            if (type is null)
            {
                return "Unknown";
            }

            return type.AsLog(type.Namespace);
        }

        /// <summary>
        /// 将 <paramref name="delegate"/> 转化为日志字符
        /// </summary>
        /// <param name="delegate">要转化为字符的对象</param>
        /// <returns>由 <paramref name="delegate"/> 转化后的字符。</returns>
        public static string AsLog(this Delegate @delegate)
        {
            if (@delegate is null)
            {
                return $"{typeof(object)}.Null";
            }

            var delegateType = @delegate.GetType();

            return $"{delegateType.AsLog(delegateType.Namespace)}({@delegate.Method.Format()})";
        }

        /// <summary>
        /// 将 <paramref name="method"/> 转化为日志字符
        /// </summary>
        /// <param name="method">要转化为字符的对象</param>
        /// <returns>由 <paramref name="method"/> 转化后的字符。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsLog(this MethodInfo method)
        {
            if (method is null)
            {
                return $"{typeof(object)}.Null";
            }

            return method.Format();
        }

        /// <summary>
        /// 将 <paramref name="field"/> 转化为日志字符
        /// </summary>
        /// <param name="field">要转化为字符的对象</param>
        /// <returns>由 <paramref name="field"/> 转化后的字符。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsLog(this MemberInfo field)
        {
            if (field is null)
            {
                return $"{typeof(object)}.Null";
            }

            var declaringType = field.DeclaringType;

            return $"{declaringType.AsLog(declaringType.Namespace)}.{field.Name}";
        }

        /// <summary>
        /// 将 <paramref name="obj"/> 提交为日志
        /// </summary>
        /// <param name="obj">要提交为日志的对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dump(this object obj)
        {
#if DEBUG
            var message = obj.AsLog();

            Log.Message(message);
            System.Diagnostics.Debug.WriteLine(message);
#else
            Log.Message(obj.AsLog());
#endif
        }

        /// <summary>
        /// 将 <paramref name="obj"/> 提交为日志
        /// </summary>
        /// <param name="obj">要提交为日志的对象</param>
        /// <param name="title">日志的标题</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dump(this object obj, string title)
        {
#if DEBUG
            var message = $"[{title}] {obj.AsLog()}";

            Log.Message(message);
            System.Diagnostics.Debug.WriteLine(message);
#else
            Log.Message($"[{title}] {obj.AsLog()}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交错误
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="message">错误内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this string subject, string message, string color = "3F48CCFF")
        {
            Log.Error($"<color=#{color}>[{subject}]</color> {message}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {message}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交错误
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="message">错误内容</param>
        /// <param name="additionalContent">附加内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this string subject, string message, object additionalContent, string color = "3F48CCFF")
        {
            Log.Error($"<color=#{color}>[{subject}]</color> {message}--->\n {additionalContent.AsLog()}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {message}--->\n {additionalContent.AsLog()}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交错误
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="content">错误内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this string subject, object content, string color = "3F48CCFF")
        {
            Log.Error($"<color=#{color}>[{subject}]</color> {content.AsLog()}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {content.AsLog()}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交错误
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="content">错误内容</param>
        /// <param name="additionalContent">附加内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this string subject, object content, object additionalContent, string color = "3F48CCFF")
        {
            Log.Error($"<color=#{color}>[{subject}]</color> {content.AsLog()}--->\n {additionalContent.AsLog()}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {content.AsLog()}--->\n {additionalContent.AsLog()}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交日志
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="message">日志内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Message(this string subject, string message, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{subject}]</color> {message}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {message}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交日志
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="message">日志内容</param>
        /// <param name="additionalContent">附加内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Message(this string subject, string message, object additionalContent, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{subject}]</color> {message}--->\n {additionalContent.AsLog()}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {message}--->\n {additionalContent.AsLog()}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交日志
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="content">日志内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Message(this string subject, object content, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{subject}]</color> {content.AsLog()}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {content.AsLog()}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交日志
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="content">日志内容</param>
        /// <param name="additionalContent">附加内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Message(this string subject, object content, object additionalContent, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{subject}]</color> {content.AsLog()}--->\n {additionalContent.AsLog()}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {content.AsLog()}--->\n {additionalContent.AsLog()}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交成功信息
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="message">日志内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Succeed(this string subject, string message, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{subject}]</color> <color=lime>{message}</color>");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {message}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交成功信息
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="message">日志内容</param>
        /// <param name="additionalContent">附加内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Succeed(this string subject, string message, object additionalContent, string color = "3F48CCFF")
        {
            Log.Message($"<color=#{color}>[{subject}]</color> <color=lime>{message}</color>--->\n {additionalContent.AsLog()}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {message}--->\n {additionalContent.AsLog()}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交警告
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="message">警告内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warning(this string subject, string message, string color = "3F48CCFF")
        {
            Log.Warning($"<color=#{color}>[{subject}]</color> {message}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {message}");
#endif
        }

        /// <summary>
        /// 以 <paramref name="subject"/> 为主语，提交警告
        /// </summary>
        /// <param name="subject">主语</param>
        /// <param name="message">警告内容</param>
        /// <param name="additionalContent">附加内容</param>
        /// <param name="color"><paramref name="subject"/> 要设置的颜色。格式详见 Unity 富文本。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warning(this string subject, string message, object additionalContent, string color = "3F48CCFF")
        {
            Log.Warning($"<color=#{color}>[{subject}]</color> {message}--->\n {additionalContent.AsLog()}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{subject}] {message}--->\n {additionalContent.AsLog()}");
#endif
        }


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

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

        private static string Format(this MethodInfo method)
        {
            var parameters = method.GetParameters();

            var parameterStrings = new string[parameters.Length];

            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                var parameter = parameters[i];

                if (string.IsNullOrEmpty(parameter.Name))
                {
                    parameterStrings[i] = parameter.ParameterType.AsLog();
                }
                else
                {
                    parameterStrings[i] = $"{parameter.ParameterType.AsLog()} {parameter.Name}";
                }
            }

            return $"{method.ReturnType.AsLog()} {method.DeclaringType.AsLog()}.{method.Name}({string.Join(", ", parameterStrings)})";
        }

        #endregion
    }
}
