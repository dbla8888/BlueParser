using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.Tokens
{
    public class BooleanExpressionToken : IConditionalExpressionToken
    {
        public TokenType TokenType => TokenType.BooleanExpression;

        public IConditionalExpressionToken Rhs { get; set; }
        public ConditionalOperator Operator { get; set; }
        public IConditionalExpressionToken  Lhs { get; set; }

        public override string ToString()
        {
            return $" {Lhs} {Operator} {Rhs}";
        }

        public IEnumerator<IConditionalExpressionToken> GetEnumerator()
        {
            yield return Lhs;
            foreach (var token in Lhs)
            {
                yield return token;
            }

            yield return Rhs;
            foreach (var token in Rhs)
            {
                yield return token;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return Lhs;
            foreach (var token in Lhs)
            {
                yield return token;
            }

            yield return Rhs;
            foreach (var token in Rhs)
            {
                yield return token;
            }
        }
    }
}
