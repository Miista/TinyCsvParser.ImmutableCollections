using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.ImmutableCollections
{
    public static class TypeConverterProviderExtensions
    {
        public static ITypeConverterProvider AddImmutableCollections(this ITypeConverterProvider typeConverterProvider)
        {
            if (typeConverterProvider == null) throw new ArgumentNullException(nameof(typeConverterProvider));

            return new ImmutableCollectionTypeConverterProviderDecorator(typeConverterProvider);
        }
    }
}