using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 将委托转化为 <see cref="IWeakEventHandler{TSender, TArgs}"/> 的工厂类
    /// </summary>
    public static class WeakEventHandlerFactory
    {
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将指定的事件处理程序转换为弱事件处理程序
        /// </summary>
        /// <typeparam name="TSender">处理器的 senderType 参数类型</typeparam>
        /// <typeparam name="TArgs">处理器的 argsType 参数类型</typeparam>
        /// <param name="handler">要转换的事件处理器</param>
        /// <returns>由 <paramref name="handler"/> 转换成的弱事件处理器。</returns>
        /// <remarks>事件处理器的参数必须设置为 (<typeparamref name="TSender"/> senderType, <typeparamref name="TArgs"/> argsType)。</remarks>
        public static IWeakEventHandler<TSender, TArgs> Convert<TSender, TArgs>(Action<TSender, TArgs> handler) where TArgs : EventArgs
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return Create<TSender, TArgs>(handler.Target, handler.Method);
        }

        /// <summary>
        /// 将指定的事件处理程序转换为弱事件处理程序
        /// </summary>
        /// <typeparam name="TSender">处理器的 senderType 参数类型</typeparam>
        /// <typeparam name="TArgs">处理器的 argsType 参数类型</typeparam>
        /// <param name="handler">要转换的事件处理器</param>
        /// <returns>由 <paramref name="handler"/> 转换成的弱事件处理器。</returns>
        /// <remarks>事件处理器的参数必须设置为 (<typeparamref name="TSender"/> senderType, <typeparamref name="TArgs"/> argsType)。</remarks>
        public static IWeakEventHandler<TSender, TArgs> Convert<TSender, TArgs>(Delegate handler) where TArgs : EventArgs
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var method = handler.Method;

            VerifyParameters(method, typeof(TSender), typeof(TArgs));
            return Create<TSender, TArgs>(handler.Target, method);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static string ConvertParamaters(ParameterInfo[] parameters)
        {
            if (parameters.Length > 0)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(parameters[0].ParameterType.FullName);
                stringBuilder.Append(' ');
                stringBuilder.Append(parameters[0].Name);

                for (int i = 1; i < parameters.Length; i++)
                {
                    stringBuilder.Append(',');
                    stringBuilder.Append(parameters[i].ParameterType.FullName);
                    stringBuilder.Append(' ');
                    stringBuilder.Append(parameters[i].Name);
                }

                return stringBuilder.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private static IWeakEventHandler<TSender, TArgs> Create<TSender, TArgs>(object owner, MethodInfo method) where TArgs : EventArgs
        {
            if (owner is null)
            {
                return new StaticEventHandler<TSender, TArgs>(method);
            }

            return (IWeakEventHandler<TSender, TArgs>)WeakEventHandlerCreators
                .GetOrAdd(new Key(method.DeclaringType, typeof(TSender), typeof(TArgs)), CreateCreator)
                .Invoke(owner, method);
        }

        private static Func<object, MethodInfo, object> CreateCreator(Key key)
        {
            var targetExp = Expression.Parameter(typeof(object), "target");
            var methodExp = Expression.Parameter(typeof(MethodInfo), "method");

            return Expression.Lambda<Func<object, MethodInfo, object>>(
                Expression.Call(
                    typeof(WeakEventHandler<,,>).MakeGenericType(key.OwnerType, key.SenderType, key.ArgsType).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static),
                    Expression.Convert(targetExp, key.OwnerType),
                    methodExp
                ),
                targetExp,
                methodExp).Compile();
        }

        private static void VerifyParameters(MethodInfo method, Type senderType, Type argsType)
        {
            var methodParamaters = method.GetParameters();

            if (methodParamaters.Length != 2 || methodParamaters[0].ParameterType != senderType || methodParamaters[1].ParameterType != argsType)
            {
                throw new ArgumentException($"Conversion requires a method with exactly two parameters ({senderType} sender, {argsType} args). " +
                    $"The provided method is ({ConvertParamaters(methodParamaters)}).");
            }
        }

        #endregion


        private static readonly ConcurrentDictionary<Key, Func<object, MethodInfo, object>> WeakEventHandlerCreators = new ConcurrentDictionary<Key, Func<object, MethodInfo, object>>();


        private readonly struct Key : IEquatable<Key>
        {
            //------------------------------------------------------
            //
            //  Public Fields
            //
            //------------------------------------------------------

            #region Public Fields

            public readonly Type OwnerType;
            public readonly Type SenderType;
            public readonly Type ArgsType;

            #endregion


            public Key(Type ownerType, Type senderType, Type argsType)
            {
                OwnerType = ownerType;
                SenderType = senderType;
                ArgsType = argsType;

                _hashCode = ownerType.GetHashCode() ^ senderType.GetHashCode() ^ argsType.GetHashCode();
            }


            //------------------------------------------------------
            //
            //  Public Methods
            //
            //------------------------------------------------------

            #region Public Methods

            public override bool Equals(object obj)
            {
                if (obj is Key other)
                {
                    return OwnerType == other.OwnerType
                        && SenderType == other.SenderType
                        && ArgsType == other.ArgsType;
                }

                return false;
            }

            public bool Equals(Key other)
            {
                return OwnerType == other.OwnerType
                    && SenderType == other.SenderType
                    && ArgsType == other.ArgsType;
            }

            public override int GetHashCode() => _hashCode;

            #endregion


            private readonly int _hashCode;
        }
    }
}
