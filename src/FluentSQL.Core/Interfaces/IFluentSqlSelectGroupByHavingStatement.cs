using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectGroupByHavingStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}