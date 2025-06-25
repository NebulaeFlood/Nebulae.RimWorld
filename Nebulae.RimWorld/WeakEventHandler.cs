using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 弱事件处理程序
    /// </summary>
    /// <typeparam name="TOwner">拥有源事件处理器的类型</typeparam>
    /// <typeparam name="TSender">处理器的 sender 参数类型</typeparam>
    /// <typeparam name="TArgs">处理器的 args 参数类型</typeparam>
    public sealed class WeakEventHandler<TOwner, TSender, TArgs> : IEquatable<WeakEventHandler<TOwner, TSender, TArgs>>, IWeakEventHandler<TSender, TArgs>
        where TOwner : class
        where TArgs : EventArgs
    {
        /// <inheritdoc/>
        public bool IsAlive => _isStatic || _owner.TryGetTarget(out _);


        private WeakEventHandler(TOwner owner, Action<TOwner, TSender, TArgs> invocation, MethodInfo method)
        {
            _invocation = invocation;
            _isStatic = owner is null;
            _method = method;
            _owner = _isStatic ? null : new WeakReference<TOwner>(owner);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 是否与指定的对象等效
        /// </summary>
        /// <param name="obj">要判断的对象</param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
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

            if (obj is WeakEventHandler<TOwner, TSender, TArgs> other)
            {
                return Equals(other);
            }

            return false;
        }

        /// <summary>
        /// 判断当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 是否与指定的委托等效
        /// </summary>
        /// <param name="other">要判断的委托</param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(Delegate other)
        {
            if (other is null)
            {
                return false;
            }

            var otherMethod = other.Method;

            if (_isStatic != otherMethod.IsStatic)
            {
                return false;
            }

            if (!otherMethod.Equals(_method))
            {
                return false;
            }

            if (_isStatic)
            {
                return true;
            }

            if (!_owner.TryGetTarget(out var owner))
            {
                return false;
            }

            return ReferenceEquals(owner, other.Target);
        }

        /// <summary>
        /// 判断当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 是否与指定的 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 等效
        /// </summary>
        /// <param name="other">要判断的 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/></param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(WeakEventHandler<TOwner, TSender, TArgs> other)
        {
            if (other is null)
            {
                return false;
            }

            if (_isStatic != other._isStatic)
            {
                return false;
            }

            if (!ReferenceEquals(_invocation, other._invocation))
            {
                return false;
            }

            if (_isStatic)
            {
                return true;
            }

            _owner.TryGetTarget(out var owner);
            other._owner.TryGetTarget(out var otherOwner);

            return ReferenceEquals(owner, otherOwner);
        }

        /// <summary>
        /// 获取当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 的哈希代码
        /// </summary>
        /// <returns>当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 的哈希代码。</returns>
        public override int GetHashCode()
        {
            return _isStatic
                ? _invocation.GetHashCode()
                : (_owner.TryGetTarget(out var owner)
                    ? owner.GetHashCode() ^ _method.GetHashCode()
                    : _method.GetHashCode());
        }

        /// <inheritdoc/>
        public void Invoke(TSender sender, TArgs args)
        {
            if (_isStatic)
            {
                _invocation(null, sender, args);
            }
            else if (_owner.TryGetTarget(out var owner))
            {
                _invocation(owner, sender, args);
            }
        }

        #endregion


        internal static WeakEventHandler<TOwner, TSender, TArgs> Create(TOwner owner, MethodInfo method)
        {
            if (InvocationCache.TryGetValue(method, out var invocation))
            {
                return new WeakEventHandler<TOwner, TSender, TArgs>(owner, invocation, method);
            }

            invocation = (Action<TOwner, TSender, TArgs>)method.CreateDelegate(typeof(Action<TOwner, TSender, TArgs>));
            InvocationCache[method] = invocation;

            return new WeakEventHandler<TOwner, TSender, TArgs>(owner, invocation, method);
        }


        private static readonly Dictionary<MethodInfo, Action<TOwner, TSender, TArgs>> InvocationCache = new Dictionary<MethodInfo, Action<TOwner, TSender, TArgs>>();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Action<TOwner, TSender, TArgs> _invocation;
        private readonly WeakReference<TOwner> _owner;

        private readonly bool _isStatic;
        private readonly MethodInfo _method;

        #endregion
    }
}
