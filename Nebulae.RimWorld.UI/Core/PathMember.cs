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
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        internal PathMember(PathMemberInfo info)
        {
            Info = info;
        }

        internal PathMember(PathMemberInfo info, MemberAccessor accessor, MemberModifier modifier)
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
