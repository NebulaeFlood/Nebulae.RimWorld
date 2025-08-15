using Nebulae.RimWorld.UI.Core.Data.Bindings;
using Nebulae.RimWorld.Utilities;
using System;
using System.Reflection;
using System.Text;

namespace Nebulae.RimWorld.UI.Automation.Diagnostics
{
    internal readonly struct DebugMember
    {
        public static readonly Comparison<DebugMember> Comparison = Compare;
        public static readonly DebugMember Empty = new DebugMember();

        public readonly BindingMember Member;
        public readonly string Name;
        public readonly int Priority;


        public string Value
        {
            get
            {
                StringBuilder.Clear();
                return StringBuilder
                    .Append(Name)
                    .AppendLine(Member.Value.AsLog())
                    .ToString();
            }
        }


        private DebugMember(object target, MemberInfo member, string name, int priority)
        {
            StringBuilder.Clear();

            Member = new BindingMember(target, member);
            Name = StringBuilder
                .Append("<color=yellow>")
                .Append(name)
                .AppendLine("</color>:")
                .ToString();
            Priority = priority;
        }


        public static bool TryCreate(object target, MemberInfo member, out DebugMember debugMember)
        {
            if (member.GetCustomAttribute(typeof(DebugMemberAttribute)) is DebugMemberAttribute attribute)
            {
                debugMember = new DebugMember(target, member, string.IsNullOrEmpty(attribute.Name) ? member.Name : attribute.Name, attribute.Priority);
                return true;
            }
            else
            {
                debugMember = Empty;
                return false;
            }
        }


        private static int Compare(DebugMember x, DebugMember y)
        {
            return x.Priority > y.Priority ? 1 : -1;
        }


        private static readonly StringBuilder StringBuilder = new StringBuilder(64);
    }
}
