using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectDistinctStatement : IFluentSqlQueryEnd
    {
        string Query { get; }
        int? Timeout { get; }

        IFluentSqlSelectTopStatement Top(int rows);
        IFluentSqlSelectFromStatement From(string table);
        IFluentSqlSelectFromStatement From(string table, string tableAlias);
    }
}
