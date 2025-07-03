using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 静态事件处理程序
    /// </summary>
    /// <typeparam name="TSender">处理器的 sender 参数类型</typeparam>
    /// <typeparam name="TArgs">处理器的 args 参数类型</typeparam>
    public sealed class StaticEventHandler<TSender, TArgs> : IWeakEventHandler<TSender, TArgs>, IEquatable<StaticEventHandler<TSender, TArgs>>
        where TArgs : EventArgs
    {
        /// <inheritdoc/>
        public bool IsAlive => true;


        internal StaticEventHandler(MethodInfo method)
        {
            if (!InvocationCache.TryGetValue(method, out _invocation))
            {
                _invocation = (Action<TSender, TArgs>)Delegate.CreateDelegate(typeof(Action<TSender, TArgs>), method);
                InvocationCache[method] = _invocation;
            }

            _method = method;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断当前 <see cref="StaticEventHandler{TSender, TArgs}"/> 是否与指定的对象等效
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

            if (obj is MethodInfo method)
            {
                return Equals(method);
            }

            if (obj is StaticEventHandler<TSender, TArgs> other)
            {
                return Equals(other);
            }

            return false;
        }

        /// <summary>
        /// 判断当前 <see cref="StaticEventHandler{TSender, TArgs}"/> 是否与指定的 <see cref="Delegate"/> 等效
        /// </summary>
        /// <param name="other">要判断的 <see cref="Delegate"/></param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(Delegate other)
        {
            return !(other is null) && _method.Equals(other.Method);
        }

        /// <summary>
        /// 判断当前 <see cref="StaticEventHandler{TSender, TArgs}"/> 是否与指定的 <see cref="MethodInfo"/> 等效
        /// </summary>
        /// <param name="other">要判断的 <see cref="MethodInfo"/></param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(MethodInfo other)
        {
            return !(other is null) && _method.Equals(other);
        }

        /// <summary>
        /// 判断当前 <see cref="StaticEventHandler{TSender, TArgs}"/> 是否与指定的 <see cref="StaticEventHandler{TSender, TArgs}"/> 等效
        /// </summary>
        /// <param name="other">要判断的 <see cref="StaticEventHandler{TSender, TArgs}"/></param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(StaticEventHandler<TSender, TArgs> other)
        {
            return !(other is null) && _method.Equals(other._method);
        }

        /// <summary>
        /// 获取当前 <see cref="StaticEventHandler{TSender, TArgs}"/> 的哈希代码
        /// </summary>
        /// <returns>当前 <see cref="StaticEventHandler{TSender, TArgs}"/> 的哈希代码。</returns>
        public override int GetHashCode() => _method.GetHashCode();

        /// <inheritdoc/>
        public void Invoke(TSender sender, TArgs args) => _invocation(sender, args);

        #endregion


        private static readonly Dictionary<MethodInfo, Action<TSender, TArgs>> InvocationCache = new Dictionary<MethodInfo, Action<TSender, TArgs>>();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Action<TSender, TArgs> _invocation;
        private readonly MethodInfo _method;

        #endregion
    }
}
