using Nebulae.RimWorld.UI.Data.Binding.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 绑定关系管理器
    /// </summary>
    /// <remarks>注意手动回收所有绑定对象存在强引用的绑定关系，否则会造成内存泄漏。</remarks>
    public static class BindingManager
    {
        private static readonly WeakReference _obsoletedBinding = new WeakReference(new object());

        internal static readonly HashSet<BindingBase> GlobalBindings = new HashSet<BindingBase>();


        #region Bind Normal Members

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员路径</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static void Bind(
            object source,
            string sourcePath,
            object target,
            string targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Bind(source, sourcePath, target, targetPath, null, mode, flags);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员路径</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="converter">成员间的值转换器</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static void Bind(
            object source,
            string sourcePath,
            object target,
            string targetPath,
            IValueConverter converter,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (source is null)
            {
                Assert(BindingFailedReason.SourceArgumentNull);
            }

            if (target is null)
            {
                Assert(BindingFailedReason.TargetArgumentNull);
            }

            MemberInfo sourceMember = FindSourceMember(source, sourcePath, flags);
            MemberInfo targetMember = FindTargetMember(target, targetPath, flags);

            // 构造函数内判断绑定是否重复时已经将绑定添加到 GlobalBingings
            new Binding(source, sourceMember, target, targetMember, converter, mode);
        }

        #endregion

        #region Bind From Normal Member to Dependency Property

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员路径</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static void Bind(
            object source,
            string sourcePath,
            DependencyObject target,
            DependencyProperty targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Bind(source, sourcePath, target, targetPath, null, mode, flags);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员路径</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="converter">成员间的值转换器</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static void Bind(
            object source,
            string sourcePath,
            DependencyObject target,
            DependencyProperty targetPath,
            IValueConverter converter,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (source is null)
            {
                Assert(BindingFailedReason.SourceArgumentNull);
            }

            if (target is null)
            {
                Assert(BindingFailedReason.TargetArgumentNull);
            }

            MemberInfo sourceMember = FindSourceMember(source, sourcePath, flags);

            // 构造函数内判断绑定是否重复时已经将绑定添加到 GlobalBingings
            new Binding(source, sourceMember, target, targetPath, converter, mode);
        }

        #endregion

        #region Bind From Dependency Property to Normal Member

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员路径</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static void Bind(
            DependencyObject source,
            DependencyProperty sourcePath,
            object target,
            string targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Bind(source, sourcePath, target, targetPath, null, mode, flags);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员路径</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="converter">成员间的值转换器</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static void Bind(
            DependencyObject source,
            DependencyProperty sourcePath,
            object target,
            string targetPath,
            IValueConverter converter,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (source is null)
            {
                Assert(BindingFailedReason.SourceArgumentNull);
            }

            if (target is null)
            {
                Assert(BindingFailedReason.TargetArgumentNull);
            }

            MemberInfo targetMember = FindTargetMember(target, targetPath, flags);

            // 构造函数内判断绑定是否重复时已经将绑定添加到 GlobalBingings
            new Binding(source, sourcePath, target, targetMember, converter, mode);
        }

        #endregion

        #region Bind Dependency Properties

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员路径</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static void Bind(
            DependencyObject source,
            DependencyProperty sourcePath,
            DependencyObject target,
            DependencyProperty targetPath,
            BindingMode mode)
        {
            Bind(source, sourcePath, target, targetPath, null, mode);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员路径</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="converter">成员间的值转换器</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static void Bind(
            DependencyObject source,
            DependencyProperty sourcePath,
            DependencyObject target,
            DependencyProperty targetPath,
            IValueConverter converter,
            BindingMode mode)
        {
            if (source is null)
            {
                Assert(BindingFailedReason.SourceArgumentNull);
            }

            if (target is null)
            {
                Assert(BindingFailedReason.TargetArgumentNull);
            }

            // 构造函数内判断绑定是否重复时已经将绑定添加到 GlobalBingings
            new Binding(source, sourcePath, target, targetPath, converter, mode);
        }

        #endregion


        /// <summary>
        /// 回收不可用的绑定关系
        /// </summary>
        /// <remarks>并不会强制调用 <see cref="GC.Collect()"/>，只是使 <see cref="BindingBase"/> 对象可以被释放。</remarks>
        public static void CollectObsolutedBingings()
        {
            if (_obsoletedBinding.IsAlive)
            {
                return;
            }

            BindingBase[] bindings = GlobalBindings.ToArray();
            Array.ForEach(bindings, x =>
            {
                if (!x.IsBinding || !x.IsBindingValid)
                {
                    x.Unbind();
                }
            });

            _obsoletedBinding.Target = new object();
        }

        /// <summary>
        /// 取消所有与指定对象关联的绑定关系
        /// </summary>
        /// <param name="obj">要取消绑定关系的对象</param>
        public static void Unbind(object obj)
        {
            GlobalBindings.RemoveWhere(x =>
                ReferenceEquals(x.SourceMember.AssociatedObject, obj)
                || ReferenceEquals(x.TargetMember.AssociatedObject, obj));
        }


        //------------------------------------------------------
        //
        //  Privaet Methods
        //
        //------------------------------------------------------

        #region Privaet Methods

        private static void Assert(
                    BindingFailedReason reason,
                    Type associatedType = null,
                    object associatedPath = null)
        {
            switch (reason)
            {
                case BindingFailedReason.SourceArgumentNull:
                    throw new ArgumentNullException("source");
                case BindingFailedReason.SourceLostMember:
                    throw new MissingMemberException(associatedType.FullName, associatedPath.ToString());
                case BindingFailedReason.TargetArgumentNull:
                    throw new ArgumentNullException("target");
                case BindingFailedReason.TargetLostMember:
                    throw new MissingMemberException(associatedType.FullName, associatedPath.ToString());
                default:
                    break;
            }
        }

        private static MemberInfo FindMember(object source, string name, BindingFlags flags, out Type type)
        {
            type = (source as Type) ?? source.GetType();

            MemberInfo member = type.GetProperty(name, flags);
            return member ?? type.GetField(name, flags);
        }

        private static MemberInfo FindSourceMember(object source, string name, BindingFlags flags)
        {
            MemberInfo member = FindMember(source, name, flags, out Type type);

            if (member is null)
            {
                Assert(BindingFailedReason.SourceLostMember, type, name);
            }

            return member;
        }

        private static MemberInfo FindTargetMember(object target, string name, BindingFlags flags)
        {
            MemberInfo member = FindMember(target, name, flags, out Type type);

            if (member is null)
            {
                Assert(BindingFailedReason.TargetLostMember, type, name);
            }

            return member;
        }

        #endregion


        private enum BindingFailedReason
        {
            //------------------------------------------------------
            //
            //  From Source
            //
            //------------------------------------------------------

            SourceArgumentNull,
            SourceLostMember,


            //------------------------------------------------------
            //
            //  From Subscriber
            //
            //------------------------------------------------------

            TargetArgumentNull,
            TargetLostMember
        }
    }
}
