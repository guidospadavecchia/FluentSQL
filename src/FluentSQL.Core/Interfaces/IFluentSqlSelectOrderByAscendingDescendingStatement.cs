using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectOrderByAscendingDescendingStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}