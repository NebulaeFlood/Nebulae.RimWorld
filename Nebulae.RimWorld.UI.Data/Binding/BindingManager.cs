using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.AccessControl;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 绑定关系管理器
    /// </summary>
    public static class BindingManager
    {
        private static readonly HashSet<IBinding> _globalBindings = new HashSet<IBinding>();


        /// <summary>
        /// 回收所有无效的绑定关系
        /// </summary>
        public static void CollectObsoleteBindings()
        {
            _globalBindings.RemoveWhere(binding => !binding.IsBindingValid);
            _globalBindings.TrimExcess();
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <typeparam name="T">被绑定成员的类型</typeparam>
        /// <param name="source">绑定源</param>
        /// <param name="target">绑定目标</param>
        /// <param name="sourcePath">绑定源路径</param>
        /// <param name="targetPath">绑定目标路径</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>创建的新的绑定关系。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="sourcePath"/> 或 <paramref name="targetPath"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="ArgumentException">当 <paramref name="sourcePath"/> 或 <paramref name="targetPath"/> ——对于依赖属性不为 <see cref="DependencyProperty"/>时；对于一般类型对象不为 <see cref="string"/> 时发生。</exception>
        /// <remarks>
        /// 对于静态对象，对应绑定源或绑定目标应该传入 <see langword="null"/>。<para/>
        /// 对于依赖属性，绑定对象应为 <see cref="DependencyObject"/>，对应绑定路径应传入绑定成员对应的 <see cref="DependencyProperty"/>。<para/>
        /// 对于附加属性，绑定对象应为 <see cref="DependencyObject"/>，对应绑定对象应传入要获取和设置属性值的对象而不是拥有附加属性的对象。<para/>
        /// 对于未实现 <see cref="INotifyPropertyChanged"/> 接口的对象，绑定关系将不会自动更新，可通过调用 <see cref="IBinding.Synchronize"/> 方法手动同步。<para/>
        /// 无法重复创建相同的绑定关系。<para/>
        /// </remarks>
        public static Binding<T> CreateBinding<T>(
            object source,
            object target,
            object sourcePath,
            object targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            VerifyBindingInfo(source, target, sourcePath, targetPath);

            return new Binding<T>(
                source,
                target,
                sourcePath,
                targetPath,
                mode,
                flags);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <typeparam name="TSource">绑定源成员值的类型</typeparam>
        /// <typeparam name="TTarget">绑定目标成员值的类型</typeparam>
        /// <param name="source">绑定源</param>
        /// <param name="target">绑定目标</param>
        /// <param name="sourcePath">绑定源路径</param>
        /// <param name="targetPath">绑定目标路径</param>
        /// <param name="valueConverter">綁定源和目标的成员值的转换器</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>创建的新的绑定关系。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="sourcePath"/> 或 <paramref name="targetPath"/> 为 <see langword="null"/> 时发生。</exception>
        /// <exception cref="ArgumentException">当 <paramref name="sourcePath"/> 或 <paramref name="targetPath"/> ——对于依赖属性不为 <see cref="DependencyProperty"/>时；对于一般类型对象不为 <see cref="string"/> 时发生。</exception>
        /// <remarks>
        /// 对于静态对象，对应绑定源或绑定目标应该传入 <see cref="Type"/>。<para/>
        /// 对于依赖属性，对应绑定路径应传入绑定成员对应的 <see cref="DependencyProperty"/>。<para/>
        /// 对于附加属性，对应绑定对象应传入要获取和设置属性值的对象而不是拥有附加属性的对象。<para/>
        /// 对于未实现 <see cref="INotifyPropertyChanged"/> 接口的对象，绑定关系将不会自动更新，可通过调用 <see cref="IBinding.Synchronize"/> 方法手动同步。<para/>
        /// 无法重复创建相同的绑定关系。<para/>
        /// </remarks>
        public static Binding<TSource, TTarget> CreateBinding<TSource, TTarget>(
            object source,
            object target,
            object sourcePath,
            object targetPath,
            IValueConverter<TSource, TTarget> valueConverter,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            VerifyBindingInfo(source, target, sourcePath, targetPath);

            if (valueConverter is null)
            {
                throw new ArgumentNullException(nameof(valueConverter), "A binding for different types must have a value converter.");
            }

            return new Binding<TSource,TTarget>(
                source,
                target,
                sourcePath,
                targetPath,
                valueConverter,
                mode,
                flags);
        }


        internal static bool IsBinding(IBinding binding)
        {
            return _globalBindings.Contains(binding);
        }

        private static void VerifyBindingInfo(
            object source,
            object target,
            object sourcePath,
            object targetPath)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (sourcePath is null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            if (targetPath is null)
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            bool sourceShouldBeDependencyObject = sourcePath is DependencyProperty;
            if (sourceShouldBeDependencyObject
                && !(source is DependencyObject))
            {
                throw new ArgumentException("The source must be a DependencyObject when the source path is a DependencyProperty.", nameof(source));
            }

            if (!sourceShouldBeDependencyObject
                && !(sourcePath is string))
            {
                throw new ArgumentException("The source path must be a string of a member's name. When the source is a DependencyObject a DependencyProperty is also availabel.", nameof(sourcePath));
            }

            bool targetShouldBeDependencyObject = targetPath is DependencyProperty;
            if (targetShouldBeDependencyObject
                && !(target is DependencyObject))
            {
                throw new ArgumentException("The target must be a DependencyObject when the target path is a DependencyProperty.", nameof(target));
            }

            if (!targetShouldBeDependencyObject
                && !(targetPath is string))
            {
                throw new ArgumentException("The target path must be a string of a member's name. When the target is a DependencyObject a DependencyProperty is also availabel.", nameof(targetPath));
            }
        }
    }
}
