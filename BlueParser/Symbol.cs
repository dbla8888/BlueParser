using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BlueParser
{
    public interface ISymbol<T>
    {
        String Id { get; }
        int BindingPower { get; }
        Func<string, int, int> Scanner { get; }
        Func<string, T> TokenParser { get; }//TODO: This naming convention is confusing, it should be mapper or something like that;
        bool Ignore { get; }
        Func<Parser<T>, T> Nud { get; }
        Func<Parser<T>, T, T> Led { get; }
        ISymbol<T> TokenClone(string id);

    }

    [DebuggerDisplay("Symbol{Id}")]
    public class Symbol<T> : ISymbol<T>
    {
        public string Id { get; set; }
        public int BindingPower { get; set; }
        public Func<string, int, int> Scanner { get; set; }
        public Func<string, T> TokenParser { get; set; }
        public bool Ignore { get; set; }
        public Func<Parser<T>, T> Nud { get; set; }
        public Func<Parser<T>, T, T> Led { get; set; }

        public ISymbol<T> TokenClone(string id)
        {
            return new Symbol<T>()
            {
                Id = id,
                BindingPower = this.BindingPower,
                Nud = this.Nud ?? (this.TokenParser == null ? null : new Func<Parser<T>, T>(parser => TokenParser(id))),//TODO: Should we be more explicit about handling constants?
                Led = this.Led
            };
        }

        private Symbol() { }

        public static Symbol<T> Constant(String id, Func<String, T> selector)
        {
            return new Symbol<T>()
            {
                Id = id,
                BindingPower = 0,
                TokenParser = selector,
                Scanner = ScanByLiteral(id)
            };
        }

        public static Symbol<T> Match(string id, Func<String, int> matcher, int bindingPower, Func<string, T> selector)
        {
            return new Symbol<T>()
            {
                Id = id,
                Scanner = (@string, pos) => pos + matcher(@string.Substring(pos)),
                TokenParser = selector
            };
        }

        public static Symbol<T> Delimited(string id, string delimiter, int bindingPower, Func<string, T> selector)
        {
            var left = new Symbol<T>() { Id = id, BindingPower = bindingPower };
            left.Scanner = (@string, startPos) =>
            {
                if (@string.SubstringEquals(startPos, delimiter))
                {
                    int i = startPos + 1;
                    while (i < @string.Length && !@string.SubstringEquals(i, delimiter))
                    {
                        ++i;
                    }
                    return i + delimiter.Length;
                }
                return startPos;
            };
            left.TokenParser = selector;
            return left;            
        }

        //TODO: separate out id from token match to allow prefix/infix operators to have the same representation
        public static Symbol<T> Prefix(string id, int bindingPower, Func<T,T> selector)
        {
            return new Symbol<T>()
            {
                Id = id,
                BindingPower = bindingPower,
                Nud = (parser) => selector(parser.Parse(bindingPower)),
                Scanner = ScanByLiteral(id)
            };
        }

        public static Symbol<T> Postfix(string id, int bindingPower, Func<T, T> selector)
        {
            return new Symbol<T>()
            {
                Id = id,
                BindingPower = bindingPower,
                Led = (parser, left) => selector(left),
                Scanner = ScanByLiteral(id)
            };
        }

        public static Symbol<T> Infix(string id, int bindingPower, Func<T, T, T> selector)
        {
            return new Symbol<T>()
            {
                Id = id,
                BindingPower = bindingPower,
                Led = (parser, left) => selector(left, parser.Parse(bindingPower)),
                Scanner = ScanByLiteral(id)
            };
        }

        public static Symbol<T> RightInfix(string id, int bindingPower, Func<T, T, T> selector)
        {
            return new Symbol<T>()
            {
                Id = id,
                BindingPower = bindingPower,
                Led = (parser, left) => selector(left, parser.Parse(bindingPower - 1)),
                Scanner = ScanByLiteral(id)
            };
        }

        //TODO: the creation of enumerables makes adding them to the collection harder than 
        //just using the initializer; consider creating custom collection that has IEnumerable Add method
        public static IEnumerable<Symbol<T>> TernaryInfix(string infix0, string infix1, int bindingPower, Func<T,T,T,T> parse)
        {
            return new[]
            {
                new Symbol<T>()
                {
                    Id = infix0,
                    BindingPower = bindingPower,
                    Led = (parser, left) =>
                    {
                        var first = parser.Parse(0);
                        parser.Advance(infix1);
                        var second = parser.Parse(0);
                        return parse(left, first, second);
                    },
                    Scanner = ScanByLiteral(infix0)
                },
                new Symbol<T>()
                {
                    Id = infix1,
                    Scanner = ScanByLiteral(infix1)
                }
            };
        }

        public static IEnumerable<Symbol<T>> TernaryPrefix(string prefix, string infix0, string infix1, int bindingPower, Func<T, T, T, T> parse)
        {
            return new[]
            {
                new Symbol<T>()
                {
                    Id = infix0,
                    BindingPower = bindingPower,
                    Nud = parser =>
                    {
                        var first = parser.Parse(bindingPower);
                        parser.Advance(infix0);
                        var second = parser.Parse(0);
                        parser.Advance(infix1);
                        var third = parser.Parse(0);
                        return parse(first, second, third);
                    },
                    Scanner = ScanByLiteral(prefix)
                },
                new Symbol<T>()
                {
                    Id = infix0,
                    Scanner = ScanByLiteral(infix0)
                },
                new Symbol<T>()
                {
                    Id = infix1,
                    Scanner = ScanByLiteral(infix1)
                }
            };
        }

        public static IEnumerable<Symbol<T>> Group(string leftgrouping, string rightGrouping, int bindingPower)
        {
            return new[]
            {
                new Symbol<T>()
                {
                    Id = leftgrouping,
                    BindingPower = bindingPower,
                    Nud = parser =>
                    {
                        var e = parser.Parse(0);
                        parser.Advance(rightGrouping);
                        return e;
                    },
                    Scanner = ScanByLiteral(leftgrouping)
                },
                new Symbol<T>()
                {
                    Id = rightGrouping,
                    Scanner = ScanByLiteral(rightGrouping)
                }
            };
        }

        public static Symbol<T> Skip(Symbol<T> symbol)
        {
            symbol.Ignore = true;
            return symbol;
        }

        internal static Symbol<T> End { get; } = new Symbol<T>() { Id = "<end>", BindingPower = int.MinValue };

        private static Func<string, int, int> ScanByLiteral(string value)
        {
            return (input, pos) => input.SubstringEquals(pos, value) ? pos + value.Length : pos;
        }
    }
}
