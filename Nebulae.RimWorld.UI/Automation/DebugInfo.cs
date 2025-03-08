using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Automation
{
    internal struct DebugInfo
    {
        private static readonly Dictionary<string, Func<object, object>> _cachedMemberAccessers =
            new Dictionary<string, Func<object, object>>();

        private readonly Func<object, object> _accesser;
        private readonly MemberInfo _member;
        private readonly object _target;


        internal string Info =>
            "<color=yellow>" + _member.Name + "</color>:\n" + _accesser(_target)?.ToString() ?? "Null";


        internal DebugInfo(Func<object, object> accesser, MemberInfo member, object target)
        {
            _accesser = accesser;
            _member = member;
            _target = target;
        }


        internal static bool TryCreate(MemberInfo member, object target, out DebugInfo info)
        {
            if (member.GetCustomAttribute<DebugInfoEntryAttribute>() is null)
            {
                info = default;
                return false;
            }

            string key = member.DeclaringType.FullName + "." + member.Name;

            if (_cachedMemberAccessers.TryGetValue(key, out var accesser))
            {
                info = new DebugInfo(accesser, member, target);
                return true;
            }

            if (member is FieldInfo field)
            {
                ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
                MemberExpression fieldExp = Expression.Field(
                    field.IsStatic ? null : Expression.Convert(targetExp, field.DeclaringType),
                    field);

                accesser = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(fieldExp, typeof(object)),
                    targetExp).Compile();
                info = new DebugInfo(accesser, member, target);

                _cachedMemberAccessers[key] = accesser;

                return true;
            }
            else if (member is PropertyInfo property)
            {
                if (property.GetMethod is MethodInfo propertyGetter)
                {
                    ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
                    MemberExpression propertyExp = Expression.Property(
                        propertyGetter.IsStatic ? null : Expression.Convert(targetExp, property.DeclaringType),
                        property);

                    accesser = Expression.Lambda<Func<object, object>>(
                            Expression.Convert(propertyExp, typeof(object)),
                            targetExp).Compile();
                    info = new DebugInfo(accesser, member, target);

                    _cachedMemberAccessers[key] = accesser;

                    return true;
                }

                info = default;
                return false;
            }

            info = default;
            return false;
        }
    }
}
