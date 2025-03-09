using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Utilities
{
    internal static class UILogUtility
    {
        private static readonly ConcurrentDictionary<Assembly, string> _assemblyTitleCache = new ConcurrentDictionary<Assembly, string>();


        internal static string GetAssemblyTitle(object obj)
        {
            Assembly assembly = Assembly.GetAssembly(obj.GetType());
            return _assemblyTitleCache.GetOrAdd(assembly, x =>
                x.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "Unknown Assembly");
        }
    }
}
