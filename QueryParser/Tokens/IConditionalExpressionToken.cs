using System.Collections.Generic;

namespace QueryParser.Tokens
{
    public enum TokenType
    {
        ComparisonExpresion = 0,
        BooleanExpression = 1,
        NegationExpression = 2,
        Value = 3,
        FieldName = 4
    }

    public interface IConditionalExpressionToken : IEnumerable<IConditionalExpressionToken>
    {
        TokenType TokenType { get; }
    }
}
