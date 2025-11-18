using Nebulae.RimWorld.UI.Core.Data;
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
        /// <summary>
        /// 成员信息
        /// </summary>
        public readonly PathMemberInfo Info;


        internal PathMember(PathMemberInfo info)
        {
            Info = info;
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
        /// <param name="other">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(PathMember other)
        {
            // 由于 PathMember 只由 MamberPath 创建，
            // 而相同路径的 MemberPath 使用同一个对象，
            // 因此只需比较两个 PathMember 是否指向同一个对象即可。
            return this == other;
        }

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
