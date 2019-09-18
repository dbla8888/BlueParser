using System;

namespace QueryParser
{
    /// <summary>
    /// This attribute is used to determine which properties on a type
    /// are valid within a query expression
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryMemberAttribute : Attribute { }
}
