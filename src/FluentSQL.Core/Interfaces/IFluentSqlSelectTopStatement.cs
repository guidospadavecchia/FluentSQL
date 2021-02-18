namespace FluentSQL.Core
{
    public interface IFluentSqlSelectTopStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectFromStatement From(string table);
        IFluentSqlSelectFromStatement From(string table, string tableAlias);
    }
}
