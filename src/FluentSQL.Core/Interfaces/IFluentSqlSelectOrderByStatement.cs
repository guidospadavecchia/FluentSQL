using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectOrderByStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectOrderByAscendingDescendingStatement Ascending();
        IFluentSqlSelectOrderByAscendingDescendingStatement Descending();

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}