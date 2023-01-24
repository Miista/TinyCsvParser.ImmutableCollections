using System.Collections.Immutable;
using TinyCsvParser;
using TinyCsvParser.ImmutableCollections;
using TinyCsvParser.Mapping;
using TinyCsvParser.Ranges;
using TinyCsvParser.TypeConverter;

namespace Sandbox
{
    public class Data
    {
        public class Mapping : CsvMapping<Data>
        {
            public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
            {
                MapProperty(new RangeDefinition(0, 1), x => x.Ints);
                MapProperty(new RangeDefinition(2, 3), x => x.Strings);
                MapProperty(new RangeDefinition(4, 5), x => x.IntSet);
            }
        }
        public ImmutableList<int> Ints { get; set; }
        public ImmutableList<string> Strings { get; set; }
        public ImmutableHashSet<int> IntSet { get; set; }
    }
    
    public class Program
    {
        private class Data
        {
            public ImmutableList<int> Ints { get; set; }
        }

// Mapping
        private class CsvDataMapping : CsvMapping<Data>
        {
            // Need to take in ITypeConverterProvider
            public CsvDataMapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
            {
                MapProperty(new RangeDefinition(0, 2), x => x.Ints);
            }
        }
        
        public static void Main(string[] args)
        {
            
       

// Parsing
        var options = new CsvParserOptions(skipHeader: false, fieldsSeparator: ',');
        var typeConverterProvider = new TypeConverterProvider().AddImmutableCollections(); // <-- This line
        var parser = new CsvParser<Data>(options, new CsvDataMapping(typeConverterProvider));
        var readerOptions = new CsvReaderOptions(new[] { ";" });
        var result = parser.ReadFromString(readerOptions, $"0,1,2").ToList();

        Console.WriteLine(string.Join(',', result[0].Result.Ints)); // 0,1,2
            
            // var options = new CsvParserOptions(skipHeader: true, fieldsSeparator: ',');
            // var typeConverterProvider = new TypeConverterProvider().AddImmutableCollections();
            // var parser = new CsvParser<Data>(options, new Data.Mapping(typeConverterProvider));
            // var readerOptions = new CsvReaderOptions(new[] { ";" });
            // var results = parser.ReadFromString(readerOptions, $"Value1,Value2,Value3,Value4,Value5,Value6;0,1,2,3,4,5").ToList();
            //
            // Console.WriteLine($"Results: {results.Count}");
            // foreach (var result in results)
            // {
            //     if (result.IsValid) Console.WriteLine(result.Result.ToString());
            //     else Console.WriteLine($"Result is invalid: {result.Error}");
            // }

        }
    }
}