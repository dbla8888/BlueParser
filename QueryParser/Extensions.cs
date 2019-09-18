using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QueryParser
{
    public static class Extensions
    {
        public static Func<T1, TResult> Memoize<T1,TResult>(this Func<T1,TResult> f)
        {
            var cache = new ConcurrentDictionary<T1, TResult>();
            return t1 => cache.GetOrAdd(t1, f);
        }

        private static Func<Type, IEnumerable<string>> _GetQueryMembers { get; } = Memoize((Type type) =>
        type.GetPublicProperties()
        .Where(x => x.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(QueryMemberAttribute)))
        .Select(x => x.Name));

        //TODO: Extensions not corectly finding attributes on interface members in .Net Core
        /// <summary>
        /// Returns a collection containing the names of all public properties which are queryable on the data store
        /// (i.e.) have the QueryMember Attribute
        /// </summary>
        public static IEnumerable<string> GetQueryMembers(this Type type) => _GetQueryMembers(type);

        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if(!type.GetTypeInfo().IsInterface)
            {
                return type.GetProperties();
            }

            return (new Type[] { type }).Concat(type.GetInterfaces()).SelectMany(i => i.GetProperties());
        }
    }
}
