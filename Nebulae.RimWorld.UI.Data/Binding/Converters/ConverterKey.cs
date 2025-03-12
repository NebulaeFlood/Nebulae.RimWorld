using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Data.Binding.Converters
{
    internal readonly struct ConverterKey : IEquatable<ConverterKey>
    {
        private readonly int _hashCode;
        private readonly Type _sourceType;
        private readonly Type _targetType;

        internal ConverterKey(Type sourceType, Type targetType)
        {
            _hashCode = sourceType.GetHashCode() ^ targetType.GetHashCode();
            _sourceType = sourceType;
            _targetType = targetType;
        }

        public override bool Equals(object obj)
        {
            return obj is ConverterKey other
                && ((_sourceType == other._sourceType && _targetType == other._targetType)
                    || (_sourceType == other._targetType && _targetType == other._sourceType));
        }

        public bool Equals(ConverterKey other)
        {
            return (_sourceType == other._sourceType && _targetType == other._targetType)
                    || (_sourceType == other._targetType && _targetType == other._sourceType);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
