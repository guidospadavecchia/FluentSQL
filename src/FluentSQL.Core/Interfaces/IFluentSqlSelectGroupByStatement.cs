using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectGroupByStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectGroupByHavingStatement Having(string condition);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}