#BlueParser

BlueParser is a configurable [Pratt parser](https://en.wikipedia.org/wiki/Pratt_parser, useful for building small DSLs

##Basic Usage:
```CSharp
    var symbols = new List<Symbol<int>>()
    {
        Symbol<int>.Infix("+", 10, (int left, int right) => left + right),
        Symbol<int>.Infix("-", 10, (int left, int right) => left - right),
        Symbol<int>.Infix("*", 20, (int left, int right) => left * right),
        Symbol<int>.Infix("/", 20, (int left, int right) => left / right),
        Symbol<int>.Prefix("!", 100, (int right) => -right),
        Symbol<int>.Match("<integer>", (str) => Regex.Match(str, @"^\d+").Value.Length, 0, x => int.Parse(x))
    };
    symbols.AddRange(Symbol<int>.Group("(", ")", int.MaxValue));
    var parser = new Parser<int>(symbols);
    var result = parser.Parse("!1+(2*10)");
    Assert.AreEqual(19, result.Value);
```
The solution also includes a more robust example of a query string parser that take a strongly typed model as an argument

##Limitations

 1. Currently symbols must be unique; most commonly this prevents the use of '-' for both minus and negative.
 2. A parser instance is not thread safe, though is cheap to create