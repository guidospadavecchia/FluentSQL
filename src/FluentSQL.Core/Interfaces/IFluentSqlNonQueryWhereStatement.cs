using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlNonQueryWhereStatement
    {
        string Query { get; }
        int? Timeout { get; }

        int Execute();
        Task<int> ExecuteAsync();
    }
}
