namespace FluentSql.Core
{
    public interface IFluentSqlInsertStatement
    {
        IFluentSqlInsertColumnsStatement Columns(params string[] columns);
        IFluentSqlInsertValuesStatement Values(params object[] values);
    }
}
