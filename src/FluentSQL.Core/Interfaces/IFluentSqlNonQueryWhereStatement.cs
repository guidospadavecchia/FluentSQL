namespace FluentSql.Core
{
    public interface IFluentSqlNonQueryWhereStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        int Execute();
    }
}
