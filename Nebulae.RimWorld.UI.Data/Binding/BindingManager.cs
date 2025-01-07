using Nebulae.RimWorld.UI.Data.Binding.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.AccessControl;
using UnityEngine.Networking.Types;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 绑定关系管理器
    /// </summary>
    public static class BindingManager
    {
        private static readonly HashSet<BindingBase> _globalBindings = new HashSet<BindingBase>();


        /// <summary>
        /// 回收所有无效的绑定关系
        /// </summary>
        public static void CollectObsoleteBindings()
        {
            _globalBindings.RemoveWhere(binding => !binding.IsBindingValid);
            _globalBindings.TrimExcess();
        }

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
        /// <returns>创建的绑定关系</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Bind(
            object source,
            string sourcePath,
            object target,
            string targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
        {
            return Bind(source, sourcePath, target, targetPath, null, mode, flags);
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
        /// <returns>创建的绑定关系</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Bind(
            object source,
            string sourcePath,
            object target,
            string targetPath,
            IValueConverter converter,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
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

            return new Binding(source, sourceMember, target, targetMember, converter, mode);
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
        /// <returns>创建的绑定关系</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Bind(
            object source,
            string sourcePath,
            DependencyObject target,
            DependencyProperty targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
        {
            return Bind(source, sourcePath, target, targetPath, null, mode, flags);
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
        /// <returns>创建的绑定关系</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Bind(
            object source,
            string sourcePath,
            DependencyObject target,
            DependencyProperty targetPath,
            IValueConverter converter,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
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

            return new Binding(source, sourceMember, target, targetPath, converter, mode);
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
        /// <returns>创建的绑定关系</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Bind(
            DependencyObject source,
            DependencyProperty sourcePath,
            object target,
            string targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
        {
            return Bind(source, sourcePath, target, targetPath, null, mode, flags);
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
        /// <returns>创建的绑定关系</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Bind(
            DependencyObject source,
            DependencyProperty sourcePath,
            object target,
            string targetPath,
            IValueConverter converter,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
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

            return new Binding(source, sourcePath, target, targetMember, converter, mode);
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
        /// <returns>创建的绑定关系</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Bind(
            DependencyObject source,
            DependencyProperty sourcePath,
            DependencyObject target,
            DependencyProperty targetPath,
            BindingMode mode)
        {
            return Bind(source, sourcePath, target, targetPath, null, mode);
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
        /// <returns>创建的绑定关系</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 
        /// <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Bind(
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

            return new Binding(source, sourcePath, target, targetPath, converter, mode);
        }

        #endregion


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
            //  From Target
            //
            //------------------------------------------------------

            TargetArgumentNull,
            TargetLostMember
        }
    }
}
