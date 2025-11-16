using Nebulae.RimWorld.Utilities;
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
    /// <remarks>派生自 <see cref="Singleton{T}"/> 的类型需要在创建实例前使用 <see cref="Exist"/> 方法判断是否已经存在相同对象。</remarks>
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
        /// <see cref="Singleton{T}"/> 的唯一标识
        /// </summary>
        public readonly int Id;

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
        /// 获取创建的 <typeparamref name="T"/> 的数量
        /// </summary>
        public static int Count => _count;


        /// <summary>
        /// 为 <see cref="Singleton{T}"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="ownerType">拥有对象的类型</param>
        protected Singleton(string name, Type ownerType)
        {
            Name = name;
            OwnerType = ownerType;

            Id = Interlocked.Increment(ref _count);
            Singletons[new SingletonKey(name, ownerType)] = (T)this;
        }


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

        /// <summary>
        /// 判断符合要求的对象是否已经存在
        /// </summary>
        /// <param name="name">对象名</param>
        /// <param name="ownerType">拥有对象的类型</param>
        /// <returns>若存在符合要求的对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool Exist(string name, Type ownerType) => Singletons.ContainsKey(new SingletonKey(name, ownerType));

        /// <summary>
        /// 尝试获取符合要求的对象
        /// </summary>
        /// <param name="name">对象名</param>
        /// <param name="ownerType">拥有对象的类型</param>
        /// <param name="singleton">符合要求的对象</param>
        /// <returns>若存在符合要求的对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool TryGet(string name, Type ownerType, out T singleton) => Singletons.TryGetValue(new SingletonKey(name, ownerType), out singleton);

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
        public override sealed bool Equals(object obj)
        {
            return obj is Singleton<T> other && Id == other.Id;
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(Singleton<T> other)
        {
            return Id == other.Id;
        }

        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override sealed int GetHashCode() => Id;

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override sealed string ToString() => $"{OwnerType.AsLog()}.{Name}";

        #endregion


        //------------------------------------------------------
        //
        //  Private Feilds
        //
        //------------------------------------------------------

        #region Private Feilds

        private static readonly ConcurrentDictionary<SingletonKey, T> Singletons = new ConcurrentDictionary<SingletonKey, T>();
        private static int _count;

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
                    && OwnerType == other.OwnerType;
            }

            /// <summary>
            /// 判断指定对象是否等于当前对象
            /// </summary>
            /// <param name="other">要与当前对象进行比较的对象</param>
            /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
            public bool Equals(SingletonKey other)
            {
                return Name == other.Name
                    && OwnerType == other.OwnerType;
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
