using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 表示依赖属性的有效值更改后调用的方法
    /// </summary>
    /// <param name="dp">属性值更改的依赖属性</param>
    /// <param name="args">有关属性更改的数据</param>
    internal delegate void DependencyPropertyChangedEventHandler(DependencyObject dp, DependencyPropertyChangedEventArgs args);


    /// <summary>
    /// 绑定的依赖属性的值变化时触发的事件处理器
    /// </summary>
    internal sealed class BindingDependencyPropertyChangedEvent
    {
        private readonly List<HandlerEntry> _invocationList = new List<HandlerEntry>(1);


        /// <summary>
        /// 添加事件处理器
        /// </summary>
        /// <param name="handler">要添加的处理器</param>
        public void Add(DependencyPropertyChangedEventHandler handler)
        {
            if (!(handler.Target is BindingBase binding))
            {
                throw new ArgumentException($"{handler.Target.GetType()} is not a valid subscriber type of {this}.", nameof(handler));
            }

            _invocationList.Add(new HandlerEntry(handler, binding));
        }

        /// <summary>
        /// 引发事件，筛选并调用已有处理器
        /// </summary>
        /// <param name="sender">事件引发源</param>
        /// <param name="args">事件参数</param>
        public void Invoke(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            for (int i = 0; i < _invocationList.Count; i++)
            {
                var entry = _invocationList[i];
                var binding = entry.Subscriber;

                if ((ReferenceEquals(sender, binding.SourceMember.BindingTarget)
                    && args.Property.Name == binding.SourceMember.MemberName)
                        || (ReferenceEquals(sender, binding.TargetMember.BindingTarget)
                            && args.Property.Name == binding.TargetMember.MemberName))
                {
                    entry.Handler(sender, args);
                }
            }
        }

        /// <summary>
        /// 移除事件处理器
        /// </summary>
        /// <param name="handler">要移除的处理器</param>
        public void Remove(DependencyPropertyChangedEventHandler handler)
        {
            for (int i = _invocationList.Count - 1; i >= 0; i--)
            {
                if (_invocationList[i].Matches(handler))
                {
                    _invocationList.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 保存事件处理器的信息条目
        /// </summary>
        private readonly struct HandlerEntry
        {
            private readonly MethodInfo _method;

            /// <summary>
            /// 事件处理器
            /// </summary>
            internal readonly DependencyPropertyChangedEventHandler Handler;

            /// <summary>
            /// 订阅事件的对象
            /// </summary>
            internal readonly BindingBase Subscriber;


            /// <summary>
            /// 初始化 <see cref="HandlerEntry"/> 的新实例
            /// </summary>
            /// <param name="handler">事件处理器</param>
            /// <param name="subscriber">订阅事件的对象</param>
            internal HandlerEntry(DependencyPropertyChangedEventHandler handler, BindingBase subscriber)
            {
                Handler = handler;
                Subscriber = subscriber;

                _method = handler.Method;
            }


            /// <summary>
            /// 判断给出的信息是否与该条目匹配
            /// </summary>
            /// <param name="handler">事件处理器</param>
            /// <returns>若匹配，返回 <see langword="true"/>，反之则返回 <see langword="false"/>。</returns>
            internal bool Matches(Delegate handler)
            {
                return ReferenceEquals(Subscriber, handler.Target)
                    && _method == handler.Method;
            }
        }
    }

    /// <summary>
    /// 绑定 <see cref="INotifyPropertyChanged"/> 类型的属性的值变化时触发的事件处理器
    /// <example>
    /// <para/>
    /// Example:
    /// <code>
    ///     private NotifiableBindingMemberChangedEvent _memberChanged = new NotifiableBindingMemberChangedEvent();
    ///     
    ///     public event DependencyPropertyChangedEventHandler PropertyChanged
    ///     {
    ///         add => _memberChanger.Add(value);
    ///         remove => _memberChanger.Remove(value);
    ///     }
    ///     </code>
    /// </example>
    /// </summary>
    public sealed class BindingNotifiablePropertyChangedEvent
    {
        private readonly List<HandlerEntry> _invocationList = new List<HandlerEntry>(1);

        /// <summary>
        /// 添加事件处理器
        /// </summary>
        /// <param name="handler">要添加的处理器</param>
        public void Add(PropertyChangedEventHandler handler)
        {
            if (!(handler.Target is BindingBase binding))
            {
                throw new ArgumentException($"{handler.Target.GetType()} is not a valid subscriber type of {this}.", nameof(handler));
            }

            _invocationList.Add(new HandlerEntry(handler, binding));
        }

        /// <summary>
        /// 引发事件，筛选并调用已有处理器
        /// </summary>
        /// <param name="sender">事件引发源</param>
        /// <param name="args">事件参数</param>
        public void Invoke(object sender, PropertyChangedEventArgs args)
        {
            for (int i = 0; i < _invocationList.Count; i++)
            {
                var entry = _invocationList[i];
                var binding = entry.Subscriber;

                if ((ReferenceEquals(sender, binding.SourceMember.BindingTarget)
                    && args.PropertyName == binding.SourceMember.MemberName)
                        || (ReferenceEquals(sender, binding.TargetMember.BindingTarget)
                            && args.PropertyName == binding.TargetMember.MemberName))
                {
                    entry.Handler(sender, args);
                }
            }
        }

        /// <summary>
        /// 移除事件处理器
        /// </summary>
        /// <param name="handler">要移除的处理器</param>
        public void Remove(PropertyChangedEventHandler handler)
        {
            for (int i = _invocationList.Count - 1; i >= 0; i--)
            {
                if (_invocationList[i].Matches(handler))
                {
                    _invocationList.RemoveAt(i);
                    break;
                }
            }
        }

        private readonly struct HandlerEntry
        {
            private readonly MethodInfo _method;

            /// <summary>
            /// 事件处理器
            /// </summary>
            internal readonly PropertyChangedEventHandler Handler;

            /// <summary>
            /// 订阅事件的对象
            /// </summary>
            internal readonly BindingBase Subscriber;


            /// <summary>
            /// 初始化 <see cref="HandlerEntry"/> 的新实例
            /// </summary>
            /// <param name="handler">事件处理器</param>
            /// <param name="subscriber">订阅事件的对象</param>
            internal HandlerEntry(PropertyChangedEventHandler handler, BindingBase subscriber)
            {
                Handler = handler;
                Subscriber = subscriber;

                _method = handler.Method;
            }


            /// <summary>
            /// 判断给出的信息是否与该条目匹配
            /// </summary>
            /// <param name="handler">事件处理器</param>
            /// <returns>若匹配，返回 <see langword="true"/>，反之则返回 <see langword="false"/>。</returns>
            internal bool Matches(Delegate handler)
            {
                return ReferenceEquals(Subscriber, handler.Target)
                    && _method == handler.Method;
            }
        }
    }
}
