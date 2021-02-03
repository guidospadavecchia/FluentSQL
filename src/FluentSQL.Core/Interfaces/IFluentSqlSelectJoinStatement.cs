namespace FluentSql.Core
{
    public interface IFluentSqlSelectJoinStatement
    {
        IFluentSqlSelectJoinOnStatement On(string condition);
    }
}
