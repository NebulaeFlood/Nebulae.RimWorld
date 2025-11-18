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
        public static readonly DefInfo<T> Empty = new DefInfo<T>(string.Empty);


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// <see cref="Verse.Def"/> 的实例
        /// </summary>
        public readonly T Def;

        /// <summary>
        /// 包含 <see cref="Def"/> 的 Mod 是否已加载
        /// </summary>
        public readonly bool Loaded;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

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

            _defName = def.defName;

            Def = def;
            Loaded = true;
        }

        private DefInfo(T def, string defName)
        {
            _defName = defName;

            Def = def;
            Loaded = true;
        }

        private DefInfo(string defName)
        {
            _defName = defName;

            Def = null;
            Loaded = false;
        }

        #endregion


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
            if (info.Loaded)
            {
                return info;
            }

            if (string.IsNullOrEmpty(info._defName))
            {
                return Empty;
            }

            var def = DefDatabase<T>.GetNamedSilentFail(info._defName);

            return def is null ? info : new DefInfo<T>(def, def.defName);
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
            Scribe_Values.Look(ref _defName, "DefName", defaultValue: string.Empty);
        }

        /// <summary>
        /// 判断当前 <see cref="DefInfo{T}"/> 是否与指定的对象等效
        /// </summary>
        /// <param name="obj">要判断的对象</param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override readonly bool Equals(object obj)
        {
            return obj is DefInfo<T> other && _defName.Equals(other._defName);
        }

        /// <summary>
        /// 判断当前 <see cref="DefInfo{T}"/> 是否与指定的对象等效
        /// </summary>
        /// <param name="other">要判断的对象</param>
        /// <returns>若二者等效，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public readonly bool Equals(DefInfo<T> other)
        {
            return _defName.Equals(other._defName);
        }

        /// <summary>
        /// 获取当前 <see cref="DefInfo{T}"/> 的哈希代码
        /// </summary>
        /// <returns>当前 <see cref="DefInfo{T}"/> 的哈希代码。</returns>
        public override readonly int GetHashCode()
        {
            return _defName.GetHashCode();
        }

        /// <summary>
        /// 解析该 <see cref="DefInfo{T}"/>，并获取其对应的 <see cref="Def"/> 实例
        /// </summary>
        /// <returns>解析后的 <see cref="DefInfo{T}"/> 实例。</returns>
        public readonly DefInfo<T> Resolve()
        {
            if (Loaded)
            {
                return this;
            }

            if (string.IsNullOrEmpty(_defName))
            {
                return Empty;
            }

            var def = DefDatabase<T>.GetNamedSilentFail(_defName);

            return def is null ? this : new DefInfo<T>(def, def.defName);
        }

        /// <summary>
        /// 获取当前 <see cref="DefInfo{T}"/> 的字符串表示形式
        /// </summary>
        /// <returns>当前 <see cref="DefInfo{T}"/> 的字符串表示形式。</returns>
        public override readonly string ToString() => _defName ?? "Empty";

        #endregion


        private string _defName;
    }
}
