using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core
{
    /// <summary>
    /// 表示一个独一无二的对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    [DebuggerStepThrough]
    public abstract class Singleton<T> : IEquatable<Singleton<T>> where T : Singleton<T>
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// <see cref="Singleton{T}"/> 的名称
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 拥有该 <see cref="Singleton{T}"/> 的类型
        /// </summary>
        public readonly Type OwnerType;

        #endregion


        /// <summary>
        /// 为 <see cref="Singleton{T}"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="ownerType">拥有对象的类型</param>
        protected Singleton(string name, Type ownerType)
        {
            _id = _globalIndex;

            Name = name;
            OwnerType = ownerType;

            Interlocked.Increment(ref _globalIndex);

            Singletons[new SingletonKey(name, ownerType)] = (T)this;
        }


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

        /// <summary>
        /// 判断目标对象是否已经存在
        /// </summary>
        /// <param name="name">对象名</param>
        /// <param name="ownerType">拥有对象的类型</param>
        /// <returns>若存在符合要求的对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool Exist(string name, Type ownerType) => Singletons.ContainsKey(new SingletonKey(name, ownerType));

        /// <summary>
        /// 尝试获取目标对象
        /// </summary>
        /// <param name="name">对象名</param>
        /// <param name="ownerType">拥有对象的类型</param>
        /// <param name="singleton">符合要求的对象</param>
        /// <returns>若存在符合要求的对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool TryGetSingleton(string name, Type ownerType, out T singleton) => Singletons.TryGetValue(new SingletonKey(name, ownerType), out singleton);

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
        /// <param name="obj">要与当前对象进行比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            return obj is Singleton<T> other && _id == other._id;
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要与当前对象进行比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(Singleton<T> other)
        {
            return _id == other._id;
        }

        /// <summary>
        /// 默认的哈希函数
        /// </summary>
        /// <returns>该实例的哈希代码。</returns>
        public override int GetHashCode() => _id;

        #endregion


        //------------------------------------------------------
        //
        //  Private Feilds
        //
        //------------------------------------------------------

        #region Private Feilds

        private static readonly ConcurrentDictionary<SingletonKey, T> Singletons = new ConcurrentDictionary<SingletonKey, T>();
        private static int _globalIndex = 0;

        private readonly int _id;

        #endregion


        /// <summary>
        /// <see cref="Singleton{T}"/> 的标识
        /// </summary>
        private readonly struct SingletonKey : IEquatable<SingletonKey>
        {
            /// <summary>
            /// 对象名
            /// </summary>
            public readonly string Name;
            /// <summary>
            /// 拥有对象的类型
            /// </summary>
            public readonly Type OwnerType;


            /// <summary>
            /// 初始化 <see cref="SingletonKey"/> 的新实例
            /// </summary>
            /// <param name="name">对象名</param>
            /// <param name="ownerType">拥有对象的类型</param>
            public SingletonKey(string name, Type ownerType)
            {
                _hashCode = name.GetHashCode() ^ ownerType.GetHashCode();

                Name = name;
                OwnerType = ownerType;
            }


            /// <summary>
            /// 判断指定对象是否等于当前对象
            /// </summary>
            /// <param name="obj">要与当前对象进行比较的对象</param>
            /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
            public override bool Equals(object obj)
            {
                return obj is SingletonKey other
                    && Name == other.Name
                    && (OwnerType == other.OwnerType
                        || OwnerType.IsSubclassOf(other.OwnerType)
                        || other.OwnerType.IsSubclassOf(OwnerType));
            }

            /// <summary>
            /// 判断指定对象是否等于当前对象
            /// </summary>
            /// <param name="other">要与当前对象进行比较的对象</param>
            /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
            public bool Equals(SingletonKey other)
            {
                return Name == other.Name
                    && (OwnerType == other.OwnerType
                        || OwnerType.IsSubclassOf(other.OwnerType)
                        || other.OwnerType.IsSubclassOf(OwnerType));
            }

            /// <summary>
            /// 默认的哈希函数
            /// </summary>
            /// <returns>该实例的哈希代码。</returns>
            public override int GetHashCode() => _hashCode;


            private readonly int _hashCode;
        }
    }
}
