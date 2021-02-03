using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectWhereStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}
