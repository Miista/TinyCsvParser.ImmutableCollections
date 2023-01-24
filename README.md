[MIT License]: https://opensource.org/licenses/MIT

# TinyCsvParser.ImmutableCollections

Adding support for parsing collections in System.Immutable.Collections.

Supports **.NET Core** (.NET Standard 2+)

## Installation

```
PM> Install-Package TinyCsvParser.ImmutableCollections
```

## Support

This extension adds support for the following types (in alphabetical order):
* `ImmutableArray<T>`
* `ImmutableHashSet<T>`
* `ImmutableList<T>`
* `ImmutableSortedSet<T>`
* `ImmutableStack<T>`
* `ImmutableQueue<T>`

## Usage

The only thing you need to keep in mind when using this extension
is that your mapping class must have a constructor taking in an instance of `ITypeConverterProvider`
and passing it on to its base constructor. See example below.

```csharp
// Entity
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

// Parsing
var options = new CsvParserOptions(skipHeader: false, fieldsSeparator: ',');
var typeConverterProvider = new TypeConverterProvider().AddImmutableCollections(); // <-- This line
var parser = new CsvParser<Data>(options, new CsvDataMapping(typeConverterProvider));
var readerOptions = new CsvReaderOptions(new[] { ";" });
var result = parser.ReadFromString(readerOptions, $"0,1,2").ToList();

Console.WriteLine(string.Join(',', result[0].Result.Ints)); // Prints 0,1,2
```

## License

The library is released under terms of the [MIT License].