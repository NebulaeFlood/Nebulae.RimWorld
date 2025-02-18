using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 绑定成员的值变化时触发的事件处理器
    /// </summary>
    /// <typeparam name="TSender">事件源类型</typeparam>
    /// <typeparam name="TArgs">事件参数类型</typeparam>
    /// <remarks>设计用于统一管理绑定关系，方便内存管理。</remarks>
    public abstract class BindingMemberChangedEvent<TSender, TArgs>
    {
        /// <summary>
        /// 事件处理器的调用序列
        /// </summary>
        protected readonly List<HandlerEntry> InvocationList = new List<HandlerEntry>(1);


        /// <summary>
        /// 初始化 <see cref="BindingMemberChangedEvent{TSender,TArgs}"/> 的新实例
        /// </summary>
        internal BindingMemberChangedEvent()
        {
        }


        /// <summary>
        /// 添加事件处理器
        /// </summary>
        /// <param name="handler">要添加的处理器</param>
        /// <param name="castedHandler">可被隐式转换为 <see cref="Action"/> 的方法组</param>
        public void Add(Delegate handler, Action<TSender, TArgs> castedHandler)
        {
            if (!(handler.Target is BindingBase binding))
            {
                throw new ArgumentException($"{handler.Target.GetType()} is not a valid subscriber type of {this}.", nameof(handler));
            }

            InvocationList.Add(new HandlerEntry(castedHandler, binding, handler.Method));
        }

        /// <summary>
        /// 引发事件，筛选并调用已有处理器
        /// </summary>
        /// <param name="sender">事件引发源</param>
        /// <param name="args">事件参数</param>
        public abstract void Invoke(TSender sender, TArgs args);

        /// <summary>
        /// 移除事件处理器
        /// </summary>
        /// <param name="handler">要移除的处理器</param>
        public void Remove(Delegate handler)
        {
            if (!(handler.Target is BindingBase binding))
            {
                return;
            }

            for (int i = InvocationList.Count - 1; i >= 0; i--)
            {
                if (InvocationList[i].Matches(handler, binding))
                {
                    InvocationList.RemoveAt(i);
                    break;
                }
            }
        }


        /// <summary>
        /// 保存事件处理器的信息条目
        /// </summary>
        protected readonly struct HandlerEntry
        {
            private readonly MethodInfo _method;

            /// <summary>
            /// 事件处理器
            /// </summary>
            public readonly Action<TSender, TArgs> Handler;

            /// <summary>
            /// 订阅事件的对象
            /// </summary>
            /// <remarks>对于静态类，该项为 <see langword="null"/>。</remarks>
            public readonly BindingBase Subscriber;


            /// <summary>
            /// 初始化 <see cref="HandlerEntry"/> 的新条目
            /// </summary>
            /// <param name="handler">事件处理器</param>
            /// <param name="subscriber">订阅事件的对象</param>
            /// <param name="method">处理器的原方法</param>
            internal HandlerEntry(Action<TSender, TArgs> handler, BindingBase subscriber, MethodInfo method)
            {
                Handler = handler;
                Subscriber = subscriber;

                _method = method;
            }


            /// <summary>
            /// 判断给出的信息是否与该条目匹配
            /// </summary>
            /// <param name="handler">事件处理器</param>
            /// <param name="subscriber">订阅事件的对象</param>
            /// <returns>若匹配，返回 <see langword="true"/>，反之则返回 <see langword="false"/>。</returns>
            public bool Matches(Delegate handler, BindingBase subscriber)
            {
                return ReferenceEquals(Subscriber, subscriber)
                    && ReferenceEquals(_method, handler.Method);
            }
        }
    }


    /// <summary>
    /// 绑定的依赖属性的值变化时触发的事件处理器
    /// </summary>
    public class BindingDependencyPropertyChangedEvent : BindingMemberChangedEvent<DependencyObject, DependencyPropertyChangedEventArgs>
    {
        /// <inheritdoc/>
        public override void Invoke(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            for (int i = 0; i < InvocationList.Count; i++)
            {
                var entry = InvocationList[i];
                var binding = entry.Subscriber;

                if ((ReferenceEquals(sender, binding.SourceMember.AssociatedObject)
                    && args.Property.Name == binding.SourceMember.MemberName)
                        || (ReferenceEquals(sender, binding.TargetMember.AssociatedObject)
                            && args.Property.Name == binding.TargetMember.MemberName))
                {
                    entry.Handler(sender, args);
                }
            }
        }
    }

    /// <summary>
    /// <see cref="INotifyPropertyChanged"/> 类型的绑定成员的值变化时触发的事件处理器
    /// <example>
    /// <para/>
    /// Example:
    /// <code>
    ///     private NotifiableBindingMemberChangedEvent _memberChanged = new NotifiableBindingMemberChangedEvent();
    ///     
    ///     public event PropertyChangedEventHandler PropertyChanged
    ///     {
    ///         add => _memberChanger.Add(value.Invoke);
    ///         remove => _memberChanger.Remove(value);
    ///     }
    ///     </code>
    /// </example>
    /// </summary>
    public class NotifiableBindingMemberChangedEvent : BindingMemberChangedEvent<object, PropertyChangedEventArgs>
    {
        /// <inheritdoc/>
        public override void Invoke(object sender, PropertyChangedEventArgs args)
        {
            for (int i = 0; i < InvocationList.Count; i++)
            {
                var entry = InvocationList[i];
                var binding = entry.Subscriber;

                if ((ReferenceEquals(sender, binding.SourceMember.AssociatedObject)
                    && args.PropertyName == binding.SourceMember.MemberName)
                        || (ReferenceEquals(sender, binding.TargetMember.AssociatedObject)
                            && args.PropertyName == binding.TargetMember.MemberName))
                {
                    entry.Handler(sender, args);
                }
            }
        }
    }
}
