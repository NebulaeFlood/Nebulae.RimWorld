using System;
using Verse;

namespace Nebulae.RimWorld
{
    /// <summary>
    /// 存储特定 <see cref="Def"/> 类型的信息
    /// </summary>
    /// <typeparam name="T"><see cref="Verse.Def"/> 的类型</typeparam>
    public struct DefInfo<T> : IEquatable<DefInfo<T>>, IExposable where T : Def
    {
        /// <summary>
        /// 一个空的 <see cref="DefInfo{T}"/> 实例
        /// </summary>
        public static readonly DefInfo<T> Empty = new DefInfo<T>();


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// <see cref="Verse.Def"/> 的实例
        /// </summary>
        public T Def;

        /// <summary>
        /// <see cref="Def"/> 的 <see cref="Def.defName"/>
        /// </summary>
        public string DefName;

        /// <summary>
        /// 包含 <see cref="Def"/> 的 Mod 是否已加载
        /// </summary>
        public bool Loaded;

        #endregion


        /// <summary>
        /// 初始化 <see cref="DefInfo{T}"/> 的新实例
        /// </summary>
        /// <param name="def">要保存信息的 <see cref="Verse.Def"/> 实例</param>
        public DefInfo(T def)
        {
            if (def is null)
            {
                throw new ArgumentNullException(nameof(def));
            }

            Def = def;
            DefName = def.defName;
            Loaded = true;
        }


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

        /// <summary>
        /// 获取指定的 <see cref="DefInfo{T}"/> 的 <see cref="Def"/> 实例
        /// </summary>
        /// <param name="info">要判获取 <see cref="Def"/> 的实例</param>
        /// <returns>指定的 <see cref="DefInfo{T}"/> 的 <see cref="Def"/> 实例。</returns>
        public static T GetDef(DefInfo<T> info) => info.Def;

        /// <summary>
        /// 判断指定的 <see cref="DefInfo{T}"/> 是否已加载
        /// </summary>
        /// <param name="info">要判断的实例</param>
        /// <returns>若指定的 <see cref="DefInfo{T}"/> 已加载，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public static bool IsLoaded(DefInfo<T> info) => info.Loaded;

        /// <summary>
        /// 解析指定的 <see cref="DefInfo{T}"/>
        /// </summary>
        /// <param name="info">要解析的实例</param>
        /// <returns>解析后的 <see cref="DefInfo{T}"/> 实例。</returns>
        public static DefInfo<T> Resolve(DefInfo<T> info)
        {
            if (string.IsNullOrEmpty(info.DefName))
            {
                return Empty;
            }

            info.Def = DefDatabase<T>.GetNamedSilentFail(info.DefName);
            info.Loaded = info.Def != null;

            return info;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将 <see cref="DefInfo{T}"/> 的数据暴露给 <see cref="Scribe_Deep"/>
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look(ref DefName, nameof(DefName), defaultValue: null);
        }

        /// <summary>
        /// 判断当前 <see cref="DefInfo{T}"/> 是否与指定的对象等效
        /// </summary>
        /// <param name="obj">要判断的对象</param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            return obj is DefInfo<T> other && DefName == other.DefName;
        }

        /// <summary>
        /// 判断当前 <see cref="DefInfo{T}"/> 是否与指定的对象等效
        /// </summary>
        /// <param name="other">要判断的对象</param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(DefInfo<T> other)
        {
            return DefName == other.DefName; ;
        }

        /// <summary>
        /// 获取当前 <see cref="DefInfo{T}"/> 的哈希代码
        /// </summary>
        /// <returns>当前 <see cref="DefInfo{T}"/> 的哈希代码。</returns>
        public override int GetHashCode()
        {
            return DefName.GetHashCode();
        }

        /// <summary>
        /// 解析该 <see cref="DefInfo{T}"/>，并获取其对应的 <see cref="Def"/> 实例
        /// </summary>
        /// <returns>解析后的 <see cref="DefInfo{T}"/> 实例。</returns>
        public DefInfo<T> Resolve()
        {
            if (string.IsNullOrEmpty(DefName))
            {
                return Empty;
            }

            Def = DefDatabase<T>.GetNamedSilentFail(DefName);
            Loaded = Def != null;

            return this;
        }

        /// <summary>
        /// 获取当前 <see cref="DefInfo{T}"/> 的字符串表示形式
        /// </summary>
        /// <returns>当前 <see cref="DefInfo{T}"/> 的字符串表示形式。</returns>
        public override string ToString() => DefName ?? "Empty";

        #endregion
    }
}
