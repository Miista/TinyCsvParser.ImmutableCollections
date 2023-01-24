using System;
using System.Collections.Immutable;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.ImmutableCollections
{
    public class ImmutableListTypeConverter<T> : IArrayTypeConverter<ImmutableList<T>>
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public ImmutableListTypeConverter(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public bool TryConvert(string[] value, out ImmutableList<T> result)
        {
            result = ImmutableList<T>.Empty;
            
            var innerTypeConverter = _typeConverterProvider.ResolveCollection<T[]>();
            
            if (innerTypeConverter.TryConvert(value, out var values))
            {
                result = values.ToImmutableList();

                return true;
            }

            return false;
        }

        public Type TargetType { get; } = typeof(ImmutableList<T>);
    }
}