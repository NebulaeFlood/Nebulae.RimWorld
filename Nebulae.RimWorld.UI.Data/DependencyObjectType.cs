using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebulae.RimWorld.UI.Data
{
    /// <summary>
    /// 用于寻找依赖属性值的依赖对象类标识
    /// </summary>
    internal class DependencyObjectType : IComparable<DependencyObjectType>
    {
        internal static readonly IComparer<DependencyObjectType> Comparer = new DependencyObjectTypeComparer();
        internal static readonly Type RootType = typeof(DependencyObject);

        private static readonly Dictionary<Type, DependencyObjectType> _globalTypes = new Dictionary<Type, DependencyObjectType>();
        private static int _globalCount = 0;


        internal readonly Type OriginalType;
        internal int Rank;

        private readonly int _hashCode;
        private DependencyObjectType _baseType;

        private DependencyObjectType(Type originalType)
        {
            OriginalType = originalType;
            Rank = 1;

            _hashCode = _globalCount++;
        }


        public int CompareTo(DependencyObjectType other)
        {
            if (Rank < other.Rank)
            {
                return -1;
            }
            else if (Rank > other.Rank)
            {
                return 1;
            }
            return 0;
        }

        public override int GetHashCode() => _hashCode;


        internal DependencyObjectType GetUpgradedBase(int rank)
        {
            DependencyObjectType baseType = _baseType;
            while (baseType != null)
            {
                if (baseType.Rank > rank)
                {
                    return baseType;
                }
                baseType = baseType._baseType;
            }
            return null;
        }

        internal void Upgrade()
        {
            Rank++;
            _baseType?.Upgrade();
        }


        internal static DependencyObjectType FromType(Type type)
        {
            if (!_globalTypes.TryGetValue(type, out DependencyObjectType dType))
            {
                dType = new DependencyObjectType(type);

                if (type != RootType)
                {
                    dType._baseType = FromType(type.BaseType);
                }

                _globalTypes[type] = dType;
            }

            return dType;
        }


        private class DependencyObjectTypeComparer : IComparer<DependencyObjectType>
        {
            public int Compare(DependencyObjectType x, DependencyObjectType y)
            {
                if (x.Rank < y.Rank)
                {
                    return -1;
                }

                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (x.Rank == y.Rank
                    && y.OriginalType.IsAssignableFrom(x.OriginalType))
                {
                    return -1;
                }

                return 1;
            }
        }
    }
}
