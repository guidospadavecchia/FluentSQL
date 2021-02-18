namespace FluentSQL.Core
{
    public interface IFluentSqlSelectGroupByStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectGroupByHavingStatement Having(string condition);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}