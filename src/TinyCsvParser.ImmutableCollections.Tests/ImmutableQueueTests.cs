using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using TinyCsvParser.Mapping;
using TinyCsvParser.Ranges;
using TinyCsvParser.TypeConverter;
using Xunit;

namespace TinyCsvParser.ImmutableCollections.Tests
{
    public class ImmutableQueueTests
    {
        public class Data
        {
            public class Mapping : CsvMapping<Data>
            {
                public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
                {
                    MapProperty(new RangeDefinition(0, 1), x => x.Ints);
                }
            }
        
            // ReSharper disable once IdentifierTypo
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618
            public ImmutableQueue<int> Ints { get; set; }
#pragma warning restore CS8618
        }

        [Fact]
        public void Test()
        {
            // Arrange
            var (parser, readerOptions) = CreateParser();

            // Act
            var results = parser.ReadFromString(readerOptions, $"Value1,Value2;0,1").ToList();
        
            // Assert
            results.Should().NotBeNullOrEmpty();
            results.First().Result.Ints.Should().BeEquivalentTo(ImmutableQueue<int>.Empty.Enqueue(0).Enqueue(1), because: "that was the values passed");
        }

        private static (ICsvParser<Data> Parser, CsvReaderOptions ReaderOptions) CreateParser()
        {
            var options = new CsvParserOptions(skipHeader: true, fieldsSeparator: ',');
            var typeConverterProvider = new TypeConverterProvider(); 
            typeConverterProvider = typeConverterProvider.Add(new ImmutableQueueTypeConverter<int>(typeConverterProvider));
            var parser = new CsvParser<Data>(options, new Data.Mapping(typeConverterProvider));
            var readerOptions = new CsvReaderOptions(new[] { ";" });

            return (Parser: parser, ReaderOptions: readerOptions);
        }
    }
}