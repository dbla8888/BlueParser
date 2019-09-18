using System;

namespace QueryParser
{
    public interface IConditionProvider<T>
    {
        IResult<IWhereCondition> GetConditionFromString(string query);
    }

    public class ConditionProvider<T> : IConditionProvider<T>
    {
        IQueryParser<T> QueryParser { get; }

        public ConditionProvider(IQueryParser<T> queryGrammar)
        {
            this.QueryParser = queryGrammar;
        }

        public IResult<IWhereCondition> GetConditionFromString(string query)
        {
            var parseResult = this.QueryParser.Parse(query);
            return parseResult.IsSuccess ?
                new Result<IWhereCondition>(new WhereCondition(parseResult.Value)) :
                new Result<IWhereCondition>(parseResult.Exception);
        }
    }
}
