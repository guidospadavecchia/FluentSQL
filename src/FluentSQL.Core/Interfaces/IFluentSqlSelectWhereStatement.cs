namespace FluentSQL.Core
{
    public interface IFluentSqlSelectWhereStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}
