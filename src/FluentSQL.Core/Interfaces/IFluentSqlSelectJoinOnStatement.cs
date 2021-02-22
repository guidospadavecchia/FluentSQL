using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectJoinOnStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectJoinOnWithNoLockStatement WithNoLock();
        IFluentSqlSelectWhereStatement Where(string condition);
        IFluentSqlSelectWhereStatement Where(string condition, Dictionary<string, object> parameters);
        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}
