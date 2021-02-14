using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlUpdateSetStatement
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlNonQueryWhereStatement Where(string condition);

        int Execute();
        Task<int> ExecuteAsync();
    }
}
