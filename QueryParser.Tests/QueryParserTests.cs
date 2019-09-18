using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueryParser.Tests
{
    [TestClass]
    public class QueryParserTests
    {
        public interface IModel
        {
            [QueryMember]
            string Id { get; }         
            string MapKey { get; }
        }

        private class Model : IModel
        {
            [QueryMember]
            public string Id { get; }
            [QueryMember]
            public string MapKey { get; }
        }

        [TestMethod, TestCategory("QueryParser")]
        public void QueryParserTest()
        {
            var query = new QueryParser<Model>();
            var result = query.Parse(
                "!(Id=1234|Id=1.10&!(MapKey<>'asd3f-234'))");

            if(result.IsSuccess)
            {
                var @string = result.ToString();
                ;
            }
            else
            {
                throw result.Exception;
            }
        }
    }
}
