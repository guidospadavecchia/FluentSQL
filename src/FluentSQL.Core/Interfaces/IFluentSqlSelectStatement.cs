namespace FluentSQL.Core
{
    public interface IFluentSqlSelectStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectTopStatement Top(int rows);
        IFluentSqlSelectDistinctStatement Distinct();
        IFluentSqlSelectFromStatement From(string table);
        IFluentSqlSelectFromStatement From(string table, string tableAlias);
    }
}
