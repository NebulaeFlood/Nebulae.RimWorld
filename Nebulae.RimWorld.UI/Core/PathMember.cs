using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core
{
    /// <summary>
    /// 组成 <see cref="MemberPath"/> 的成员
    /// </summary>
    public sealed class PathMember
    {
        /// <summary>
        /// 成员信息
        /// </summary>
        public readonly PathMemberInfo Info;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取目标成员的访问器
        /// </summary>
        public MemberAccessor<object> Accessor => accessor;

        /// <summary>
        /// 获取目标成员的修改器
        /// </summary>
        /// <remarks>当 <see cref="Info"/> 的 <see cref="PathMemberInfo.IsWritable"/> 为 <see langword="false"/> 时，此属性为 <see langword="null"/>。</remarks>
        public MemberModifier<object> Modifier => modifier;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        internal PathMember(PathMemberInfo info)
        {
            Info = info;
        }

        internal PathMember(PathMemberInfo info, MemberAccessor<object> accessor, MemberModifier<object> modifier)
        {
            Info = info;

            this.accessor = accessor;
            this.modifier = modifier;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return Info.ToString();
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        internal MemberAccessor<object> accessor;
        internal MemberModifier<object> modifier;

        #endregion


        //------------------------------------------------------
        //
        //  Public Delegates
        //
        //------------------------------------------------------

        #region Public Delegates

        /// <summary>
        /// 成员访问器
        /// </summary>
        /// <typeparam name="T">成员类型</typeparam>
        /// <param name="target">拥有成员的对象</param>
        /// <returns>目标成员的值。</returns>
        public delegate T MemberAccessor<out T>(object target);

        /// <summary>
        /// 成员修改器
        /// </summary>
        /// <typeparam name="T">成员类型</typeparam>
        /// <param name="target">拥有成员的对象</param>
        /// <param name="value">要设置的值</param>
        public delegate void MemberModifier<in T>(object target, T value);

        #endregion
    }
}
