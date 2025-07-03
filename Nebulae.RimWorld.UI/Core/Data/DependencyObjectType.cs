using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data
{
    internal sealed class DependencyObjectType
    {
        private DependencyObjectType(Type type)
        {
            Type = type;

            if (type.BaseType != RootType)
            {
                Base = From(type.BaseType);
            }
            else
            {
                Base = null;
            }
        }


        internal static DependencyObjectType From(Type type)
        {
            return _createdTypes.GetOrAdd(type, t => new DependencyObjectType(t));
        }

        internal bool IsSubclassOf(DependencyObjectType dType)
        {
            return Base != null && (ReferenceEquals(Base, dType) || Base.IsSubclassOf(dType));
        }


        internal static readonly IComparer<DependencyObjectType> Comparer = new DependencyObjectTypeComparer();
        internal static readonly Type RootType = typeof(DependencyObject);

        internal readonly DependencyObjectType Base;
        internal readonly Type Type;


        private static readonly ConcurrentDictionary<Type, DependencyObjectType> _createdTypes = new ConcurrentDictionary<Type, DependencyObjectType>();


        private class DependencyObjectTypeComparer : IComparer<DependencyObjectType>
        {
            public int Compare(DependencyObjectType x, DependencyObjectType y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                return x.IsSubclassOf(y) ? -1 : 1;
            }
        }
    }
}
