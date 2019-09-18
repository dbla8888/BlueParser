using System.Collections;
using System.Collections.Generic;

namespace QueryParser.Tokens
{
    public class ValueToken : IConditionalExpressionToken
    {
        public TokenType TokenType => TokenType.Value;

        public object Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
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
