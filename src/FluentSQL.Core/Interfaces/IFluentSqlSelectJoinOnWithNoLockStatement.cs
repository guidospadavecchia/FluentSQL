namespace FluentSQL.Core
{
    public interface IFluentSqlSelectJoinOnWithNoLockStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectWhereStatement Where(string condition);
        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}
