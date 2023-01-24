using System;
using System.Collections.Immutable;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.ImmutableCollections
{
    public class ImmutableStackTypeConverter<T> : IArrayTypeConverter<ImmutableStack<T>>
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public ImmutableStackTypeConverter(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public bool TryConvert(string[] value, out ImmutableStack<T> result)
        {
            result = ImmutableStack<T>.Empty;
            
            var innerTypeConverter = _typeConverterProvider.ResolveCollection<T[]>();
            
            if (innerTypeConverter.TryConvert(value, out var values))
            {
                result = ImmutableStack.CreateRange(values);

                return true;
            }

            return false;
        }

        public Type TargetType { get; } = typeof(ImmutableStack<T>);
    }
}