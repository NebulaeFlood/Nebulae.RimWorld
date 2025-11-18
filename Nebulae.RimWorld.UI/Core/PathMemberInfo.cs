using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core
{
    /// <summary>
    /// 路径成员，用于表示 <see cref="MemberPath"/> 中的节点
    /// </summary>
    public sealed class PathMemberInfo : IEquatable<PathMemberInfo>
    {
        //------------------------------------------------------
        //
        //  Public Static Fields
        //
        //------------------------------------------------------

        #region Public Static Fields

        /// <summary>
        /// <see cref="DependencyObject.GetValue(DependencyProperty)"/> 方法的 <see cref="MethodInfo"/>
        /// </summary>
        public static readonly MethodInfo DependencyObjectGetValueMethod = typeof(DependencyObject).GetMethod(nameof(DependencyObject.GetValue), BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(DependencyProperty) }, null);

        /// <summary>
        /// <see cref="DependencyObject.SetValue(DependencyProperty, object)"/> 方法的 <see cref="MethodInfo"/>
        /// </summary>
        public static readonly MethodInfo DependencyObjectSetValueMethod = typeof(DependencyObject).GetMethod(nameof(DependencyObject.SetValue), BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(DependencyProperty), typeof(object) }, null);

        #endregion


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 声明该成员的类型
        /// </summary>
        public readonly Type DeclaringType;

        /// <summary>
        /// 该成员是否为静态成员
        /// </summary>
        public readonly bool IsStatic;

        /// <summary>
        /// 该成员是否可写
        /// </summary>
        public readonly bool IsWritable;

        /// <summary>
        /// 该成员的元数据信息
        /// </summary>
        /// <remarks>此项可能为 <see cref="DependencyProperty"/>、<see cref="FieldInfo"/> 或 <see cref="PropertyInfo"/>。</remarks>
        public readonly object Metadata;

        /// <summary>
        /// 该成员的名称
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 该成员的种类
        /// </summary>
        public readonly PathMemberType Type;

        /// <summary>
        /// 该成员的类型
        /// </summary>
        public readonly Type ValueType;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        internal PathMemberInfo(DependencyProperty property)
        {
            DeclaringType = property.OwnerType;

            IsStatic = false;
            IsWritable = true;

            Metadata = property;
            Name = property.Name;
            Type = PathMemberType.DependencyProperty;
            ValueType = property.ValueType;
        }

        internal PathMemberInfo(FieldInfo field)
        {
            DeclaringType = field.DeclaringType;

            IsStatic = field.IsStatic;
            IsWritable = !field.IsInitOnly;

            Metadata = field;
            Name = field.Name;
            Type = PathMemberType.Field;
            ValueType = field.FieldType;
        }

        internal PathMemberInfo(PropertyInfo member, PathMemberType type)
        {
            DeclaringType = member.DeclaringType;

            IsStatic = member.GetGetMethod(true).IsStatic;
            IsWritable = member.GetSetMethod(true) != null;

            Metadata = member;
            Name = member.Name;
            Type = type;
            ValueType = member.PropertyType;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            return obj is PathMemberInfo other ? Metadata.Equals(other.Metadata) : Metadata.Equals(obj);
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(PathMemberInfo other)
        {
            return Metadata.Equals(other.Metadata);
        }

        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override int GetHashCode()
        {
            return Metadata.GetHashCode();
        }

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return Type switch
            {
                PathMemberType.DependencyProperty => $"({DeclaringType.AsLog()}.{Name})",
                PathMemberType.Indexer => "[]",
                _ => Name,
            };
        }

        #endregion
    }
}
