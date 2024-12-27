using System.Collections.Generic;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 绑定关系管理器
    /// </summary>
    public static class BindingManager
    {
        private static readonly HashSet<IBinding> _globalBindings = new HashSet<IBinding>();


        internal static bool IsBinding(IBinding binding) => _globalBindings.Contains(binding);
    }
}
