using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core
{
    /// <summary>
    /// 组成 <see cref="MemberPath"/> 的成员
    /// </summary>
    public sealed class PathMember : IEquatable<PathMember>
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
        /// 该成员是否可写
        /// </summary>
        public readonly bool IsReadOnly;

        /// <summary>
        /// 该成员是否为静态成员
        /// </summary>
        public readonly bool IsStatic;

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

        internal PathMember(DependencyProperty property)
        {
            DeclaringType = property.OwnerType;

            IsStatic = false;
            IsReadOnly = true;

            Metadata = property;
            Name = property.Name;
            Type = PathMemberType.DependencyProperty;
            ValueType = property.ValueType;
        }

        internal PathMember(PropertyInfo indexer, ParameterInfo[] parameters)
        {
            DeclaringType = indexer.DeclaringType;

            IsStatic = indexer.GetGetMethod(true).IsStatic;
            IsReadOnly = indexer.GetSetMethod(true) is null;

            Metadata = indexer;
            Name = FormatIndexerSignatureUnsafe(parameters);
            Type = PathMemberType.Indexer;
            ValueType = indexer.PropertyType;
        }

        internal PathMember(FieldInfo field)
        {
            DeclaringType = field.DeclaringType;

            IsStatic = field.IsStatic;
            IsReadOnly = field.IsInitOnly;

            Metadata = field;
            Name = field.Name;
            Type = PathMemberType.Field;
            ValueType = field.FieldType;
        }

        internal PathMember(PropertyInfo member)
        {
            DeclaringType = member.DeclaringType;

            IsStatic = member.GetGetMethod(true).IsStatic;
            IsReadOnly = member.GetSetMethod(true) is null;

            Metadata = member;
            Name = member.Name;
            Type = PathMemberType.Property;
            ValueType = member.PropertyType;
        }

        internal PathMember(Type targetType)
        {
            DeclaringType = targetType.DeclaringType;

            IsStatic = false;
            IsReadOnly = true;

            Metadata = targetType;
            Name = targetType.AsLog();
            Type = PathMemberType.Target;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

        /// <summary>
        /// 获取格式化索引器签名
        /// </summary>
        /// <param name="parameters">索引器的参数</param>
        /// <returns>由索引器参数组成的格式化字符串。</returns>
        /// <remarks><see cref="Name"/> 使用该方法生成的格式化字符串。</remarks>
        public static string FormatIndexerSignature(params ParameterInfo[] parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var length = parameters.Length;

            if (length < 1)
            {
                throw new ArgumentException("Indexer must have at least one parameter.", nameof(parameters));
            }

            if (length is 1)
            {
                var parameter = parameters[0];

                if (parameter is null)
                {
                    throw new ArgumentException("Indexer parameters cannot contains null.", nameof(parameters));
                }

                return string.IsNullOrEmpty(parameter.Name)
                    ? $"[({parameter.ParameterType.AsLog()})]"
                    : $"[({parameter.ParameterType.AsLog()}) {parameter.Name}]";
            }
            else
            {
                var parameter = parameters[0];

                if (parameter is null)
                {
                    throw new ArgumentException("Indexer parameters cannot contains null.", nameof(parameters));
                }

                var builder = new StringBuilder(length * 16).Append('[');
                builder.Append('(').Append(parameter.ParameterType.AsLog()).Append(')');

                if (!string.IsNullOrEmpty(parameter.Name))
                {
                    builder.Append(' ').Append(parameter.Name);
                }

                for (int i = 1; i < length; i++)
                {
                    parameter = parameters[i];

                    if (parameter is null)
                    {
                        throw new ArgumentException("Indexer parameters cannot contains null.", nameof(parameters));
                    }

                    builder.Append(", ").Append('(').Append(parameter.ParameterType.AsLog()).Append(')');

                    if (!string.IsNullOrEmpty(parameter.Name))
                    {
                        builder.Append(' ').Append(parameter.Name);
                    }
                }

                return builder.Append(']').ToString();
            }
        }

        /// <summary>
        /// 获取格式化索引器签名
        /// </summary>
        /// <param name="parameters">索引器的参数</param>
        /// <returns>由索引器参数组成的格式化字符串。</returns>
        /// <remarks>该方法不检查参数是否有效。</remarks>
        public static string FormatIndexerSignatureUnsafe(params ParameterInfo[] parameters)
        {
            var length = parameters.Length;

            if (length is 1)
            {
                var parameter = parameters[0];

                return string.IsNullOrEmpty(parameter.Name)
                    ? $"[({parameter.ParameterType.AsLog()})]"
                    : $"[({parameter.ParameterType.AsLog()}) {parameter.Name}]";
            }
            else
            {
                var builder = new StringBuilder(length * 16).Append('[');
                var parameter = parameters[0];

                builder.Append('(').Append(parameter.ParameterType.AsLog()).Append(')');

                if (!string.IsNullOrEmpty(parameter.Name))
                {
                    builder.Append(' ').Append(parameter.Name);
                }

                for (int i = 1; i < length; i++)
                {
                    parameter = parameters[i];

                    builder.Append(", ").Append('(').Append(parameter.ParameterType.AsLog()).Append(')');

                    if (!string.IsNullOrEmpty(parameter.Name))
                    {
                        builder.Append(' ').Append(parameter.Name);
                    }
                }

                return builder.Append(']').ToString();
            }
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
            if (obj is null)
            {
                return false;
            }

            // 由于 PathMember 只由 MamberPath 创建，
            // 而相同路径使用同一个 MemberPath;
            // 又因为不同路径会导致 accesor 等委托实际不同，
            // 因此只比较两个 PathMember 是否指向同一个对象。
            return obj is PathMember other && this == other;
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(PathMember other)
        {
            // 由于 PathMember 只由 MamberPath 创建，
            // 而相同路径使用同一个 MemberPath;
            // 又因为不同路径会导致 accesor 等委托实际不同，
            // 因此只比较两个 PathMember 是否指向同一个对象。
            return this == other;
        }

        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(DeclaringType, Name, accessor);
        }

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return Type switch
            {
                PathMemberType.DependencyProperty => $"({Metadata})",
                PathMemberType.Target => "{Self}",
                _ => Name,
            };
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        internal MemberAccessor accessor;
        internal MemberModifier modifier;

        #endregion


        //------------------------------------------------------
        //
        //  Internal Delegates
        //
        //------------------------------------------------------

        #region Internal Delegates

        internal delegate object MemberAccessor(MemberPath path, object target);

        internal delegate void MemberModifier(MemberPath path, object target, object value);

        #endregion
    }
}
