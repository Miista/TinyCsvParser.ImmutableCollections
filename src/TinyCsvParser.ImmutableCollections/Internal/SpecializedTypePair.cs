using System;

namespace TinyCsvParser.ImmutableCollections.Internal
{
    internal readonly struct SpecializedTypePair
    {
        public Type CollectionType { get; }
        public Type ConverterType { get; }
        public Type ConcreteType { get; }

        public SpecializedTypePair(Type collectionType, Type converterType, Type concreteType)
        {
            CollectionType = collectionType ?? throw new ArgumentNullException(nameof(collectionType));
            ConverterType = converterType ?? throw new ArgumentNullException(nameof(converterType));
            ConcreteType = concreteType ?? throw new ArgumentNullException(nameof(concreteType));
        }        
    }
}