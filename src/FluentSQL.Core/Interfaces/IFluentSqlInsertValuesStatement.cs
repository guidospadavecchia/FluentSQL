using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlInsertValuesStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        int Execute();
        Task<int> ExecuteAsync();
    }
}
