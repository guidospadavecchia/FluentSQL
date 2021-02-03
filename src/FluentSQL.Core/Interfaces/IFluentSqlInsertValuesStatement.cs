namespace FluentSql.Core
{
    public interface IFluentSqlInsertValuesStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        int Execute();
    }
}
