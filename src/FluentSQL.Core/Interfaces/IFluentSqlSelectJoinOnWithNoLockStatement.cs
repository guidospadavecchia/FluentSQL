using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectJoinOnWithNoLockStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectWhereStatement Where(string condition);
        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}
