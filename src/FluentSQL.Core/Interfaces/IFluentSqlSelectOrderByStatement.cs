using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectOrderByStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectOrderByAscendingDescendingStatement Ascending();
        IFluentSqlSelectOrderByAscendingDescendingStatement Descending();
    }
}