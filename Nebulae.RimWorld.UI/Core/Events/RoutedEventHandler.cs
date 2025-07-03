using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Events
{
    /// <summary>
    /// 路由事件处理程序
    /// </summary>
    /// <param name="sender">正在处理路由事件的对象</param>
    /// <param name="e">事件数据</param>
    public delegate void RoutedEventHandler(object sender, RoutedEventArgs e);


    internal sealed class RoutedEventHandler<TOwner, TArgs> : IRoutedEventHandler, IEquatable<RoutedEventHandler<TOwner, TArgs>>
        where TOwner : class
        where TArgs : RoutedEventArgs
    {
        public bool IsAlive => _owner.TryGetTarget(out _);


        private RoutedEventHandler(TOwner owner, Action<TOwner, object, TArgs> invocation, MethodInfo method)
        {
            _invocation = invocation;
            _method = method;
            _owner = new WeakReference<TOwner>(owner);
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

            if (obj is RoutedEventHandler<TOwner, TArgs> other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Delegate other)
        {
            if (other is null)
            {
                return false;
            }

            var otherMethod = other.Method;

            return !otherMethod.IsStatic
                && otherMethod.Equals(_method)
                && _owner.TryGetTarget(out var owner)
                && ReferenceEquals(owner, other.Target);
        }

        public bool Equals(MethodInfo other)
        {
            return !(other is null) && !other.IsStatic && _method.Equals(other);
        }

        public bool Equals(RoutedEventHandler<TOwner, TArgs> other)
        {
            if (other is null)
            {
                return false;
            }

            if (!ReferenceEquals(_invocation, other._invocation))
            {
                return false;
            }

            return _owner.TryGetTarget(out var owner)
                && other._owner.TryGetTarget(out var otherOwner)
                && ReferenceEquals(owner, otherOwner);
        }

        public override int GetHashCode()
        {
            return _owner.TryGetTarget(out var owner)
                ? owner.GetHashCode() ^ _method.GetHashCode()
                : _method.GetHashCode();
        }

        public void Invoke(object sender, RoutedEventArgs args)
        {
            if (_owner.TryGetTarget(out var owner))
            {
                _invocation(owner, sender, (TArgs)args);
            }
        }

        #endregion


        internal static RoutedEventHandler<TOwner, TArgs> Create(TOwner owner, MethodInfo method)
        {
            if (InvocationCache.TryGetValue(method, out var invocation))
            {
                return new RoutedEventHandler<TOwner, TArgs>(owner, invocation, method);
            }

            invocation = (Action<TOwner, object, TArgs>)method.CreateDelegate(typeof(Action<TOwner, object, TArgs>));
            InvocationCache[method] = invocation;

            return new RoutedEventHandler<TOwner, TArgs>(owner, invocation, method);
        }


        private static readonly Dictionary<MethodInfo, Action<TOwner, object, TArgs>> InvocationCache = new Dictionary<MethodInfo, Action<TOwner, object, TArgs>>();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Action<TOwner, object, TArgs> _invocation;
        private readonly WeakReference<TOwner> _owner;

        private readonly MethodInfo _method;

        #endregion
    }
}
