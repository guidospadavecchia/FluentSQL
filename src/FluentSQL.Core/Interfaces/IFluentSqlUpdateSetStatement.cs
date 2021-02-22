using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlUpdateSetStatement
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlNonQueryWhereStatement Where(string condition, Dictionary<string, object> parameters);

        int Execute();
        Task<int> ExecuteAsync();
    }
}
