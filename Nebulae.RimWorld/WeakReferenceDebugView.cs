using System;
using System.Diagnostics;

namespace Nebulae.RimWorld
{
    internal sealed class WeakReferenceDebugView<T> where T : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly WeakReference<T> _weakReference;

        internal WeakReferenceDebugView(WeakReference<T> weakReference)
        {
            _weakReference = weakReference;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal T Value => _weakReference.TryGetTarget(out var target) ? target : null;
    }
}
