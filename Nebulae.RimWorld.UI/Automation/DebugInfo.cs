using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Automation
{
    internal readonly struct DebugInfo
    {
        internal static readonly Comparison<DebugInfo> Comparison = (x, y) =>
            x._priority < y._priority ? -1 : 1;


        private static readonly Dictionary<string, Func<object, object>> _cachedMemberAccessers =
            new Dictionary<string, Func<object, object>>();

        private readonly Func<object, object> _accesser;
        private readonly string _name;
        private readonly int _priority;
        private readonly object _target;


        internal string Info =>
            "<color=yellow>" + _name + "</color>:\n" + _accesser(_target)?.ToString() ?? "Null\n";


        internal DebugInfo(
            Func<object, object> accesser,
            string name,
            int priority,
            object target)
        {
            _accesser = accesser;
            _name = name;
            _target = target;

            _priority = priority;
        }


        internal static bool TryCreate(MemberInfo member, object target, out DebugInfo info)
        {
            if (member.GetCustomAttribute<DebugInfoEntryAttribute>() is DebugInfoEntryAttribute entry)
            {
                string key = member.DeclaringType.FullName + "." + member.Name;
                string name = string.IsNullOrEmpty(entry.Name)
                    ? member.Name
                    : entry.Name;

                if (_cachedMemberAccessers.TryGetValue(key, out var accesser))
                {
                    info = new DebugInfo(accesser, name, entry.Priority, target);
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
                    info = new DebugInfo(accesser, name, entry.Priority, target);

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
                        info = new DebugInfo(accesser, name, entry.Priority, target);

                        _cachedMemberAccessers[key] = accesser;

                        return true;
                    }
                }
            }

            info = default;
            return false;
        }
    }
}
