using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 弱事件处理器
    /// </summary>
    /// <typeparam name="TOwner">拥有源事件处理器的类型</typeparam>
    /// <typeparam name="TSender">处理器的 sender 参数类型</typeparam>
    /// <typeparam name="TArgs">处理器的 args 参数类型</typeparam>
    internal sealed class WeakEventHandler<TOwner, TSender, TArgs> : IEquatable<WeakEventHandler<TOwner, TSender, TArgs>>, IWeakEventHandler<TSender, TArgs>
        where TOwner : class
        where TArgs : EventArgs
    {
        /// <inheritdoc/>
        public bool IsAlive => _owner.TryGetTarget(out _);


        private WeakEventHandler(TOwner owner, Action<TOwner, TSender, TArgs> invocation, MethodInfo method)
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

        /// <summary>
        /// 判断当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 是否与指定的对象等效
        /// </summary>
        /// <param name="obj">要比较的对象</param>
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

            if (obj is MethodInfo method)
            {
                return Equals(method);
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
        /// <param name="other">要比较的委托</param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(Delegate other)
        {
            if (other is null)
            {
                return false;
            }

            _owner.TryGetTarget(out var owner);

            return owner == other.Target
                && _method.Equals(other.Method);
        }

        /// <summary>
        /// 判断当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 是否与指定的 <see cref="MethodInfo"/> 等效
        /// </summary>
        /// <param name="other">要比较 <see cref="MethodInfo"/></param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(MethodInfo other)
        {
            return other is not null
                && _method.Equals(other);
        }

        /// <summary>
        /// 判断当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 是否与指定的 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 等效
        /// </summary>
        /// <param name="other">要比较 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/></param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(WeakEventHandler<TOwner, TSender, TArgs> other)
        {
            if (other is null)
            {
                return false;
            }

            if (_owner.TryGetTarget(out var owner) != other._owner.TryGetTarget(out var otherOwner))
            {
                return false;
            }

            return owner == otherOwner
                && _method.Equals(other._method);
        }

        /// <summary>
        /// 获取当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 的哈希代码
        /// </summary>
        /// <returns>当前 <see cref="WeakEventHandler{TOwner, TSender, TArgs}"/> 的哈希代码。</returns>
        public override int GetHashCode()
        {
            _owner.TryGetTarget(out var owner);
            return HashCode.Combine(owner, _method);
        }

        /// <inheritdoc/>
        public void Invoke(TSender sender, TArgs args)
        {
            if (_owner.TryGetTarget(out var owner))
            {
                _invocation(owner, sender, args);
            }
        }

        /// <inheritdoc/>
        public bool TryGetOwner(out object owner)
        {
            var isAlive = _owner.TryGetTarget(out var directOwner);
            owner = directOwner;
            return isAlive;
        }

        #endregion


        internal static WeakEventHandler<TOwner, TSender, TArgs> Create(TOwner owner, MethodInfo method)
        {
            return new WeakEventHandler<TOwner, TSender, TArgs>(owner, InvocationCache.GetOrAdd(method, CreateInvocation), method);
        }


        private static Action<TOwner, TSender, TArgs> CreateInvocation(MethodInfo method)
        {
            return (Action<TOwner, TSender, TArgs>)method.CreateDelegate(typeof(Action<TOwner, TSender, TArgs>));
        }


        private static readonly ConcurrentDictionary<MethodInfo, Action<TOwner, TSender, TArgs>> InvocationCache = new();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Action<TOwner, TSender, TArgs> _invocation;
        private readonly WeakReference<TOwner> _owner;

        private readonly MethodInfo _method;

        #endregion
    }
}
