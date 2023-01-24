using System;

namespace TinyCsvParser.ImmutableCollections.Internal
{
    internal readonly struct TypePair
    {
        public Type CollectionType { get; }
        public Type ConverterType { get; }

        public TypePair(Type collectionType, Type converterType)
        {
            CollectionType = collectionType ?? throw new ArgumentNullException(nameof(collectionType));
            ConverterType = converterType ?? throw new ArgumentNullException(nameof(converterType));
        }

        public SpecializedTypePair SpecializeTo(Type specialization)
        {
            var specializedCollectionType = CollectionType.GetGenericTypeDefinition().MakeGenericType(specialization);
            var specializedConverterType = ConverterType.GetGenericTypeDefinition().MakeGenericType(specialization);

            return new SpecializedTypePair(specializedCollectionType, specializedConverterType, specialization);
        }
    }
}