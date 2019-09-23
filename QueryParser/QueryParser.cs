using BlueParser;
using System;
using System.Collections;
using System.Collections.Generic;
using QueryParser.Tokens;
using Token = QueryParser.Tokens.IConditionalExpressionToken;
using System.Text.RegularExpressions;

namespace QueryParser
{
    public interface IQueryParser<TModel>
    {
        IResult<Token> Parse(string @string);
    }

    public class QueryParser<TModel> : IQueryParser<TModel>
    {
        private Parser<Token> Parser { get; set; }
        private IEnumerable<Symbol<Token>> Symbols { get; set; }

        public QueryParser()
        {
            //TODO: Define different value tokens
            //TODO: Typechecking on ComparisonExpressionTokens
            //TODO: Use instance regex
            //TODO: Use type info from properties to validate parsed types
            var symbols = new List<Symbol<Token>>()
            {
                Symbol<Token>.Infix("=", 10, (Token lhs, Token rhs) => new ComparisonExpressionToken() { FieldName = (FieldNameToken)lhs, Operator = ConditionalOperator.EQUALS, Value = (ValueToken)rhs }),
                Symbol<Token>.Infix("<>", 10, (Token lhs, Token rhs) => new ComparisonExpressionToken() { FieldName = (FieldNameToken)lhs, Operator = ConditionalOperator.NOT_EQUALS, Value = (ValueToken)rhs }),
                Symbol<Token>.Infix(">", 10, (Token lhs, Token rhs) => new ComparisonExpressionToken() { FieldName = (FieldNameToken)lhs, Operator = ConditionalOperator.GREATER_THAN, Value = (ValueToken)rhs }),
                Symbol<Token>.Infix("<", 10, (Token lhs, Token rhs) => new ComparisonExpressionToken() { FieldName = (FieldNameToken)lhs, Operator = ConditionalOperator.LESS_THAN, Value = (ValueToken)rhs }),
                Symbol<Token>.Infix(">=", 10, (Token lhs, Token rhs) => new ComparisonExpressionToken() { FieldName = (FieldNameToken)lhs, Operator = ConditionalOperator.GREATER_THAN_OR_EQUAL, Value = (ValueToken)rhs }),
                Symbol<Token>.Infix("<=", 10, (Token lhs, Token rhs) => new ComparisonExpressionToken() { FieldName = (FieldNameToken)lhs, Operator = ConditionalOperator.LESS_THAN_OR_EQUAL, Value = (ValueToken)rhs }),
                Symbol<Token>.Infix("&", 5, (Token lhs, Token rhs) => new BooleanExpressionToken() { Lhs = lhs, Operator = ConditionalOperator.AND, Rhs = rhs }),
                Symbol<Token>.Infix("|", 5, (Token lhs, Token rhs) => new BooleanExpressionToken() { Lhs = lhs, Operator = ConditionalOperator.OR, Rhs = rhs }),
                Symbol<Token>.Prefix("!", 100, (Token exp) => new NegationExpressionToken() { Expression = exp }),
                Symbol<Token>.Delimited("<string>", "'", 0, (string x) => new ValueToken() { Value = x.Trim('\'') }),
                Symbol<Token>.Match(
                    "<number>",
                    (string @string)=> Regex.Match(@string, @"^(?:\d*[.,])?\d+").Length,
                    1,
                    x =>
                    {
                        if(int.TryParse(x, out int @int))
                        {
                            return new ValueToken() { Value = @int };
                        }else if (decimal.TryParse(x, out decimal @decimal))
                        {
                            return new ValueToken() { Value = @decimal };
                        }
                        throw new Exception($"Unable to parse '{x}' as numeric");
                    }),
            };

            symbols.AddRange(Symbol<Token>.Group("(", ")", int.MaxValue - 100));

            foreach(var fieldName in typeof(TModel).GetQueryMembers())
            {
                symbols.Add(Symbol<Token>.Constant(fieldName, x => new FieldNameToken() { FieldName = x }));
            }

            this.Symbols = symbols;
        }

        public IResult<Token> Parse(string @string)
        {
            //Parser is not currently (8/14/17) thread safe, so each call gets it's own instance
            this.Parser = new Parser<Token>(this.Symbols);
            var parseResult = Parser.Parse(@string);
            return parseResult.IsSuccess ?
                new Result<Token>(parseResult.Value) :
                new Result<Token>(new AggregateException(parseResult.Exceptions));   
        }
    }
}
