using System;
using System.Collections.Immutable;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.ImmutableCollections
{
    public class ImmutableSortedSetTypeConverter<T> : IArrayTypeConverter<ImmutableSortedSet<T>>
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public ImmutableSortedSetTypeConverter(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public bool TryConvert(string[] value, out ImmutableSortedSet<T> result)
        {
            result = ImmutableSortedSet<T>.Empty;
            
            var innerTypeConverter = _typeConverterProvider.ResolveCollection<T[]>();
            
            if (innerTypeConverter.TryConvert(value, out var values))
            {
                result = values.ToImmutableSortedSet();

                return true;
            }

            return false;
        }

        public Type TargetType { get; } = typeof(ImmutableSortedSet<T>);
    }
}