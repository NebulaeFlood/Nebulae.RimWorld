using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Core.Events
{
    internal sealed class StaticRoutedEventHandler<T> : IRoutedEventHandler, IEquatable<StaticRoutedEventHandler<T>>
        where T : RoutedEventArgs
    {
        public bool IsAlive => true;


        internal StaticRoutedEventHandler(MethodInfo method)
        {
            _invocation = InvocationCache.GetOrAdd(method, CreateInvocation);
            _method = method;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj is Delegate @delegate)
            {
                return Equals(@delegate);
            }

            if (obj is MethodInfo method)
            {
                return Equals(method);
            }

            if (obj is StaticRoutedEventHandler<T> other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Delegate other)
        {
            return !(other is null) && _method.Equals(other.Method);
        }

        public bool Equals(MethodInfo other)
        {
            return !(other is null) && _method.Equals(other);
        }

        public bool Equals(StaticRoutedEventHandler<T> other)
        {
            return !(other is null) && _method.Equals(other._method);
        }

        public override int GetHashCode() => _method.GetHashCode();

        /// <inheritdoc/>
        public void Invoke(object sender, RoutedEventArgs args) => _invocation(sender, (T)args);

        #endregion


        private static Action<object, T> CreateInvocation(MethodInfo method)
        {
            return (Action<object, T>)Delegate.CreateDelegate(typeof(Action<object, T>), method);
        }


        private static readonly ConcurrentDictionary<MethodInfo, Action<object, T>> InvocationCache = new ConcurrentDictionary<MethodInfo, Action<object, T>>();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Action<object, T> _invocation;
        private readonly MethodInfo _method;

        #endregion
    }
}
