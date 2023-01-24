using System;
using System.Collections.Immutable;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.ImmutableCollections
{
    public class ImmutableHashSetTypeConverter<T> : IArrayTypeConverter<ImmutableHashSet<T>>
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public ImmutableHashSetTypeConverter(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public bool TryConvert(string[] value, out ImmutableHashSet<T> result)
        {
            result = ImmutableHashSet<T>.Empty;
            
            var innerTypeConverter = _typeConverterProvider.ResolveCollection<T[]>();
            
            if (innerTypeConverter.TryConvert(value, out var values))
            {
                result = values.ToImmutableHashSet();

                return true;
            }

            return false;
        }

        public Type TargetType { get; } = typeof(ImmutableHashSet<T>);
    }
}