using System.Collections.Generic;

namespace QueryParser.Tokens
{
    public class ConditionalOperator
    {
        private Operators Op { get; }
        internal string OpString { get; }
        internal enum Operators
        {
            EQUALS,
            NOT_EQUALS,
            GREATER_THAN,
            LESS_THAN,
            GREATER_THAN_OR_EQUAL,
            LESS_THAN_OR_EQUAL,
            AND,
            OR
        }

        private ConditionalOperator(Operators op, string opString)
        {
            this.Op = op;
            this.OpString = opString;
        }

        public override string ToString() => this.OpString;

        internal static ConditionalOperator EQUALS { get; } = new ConditionalOperator(Operators.EQUALS, "=");
        internal static ConditionalOperator NOT_EQUALS { get; } = new ConditionalOperator(Operators.NOT_EQUALS, "<>");
        internal static ConditionalOperator GREATER_THAN { get; } = new ConditionalOperator(Operators.GREATER_THAN, ">");
        internal static ConditionalOperator LESS_THAN { get; } = new ConditionalOperator(Operators.LESS_THAN, "<");
        internal static ConditionalOperator GREATER_THAN_OR_EQUAL { get; } = new ConditionalOperator(Operators.GREATER_THAN_OR_EQUAL, ">=");
        internal static ConditionalOperator LESS_THAN_OR_EQUAL { get; } = new ConditionalOperator(Operators.LESS_THAN_OR_EQUAL, "<=");
        internal static ConditionalOperator AND { get; } = new ConditionalOperator(Operators.AND, "AND");
        internal static ConditionalOperator OR { get; } = new ConditionalOperator(Operators.OR, "OR");

        internal static IEnumerable<ConditionalOperator> ConditionalOperators { get; } = new List<ConditionalOperator>()
        {  EQUALS, NOT_EQUALS, GREATER_THAN, LESS_THAN, GREATER_THAN_OR_EQUAL, LESS_THAN_OR_EQUAL, AND, OR };
    }
}
