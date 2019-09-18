using QueryParser.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace QueryParser
{
    public interface IWhereCondition
    {
        IDictionary<string, object> Parameters { get; }
    }

    public class WhereCondition : IWhereCondition
    {
        private IConditionalExpressionToken Expression { get; }

        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public WhereCondition(IConditionalExpressionToken expression)
        {
            this.Expression = expression;
            var conditions = expression
                .Union(new[] { expression })
                .Where(x => x.TokenType == TokenType.ComparisonExpresion)
                .Cast<ComparisonExpressionToken>();

            foreach (var condition in conditions)
            {
                Parameters.Add($"{condition.FieldName}_{condition.Index}", condition.Value.Value);
            }
        }

        public override string ToString()
        {
            return this.Expression.ToString();
        }
    }
}
