using RimWorld;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nebulae.RimWorld.UI.Core.Events
{
    internal static class RoutedEventHandlerFactory
    {
        public static IRoutedEventHandler Convert(Delegate handler, Type argsType)
        {
            var method = handler.Method;
            var methodParamaters = method.GetParameters();

            if (methodParamaters.Length != 2 || methodParamaters[0].ParameterType != typeof(object) || methodParamaters[1].ParameterType != argsType)
            {
                throw new ArgumentException($"Conversion requires a method with exactly two parameters ({typeof(object)} sender, {argsType} args). " +
                    $"The provided method is ({ConvertParamaters(methodParamaters)}).");
            }

            var key = new Key(method.DeclaringType, argsType);

            return method.IsStatic
                ? RoutedEventHandlerCreators.GetOrAdd(key, CreateStaticCreator).Invoke(handler.Target, method)
                : RoutedEventHandlerCreators.GetOrAdd(key, CreateCreator).Invoke(handler.Target, method);
        }


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

        private static Func<object, MethodInfo, IRoutedEventHandler> CreateCreator(Key key)
        {
            var targetExp = Expression.Parameter(typeof(object), "target");
            var methodExp = Expression.Parameter(typeof(MethodInfo), "method");

            return Expression.Lambda<Func<object, MethodInfo, IRoutedEventHandler>>(
                Expression.Call(
                    typeof(RoutedEventHandler<,>).MakeGenericType(key.OwnerType, key.ArgsType).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static),
                    Expression.Convert(targetExp, key.OwnerType),
                    methodExp
                ),
                targetExp,
                methodExp).Compile();
        }

        private static Func<object, MethodInfo, IRoutedEventHandler> CreateStaticCreator(Key key)
        {
            var targetExp = Expression.Parameter(typeof(object), "target");
            var methodExp = Expression.Parameter(typeof(MethodInfo), "method");

            return Expression.Lambda<Func<object, MethodInfo, IRoutedEventHandler>>(
                Expression.New(typeof(StaticRoutedEventHandler<>).MakeGenericType(key.ArgsType).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(MethodInfo) }, null)
                    , methodExp),
                targetExp,
                methodExp).Compile();
        }

        #endregion


        private static readonly ConcurrentDictionary<Key, Func<object, MethodInfo, IRoutedEventHandler>> RoutedEventHandlerCreators = new ConcurrentDictionary<Key, Func<object, MethodInfo, IRoutedEventHandler>>();


        private readonly struct Key : IEquatable<Key>
        {
            //------------------------------------------------------
            //
            //  Public Fields
            //
            //------------------------------------------------------

            #region Public Fields

            public readonly Type OwnerType;
            public readonly Type ArgsType;

            #endregion


            public Key(Type ownerType, Type argsType)
            {
                OwnerType = ownerType;
                ArgsType = argsType;

                _hashCode = ownerType.GetHashCode() ^ argsType.GetHashCode();
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
                        && ArgsType == other.ArgsType;
                }

                return false;
            }

            public bool Equals(Key other)
            {
                return OwnerType == other.OwnerType
                    && ArgsType == other.ArgsType;
            }

            public override int GetHashCode() => _hashCode;

            #endregion


            private readonly int _hashCode;
        }
    }
}
