using System.Collections;
using System.Collections.Generic;

namespace QueryParser.Tokens
{
    public class NegationExpressionToken : IConditionalExpressionToken
    {
        public TokenType TokenType => TokenType.NegationExpression;

        public IConditionalExpressionToken Expression { get; set; }

        public override string ToString()
        {
            return $"NOT ({Expression})";
        }

        public IEnumerator<IConditionalExpressionToken> GetEnumerator()
        {
            yield return Expression;
            foreach (var token in Expression)
            {
                yield return token;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return Expression;
            foreach (var token in Expression)
            {
                yield return token;
            }
        }
    }
}
