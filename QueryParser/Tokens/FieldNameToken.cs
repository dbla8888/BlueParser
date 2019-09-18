using System.Collections;
using System.Collections.Generic;

namespace QueryParser.Tokens
{
    public class FieldNameToken : IConditionalExpressionToken
    {
        public TokenType TokenType => TokenType.FieldName;

        public string FieldName { get; set; }

        public override string ToString()
        {
            return this.FieldName;
        }

        public IEnumerator<IConditionalExpressionToken> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }
    }
}
