using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectTopStatement Top(int rows);
        IFluentSqlSelectDistinctStatement Distinct();
        IFluentSqlSelectFromStatement From(string table);
        IFluentSqlSelectFromStatement From(string table, string tableAlias);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();

        Task<IEnumerable<dynamic>> ToDynamicAsync();
        Task<IEnumerable<T>> ToMappedObjectAsync<T>();
    }
}
