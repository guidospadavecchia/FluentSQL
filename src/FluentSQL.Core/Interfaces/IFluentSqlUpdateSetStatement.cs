namespace FluentSql.Core
{
    public interface IFluentSqlUpdateSetStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlNonQueryWhereStatement Where(string condition);

        int Execute();
    }
}
