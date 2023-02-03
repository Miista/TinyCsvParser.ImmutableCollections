using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using TinyCsvParser.ImmutableCollections.Internal;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.ImmutableCollections
{
    public class ImmutableCollectionTypeConverterProviderDecorator : ITypeConverterProvider
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        private static readonly Dictionary<Type, TypePair> TypeMaps = new Dictionary<Type, TypePair>
        {
            { typeof(ImmutableArray<>), new TypePair(typeof(ImmutableArray<>), typeof(ImmutableArrayTypeConverter<>)) },
            { typeof(ImmutableList<>), new TypePair(typeof(ImmutableList<>), typeof(ImmutableListTypeConverter<>)) },
            { typeof(ImmutableHashSet<>), new TypePair(typeof(ImmutableHashSet<>), typeof(ImmutableHashSetTypeConverter<>)) },
            { typeof(ImmutableSortedSet<>), new TypePair(typeof(ImmutableSortedSet<>), typeof(ImmutableSortedSetTypeConverter<>)) },
            { typeof(ImmutableStack<>), new TypePair(typeof(ImmutableStack<>), typeof(ImmutableStackTypeConverter<>)) },
            { typeof(ImmutableQueue<>), new TypePair(typeof(ImmutableQueue<>), typeof(ImmutableQueueTypeConverter<>)) },
        };

        public ImmutableCollectionTypeConverterProviderDecorator(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public ITypeConverter<TTargetType> Resolve<TTargetType>()
        {
            return _typeConverterProvider.Resolve<TTargetType>();
        }

        public IArrayTypeConverter<TTargetType> ResolveCollection<TTargetType>()
        {
            if (typeof(TTargetType).IsGenericType)
            {
                var genericTypeDefinition = typeof(TTargetType).GetGenericTypeDefinition();
                
                if (TypeMaps.TryGetValue(genericTypeDefinition, out var typePair))
                {
                    var specializedType = typeof(TTargetType).GenericTypeArguments.First();
                    var specializedTypePair = typePair.SpecializeTo(specializedType);
                    var arrayTypeConverter = CreateInstance<TTargetType>(specializedTypePair, this);

                    return arrayTypeConverter;
                }
            }

            return _typeConverterProvider.ResolveCollection<TTargetType>();
        }

        private static IArrayTypeConverter<T> CreateInstance<T>(SpecializedTypePair specializedTypePair, ITypeConverterProvider typeConverterProvider)
        {
            var createMethod =
                typeof(ImmutableCollectionTypeConverterProviderDecorator)
                    .GetMethod(nameof(Create), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(specializedTypePair.ConverterType, specializedTypePair.CollectionType)
                ?? throw new InvalidOperationException($"Cannot make static generic method from '{nameof(Create)}");

            var optionalConverterInstance = createMethod.Invoke(null, new object[] { typeConverterProvider })
                                            ?? throw new InvalidOperationException(
                                                $"Cannot make instance of {specializedTypePair.ConverterType} specialized to type '{specializedTypePair.ConcreteType}'"
                                            );

            return optionalConverterInstance as IArrayTypeConverter<T>;
        }

        private static IArrayTypeConverter<TCollection> Create<TConverter, TCollection>(ITypeConverterProvider typeConverterProvider)
        {
            var constructorInfo = GetConstructor(typeof(TConverter));
            var createdInstance = constructorInfo.Invoke(new object[] { typeConverterProvider });
            var arrayTypeConverter = createdInstance as IArrayTypeConverter<TCollection>;

            return arrayTypeConverter;
        }

        private static ConstructorInfo GetConstructor(Type converterType) =>
            converterType.GetConstructor(new[] { typeof(ITypeConverterProvider) })
            ?? throw new InvalidOperationException(
                $"Type {converterType} does not have a public constructor which takes {nameof(ITypeConverterProvider)} as its sole parameter"
            );
    }
}