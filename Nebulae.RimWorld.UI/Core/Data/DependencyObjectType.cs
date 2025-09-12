using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// <see cref="DependencyObject"/> 的 CLR 类型包装器
    /// </summary>
    public sealed class DependencyObjectType : IEquatable<DependencyObjectType>, IEquatable<Type>
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// <see cref="DependencyObject"/> 的唯一标识
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// <see cref="DependencyObject"/> 实际继承的 CLR 类型的包装器
        /// </summary>
        public readonly DependencyObjectType BaseType;

        /// <summary>
        /// <see cref="DependencyObject"/> 实际的 CLR 类型
        /// </summary>
        public readonly Type Type;

        #endregion


        private DependencyObjectType(Type type)
        {
            Id = Interlocked.Increment(ref _count);
            Type = type;

            if (type.BaseType != typeof(DependencyObject))
            {
                BaseType = FromSystemTypeUnsafe(type.BaseType);
            }
            else
            {
                BaseType = null;
            }
        }


        /// <summary>
        /// 根据 CLR 类型获取 <see cref="DependencyObjectType"/>
        /// </summary>
        /// <param name="systemType">CLR 类型</param>
        /// <returns>一个表示 <paramref name="systemType"/> 的 <see cref="DependencyObjectType"/>。</returns>
        public static DependencyObjectType FromSystemType(Type systemType)
        {
            if (systemType is null)
            {
                throw new ArgumentNullException(nameof(systemType));
            }

            if (!typeof(DependencyObject).IsAssignableFrom(systemType))
            {
                throw new ArgumentException($"'{systemType.AsLog()}' is not a valid value. '{typeof(DependencyObjectType).AsLog()}' only supports '{typeof(DependencyObject).AsLog()}' and its derived types.", nameof(systemType));
            }

            return CreatedTypes.GetOrAdd(systemType, x => new DependencyObjectType(x));
        }


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
            if (obj is DependencyObjectType other)
            {
                return Id == other.Id;
            }

            return obj is Type type && Type == type;
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(DependencyObjectType other)
        {
            return Id == other.Id;
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(Type other)
        {
            return Type == other;
        }

        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override sealed int GetHashCode() => Id;

        /// <summary>
        /// 判断 <see cref="Type"/> 是否为 <paramref name="dependencyObjectType"/> 的 CLR 类型的子类
        /// </summary>
        /// <param name="dependencyObjectType">要比较的 <see cref="DependencyObjectType"/></param>
        /// <returns>若 <see cref="Type"/> 是<paramref name="dependencyObjectType"/> 的 CLR 类型的子类，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool IsSubclassOf(DependencyObjectType dependencyObjectType)
        {
            if (dependencyObjectType is null)
            {
                return false;
            }

            var baseType = BaseType;

            while (baseType != null)
            {
                if (baseType.Id == dependencyObjectType.Id)
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        override public string ToString() => Type.AsLog();

        #endregion


        internal static DependencyObjectType FromSystemTypeUnsafe(Type systemType)
        {
            return CreatedTypes.GetOrAdd(systemType, x => new DependencyObjectType(x));
        }


        private static readonly ConcurrentDictionary<Type, DependencyObjectType> CreatedTypes = new ConcurrentDictionary<Type, DependencyObjectType>();

        private static int _count;
    }
}
