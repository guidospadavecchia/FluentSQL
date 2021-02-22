using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectFromWithNoLockStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectJoinStatement Join(string table, JoinTypes joinType);
        IFluentSqlSelectJoinStatement Join(string table, string tableAlias, JoinTypes joinType);
        IFluentSqlSelectWhereStatement Where(string condition);
        IFluentSqlSelectWhereStatement Where(string condition, Dictionary<string, object> parameters);
        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}
