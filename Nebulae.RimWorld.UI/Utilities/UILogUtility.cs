using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Utilities
{
    internal static class UILogUtility
    {
        private static readonly ConcurrentDictionary<Assembly, string> _assemblyTitleCache = new ConcurrentDictionary<Assembly, string>();


        internal static string GetAssemblyTitle(Type type)
        {
            Assembly assembly = Assembly.GetAssembly(type);
            return _assemblyTitleCache.GetOrAdd(assembly, x =>
                x.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "Unknown Assembly");
        }
    }
}
