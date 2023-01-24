using System;
using System.Collections.Immutable;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.ImmutableCollections
{
    public class ImmutableQueueTypeConverter<T> : IArrayTypeConverter<ImmutableQueue<T>>
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public ImmutableQueueTypeConverter(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public bool TryConvert(string[] value, out ImmutableQueue<T> result)
        {
            result = ImmutableQueue<T>.Empty;
            
            var innerTypeConverter = _typeConverterProvider.ResolveCollection<T[]>();
            
            if (innerTypeConverter.TryConvert(value, out var values))
            {
                result = ImmutableQueue.CreateRange(values);

                return true;
            }

            return false;
        }

        public Type TargetType { get; } = typeof(ImmutableQueue<T>);
    }
}