using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using TinyCsvParser.Mapping;
using TinyCsvParser.Ranges;
using TinyCsvParser.TypeConverter;
using Xunit;

namespace TinyCsvParser.ImmutableCollections.Tests
{
    public class AddImmutableCollectionsTests
    {
        public class SingleValueCollections
        {
            public class Data<T>
            {
                public class Mapping : CsvMapping<Data<T>>
                {
                    public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
                    {
                        MapProperty(new RangeDefinition(0, 1), x => x.ImmutableArray);
                        MapProperty(new RangeDefinition(0, 1), x => x.ImmutableStack);
                        MapProperty(new RangeDefinition(0, 1), x => x.ImmutableQueue);
                        MapProperty(new RangeDefinition(0, 1), x => x.ImmutableList);
                        MapProperty(new RangeDefinition(0, 1), x => x.PrimitiveArray);
                        MapProperty(new RangeDefinition(0, 1), x => x.ImmutableSet);
                        MapProperty(new RangeDefinition(0, 1), x => x.ImmutableSortedSet);
                    }
                }

                // ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618
                public ImmutableArray<T> ImmutableArray { get; set; }
                public ImmutableList<T> ImmutableList { get; set; }
                public ImmutableStack<T> ImmutableStack { get; set; }
                public ImmutableQueue<T> ImmutableQueue { get; set; }
                public ImmutableHashSet<T> ImmutableSet { get; set; }
                public ImmutableSortedSet<T> ImmutableSortedSet { get; set; }
                public T[] PrimitiveArray { get; set; }
                // ReSharper restore UnusedAutoPropertyAccessor.Global
#pragma warning restore CS8618
            }

            [Theory]
            [MemberData(nameof(Test_Data))]
            public void Test<T>(
                string csvData,
                ImmutableArray<T> expectedImmutableArray,
                ImmutableList<T> expectedImmutableList,
                T[] expectedArray,
                ImmutableQueue<T> expectedImmutableQueue,
                ImmutableStack<T> expectedImmutableStack,
                ImmutableHashSet<T> expectedImmutableSet,
                ImmutableSortedSet<T> expectedImmutableSortedSet
            )
            {
                // Arrange
                var (parser, readerOptions) = CreateParser<T>();

                // Act
                var results = parser.ReadFromString(readerOptions, csvData).ToList();

                // Assert
                results.Should().NotBeNullOrEmpty();
                results.First().Result.ImmutableArray.Should().BeEquivalentTo(expectedImmutableArray, because: "that was the values passed");
                results.First().Result.ImmutableList.Should().BeEquivalentTo(expectedImmutableList, because: "that was the values passed");
                results.First().Result.ImmutableQueue.Should().BeEquivalentTo(expectedImmutableQueue, because: "that was the values passed");
                results.First().Result.ImmutableStack.Should().BeEquivalentTo(expectedImmutableStack, because: "that was the values passed");
                results.First().Result.ImmutableSet.Should().BeEquivalentTo(expectedImmutableSet, because: "that was the values passed");
                results.First().Result.ImmutableSortedSet.Should().BeEquivalentTo(expectedImmutableSortedSet, because: "that was the values passed");
                results.First().Result.PrimitiveArray.Should().BeEquivalentTo(expectedArray, because: "that was the values passed");
            }

            // ReSharper disable once InconsistentNaming
            [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
            public static IEnumerable<object[]> Test_Data
            {
                get
                {
                    var fixture = new Fixture();

                    yield return TestCase(fixture.CreateMany<int>(2));
                    yield return TestCase(fixture.CreateMany<float>(2));
                    yield return TestCase(fixture.CreateMany<double>(2));
                    yield return TestCase(fixture.CreateMany<string>(2));
                    yield return TestCase(fixture.CreateMany<byte>(2));
                    yield return TestCase(fixture.CreateMany<short>(2));
                    yield return TestCase(fixture.CreateMany<ushort>(2));
                    yield return TestCase(fixture.CreateMany<ulong>(2));
                    yield return TestCase(fixture.CreateMany<long>(2));
                    yield return TestCase(fixture.CreateMany<sbyte>(2));
                    yield return TestCase(fixture.CreateMany<uint>(2));

                    object[] TestCase<T>(IEnumerable<T> values)
                    {
                        var csvData = $"Value1,Value2;{string.Join(',', values)}";

                        return new object[]
                        {
                            csvData,
                            values.ToImmutableArray(),
                            values.ToImmutableList(),
                            values.ToArray(),
                            ImmutableQueue.CreateRange(values),
                            ImmutableStack.CreateRange(values),
                            values.ToImmutableHashSet(),
                            values.ToImmutableSortedSet()
                        };
                    }
                }
            }
            
            private static (ICsvParser<Data<T>> Parser, CsvReaderOptions ReaderOptions) CreateParser<T>()
            {
                var options = new CsvParserOptions(skipHeader: true, fieldsSeparator: ',');
                var typeConverterProvider = new TypeConverterProvider().AddImmutableCollections(); 
                var parser = new CsvParser<Data<T>>(options, new Data<T>.Mapping(typeConverterProvider));
                var readerOptions = new CsvReaderOptions(new[] { ";" });

                return (Parser: parser, ReaderOptions: readerOptions);
            }
        }
    }
}