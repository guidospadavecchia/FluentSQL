namespace FluentSQL.Core
{
    public interface IFluentSqlSelectFromStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectFromWithNoLockStatement WithNoLock();
        IFluentSqlSelectJoinStatement Join(string table, JoinTypes joinType);
        IFluentSqlSelectJoinStatement Join(string table, string tableAlias, JoinTypes joinType);
        IFluentSqlSelectWhereStatement Where(string condition);
        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}
