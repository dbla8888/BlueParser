using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlueParser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod, TestCategory("Parser")]
        public void ParserTestE2E()
        {
            var symbols = new List<Symbol<int>>()
            {
                Symbol<int>.Infix("+", 10,(int left, int right) => left + right),
                Symbol<int>.Infix("-", 10,(int left, int right) => left - right),
                Symbol<int>.Infix("*", 20,(int left, int right) => left * right),
                Symbol<int>.Infix("/", 20,(int left, int right) => left / right),
                Symbol<int>.Prefix("!", 100, (int right) => -right),
                Symbol<int>.Match("<integer>", (str) => Regex.Match(str, @"^\d+").Value.Length, 0, x => int.Parse(x))
            };
            symbols.AddRange(Symbol<int>.Group("(", ")", int.MaxValue));
            var parser = new Parser<int>(symbols);
            var result = parser.Parse("!1+(2*10)");
            Assert.AreEqual(19, result.Value);
        }
    }
}
