using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Nebulae.RimWorld.UI.Core.Events
{
    /// <summary>
    /// 路由事件的标识
    /// </summary>
    public sealed class RoutedEvent : Singleton<RoutedEvent>
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// <see cref="RoutedEvent"/> 的数据类型
        /// </summary>
        /// <remarks>必须为 <see cref="RoutedEventArgs"/> 或其派生类。</remarks>
        public readonly Type ArgType;

        /// <summary>
        /// <see cref="RoutedEvent"/> 的路由策略
        /// </summary>
        public readonly RoutingStrategy Strategy;

        #endregion


        private RoutedEvent(string name, RoutingStrategy strategy, Type ownerType, Type argType)
            : base(name, ownerType)
        {
            ArgType = argType;
            Strategy = strategy;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 注册路由事件
        /// </summary>
        /// <param name="name">路由事件名称</param>
        /// <param name="strategy">路由策略</param>
        /// <param name="ownerType">拥有事件的类型</param>
        /// <param name="argType">事件数据的类型</param>
        /// <returns><see cref="RoutedEvent"/> 的新实例。</returns>
        /// <exception cref="ArgumentException">当 <paramref name="name"/> 为空或 <paramref name="argType"/> 不是 <see cref="RoutedEventArgs"/> 或其子类型时发生。</exception>
        /// <exception cref="InvalidEnumArgumentException">当 <paramref name="strategy"/> 不是已定义的任何策略时发生。</exception>
        /// <exception cref="ArgumentNullException">当 <paramref name="argType"/> 或 <paramref name="ownerType"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="InvalidOperationException">当目标路由事件已被注册时发生。</exception>
        public static RoutedEvent Register(string name, RoutingStrategy strategy, Type ownerType, Type argType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Event Name cannot be null or whitespace.", nameof(name));
            }

            if (strategy != RoutingStrategy.Tunnel
                && strategy != RoutingStrategy.Bubble
                && strategy != RoutingStrategy.Direct
                && strategy != RoutingStrategy.TopHit)
            {
                throw new InvalidEnumArgumentException(nameof(strategy), (int)strategy, typeof(RoutingStrategy));
            }

            if (ownerType is null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            if (argType is null)
            {
                throw new ArgumentNullException(nameof(argType));
            }

            if (argType != ArgBaseType && !argType.IsSubclassOf(ArgBaseType))
            {
                throw new ArgumentException($"Argument ownerType must be {ArgBaseType} or derived ownerType while provided ownerType is {argType.Name}.", nameof(argType));
            }

            if (Exist(name, ownerType))
            {
                throw new InvalidOperationException($"The event '{ownerType}.{name}' has already been registered.");
            }

            return new RoutedEvent(name, strategy, ownerType, argType);
        }


        /// <summary>
        /// 注册类处理程序
        /// </summary>
        /// <param name="ownerType">拥有处理程序的类型</param>
        /// <param name="handler">处理程序</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="ownerType"/> 或 <paramref name="handler"/> 为 <see langword="null"/> 时发生。</exception>
        /// <remarks>每个类只能注册一个处理程序，必须在静态构造函数中注册处理程序。</remarks>
        public void RegisterClassHandler(Type ownerType, Delegate handler)
        {
            if (ownerType is null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var method = handler.Method;

            if (!method.IsStatic)
            {
                throw new ArgumentException($"Class handler for '{OwnerType}.{Name}' of ownerType '{ownerType}' must be static.", nameof(handler));
            }

            if (!_classHandlers.TryAdd(ownerType, RoutedEventHandlerFactory.Convert(handler, ArgType)))
            {
                throw new InvalidOperationException($"Class handler for '{OwnerType}.{Name}' of ownerType '{ownerType}' has been registered.");
            }

            _classHandlersMap.Clear();
        }

        /// <summary>
        /// 返回路由事件的名称
        /// </summary>
        /// <returns>路由事件的名称。</returns>
        public override string ToString() => Name;

        #endregion


        internal bool TryGetClassHandler(Type ownerType, out IRoutedEventHandler handler)
        {
            if (_classHandlersMap.TryGetValue(ownerType, out handler))
            {
                return true;
            }

            if (_classHandlers.TryGetValue(ownerType, out handler))
            {
                _classHandlersMap[ownerType] = handler;
                return true;
            }

            var parentType = ownerType.BaseType;

            while (parentType != null)
            {
                if (_classHandlers.TryGetValue(parentType, out handler))
                {
                    _classHandlersMap[ownerType] = handler;
                    return true;
                }

                parentType = parentType.BaseType;
            }

            return false;
        }


        internal static readonly Type ArgBaseType = typeof(RoutedEventArgs);

        private readonly Dictionary<Type, IRoutedEventHandler> _classHandlers = new Dictionary<Type, IRoutedEventHandler>();
        private readonly Dictionary<Type, IRoutedEventHandler> _classHandlersMap = new Dictionary<Type, IRoutedEventHandler>();
    }
}
