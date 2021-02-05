namespace FluentSQL.Core
{
    public interface IFluentSqlInsertColumnsStatement
    {
        IFluentSqlInsertValuesStatement Values(params object[] values);
    }
}
