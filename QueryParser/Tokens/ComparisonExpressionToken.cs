using System.Collections;
using System.Collections.Generic;

namespace QueryParser.Tokens
{
    public class ComparisonExpressionToken : IConditionalExpressionToken
    {
        public TokenType TokenType => TokenType.ComparisonExpresion;
        private static int GlobalIndex { get; set; }
        public int Index { get; }

        public ComparisonExpressionToken()
        {
            //TODO: Don't love this
            this.Index = GlobalIndex;
            GlobalIndex++;
            GlobalIndex %= 1024;
        }

        public ValueToken Value { get; set; }
        public ConditionalOperator Operator { get; set; }
        public FieldNameToken FieldName { get; set; }

        public override string ToString()
        {
            return $" {FieldName} {Operator} @{FieldName}_{Index}";
        }

        public IEnumerator<IConditionalExpressionToken> GetEnumerator()
        {
            yield return FieldName;
            yield return Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return FieldName;
            yield return Value;
        }
    }
}
