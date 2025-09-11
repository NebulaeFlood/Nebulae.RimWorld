using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Nebulae.RimWorld.UI.Core
{
    /// <summary>
    /// 表示一个独一无二的对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <remarks>
    /// 用于保证成员在 <see cref="OwnerType"/> 及其子类型中唯一。<para/>
    /// 子类型需要在创建实例前使用 <see cref="Exist"/> 方法判断是否已经存在相同对象。
    /// </remarks>
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
        /// 获取创建的 <typeparamref name="T"/> 数量
        /// </summary>
        public static int Count => _globalIndex;


        /// <summary>
        /// 为 <see cref="Singleton{T}"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="ownerType">拥有对象的类型</param>
        protected Singleton(string name, Type ownerType)
        {
            Name = name;
            OwnerType = ownerType;

            var key = new SingletonKey(name, ownerType);
            _hashCode = key.GetHashCode();

            Interlocked.Increment(ref _globalIndex);
            Singletons[key] = (T)this;
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
        public override sealed bool Equals(object obj)
        {
            return obj is Singleton<T> other && _hashCode == other._hashCode;
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要与当前对象进行比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(Singleton<T> other)
        {
            return _hashCode == other._hashCode;
        }

        /// <summary>
        /// 默认的哈希函数
        /// </summary>
        /// <returns>该实例的哈希代码。</returns>
        public override sealed int GetHashCode() => _hashCode;

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override sealed string ToString() => $"{OwnerType}.{Name}";

        #endregion


        //------------------------------------------------------
        //
        //  Private Feilds
        //
        //------------------------------------------------------

        #region Private Feilds

        private static readonly ConcurrentDictionary<SingletonKey, T> Singletons = new ConcurrentDictionary<SingletonKey, T>();
        private static int _globalIndex = 0;

        private readonly int _hashCode;

        #endregion


        /// <summary>
        /// <see cref="Singleton{T}"/> 的标识
        /// </summary>
        private readonly struct SingletonKey : IEquatable<SingletonKey>
        {
            //------------------------------------------------------
            //
            //  Public Fields
            //
            //------------------------------------------------------

            #region Public Fields

            /// <summary>
            /// 哈希值
            /// </summary>
            public readonly int HashCode;

            /// <summary>
            /// 对象名称
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// 拥有对象的类型
            /// </summary>
            public readonly Type OwnerType;

            #endregion


            /// <summary>
            /// 初始化 <see cref="SingletonKey"/> 的新实例
            /// </summary>
            /// <param name="name">对象名</param>
            /// <param name="ownerType">拥有对象的类型</param>
            public SingletonKey(string name, Type ownerType)
            {
                HashCode = name.GetHashCode() ^ ownerType.GetHashCode();
                Name = name;
                OwnerType = ownerType;
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
            /// 获取实例的哈希代码
            /// </summary>
            /// <returns>该实例的哈希代码。</returns>
            public override int GetHashCode() => HashCode;

            /// <summary>
            /// 获取表示当前对象的字符串
            /// </summary>
            /// <returns>表示当前对象的字符串。</returns>
            public override string ToString() => $"{OwnerType}.{Name}";

            #endregion
        }
    }
}
