using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectGroupByStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectGroupByHavingStatement Having(string condition);
        IFluentSqlSelectGroupByHavingStatement Having(string condition, Dictionary<string, object> parameters);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}