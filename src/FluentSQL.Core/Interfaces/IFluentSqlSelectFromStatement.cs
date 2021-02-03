using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectFromStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectFromWithNoLockStatement WithNoLock();
        IFluentSqlSelectJoinStatement Join(string table, JoinTypes joinType);
        IFluentSqlSelectJoinStatement Join(string table, string tableAlias, JoinTypes joinType);
        IFluentSqlSelectWhereStatement Where(string condition);
        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}
