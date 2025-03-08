using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nebulae.RimWorld.UI.Automation
{
    internal readonly struct DebugInfo
    {
        internal static readonly Comparison<DebugInfo> Comparison = Compare;


        private static readonly Dictionary<string, Func<object, object>> _cachedMemberAccessers =
            new Dictionary<string, Func<object, object>>();
        private static readonly StringBuilder _stringBuilder = new StringBuilder();

        private readonly Func<object, object> _accesser;
        private readonly string _name;
        private readonly int _priority;
        private readonly object _target;


        internal string Info
        {
            get
            {
                var result = _stringBuilder.Append(_name).Append(_accesser(_target)?.ToString() ?? "Null").Append("\n").ToString();
                _stringBuilder.Clear();
                return result;
            }
        }


        internal DebugInfo(
            Func<object, object> accesser,
            string name,
            int priority,
            object target)
        {
            _accesser = accesser;
            _name = _stringBuilder.Append("<color=yellow>").Append(name).Append("</color>:\n").ToString();
            _priority = priority;
            _target = target;
            
            _stringBuilder.Clear();
        }


        internal static int Compare(DebugInfo x, DebugInfo y)
        {
            return x._priority<y._priority? -1 : 1;
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
