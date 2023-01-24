using System;
using System.Collections.Immutable;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.ImmutableCollections
{
    public class ImmutableArrayTypeConverter<T> : IArrayTypeConverter<ImmutableArray<T>>
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public ImmutableArrayTypeConverter(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public bool TryConvert(string[] value, out ImmutableArray<T> result)
        {
            result = ImmutableArray<T>.Empty;
            
            var innerTypeConverter = _typeConverterProvider.ResolveCollection<T[]>();
            
            if (innerTypeConverter.TryConvert(value, out var values))
            {
                result = values.ToImmutableArray();

                return true;
            }

            return false;
        }

        public Type TargetType { get; } = typeof(ImmutableArray<T>);
    }
}