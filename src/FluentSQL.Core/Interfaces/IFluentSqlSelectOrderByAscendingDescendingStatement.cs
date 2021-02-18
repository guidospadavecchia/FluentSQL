namespace FluentSQL.Core
{
    public interface IFluentSqlSelectOrderByAscendingDescendingStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }
    }
}