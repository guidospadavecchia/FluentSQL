namespace FluentSQL.Core
{
    public interface IFluentSqlSelectGroupByHavingStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}