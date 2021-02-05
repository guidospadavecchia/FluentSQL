using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectOrderByStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectOrderByAscendingDescendingStatement Ascending();
        IFluentSqlSelectOrderByAscendingDescendingStatement Descending();

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();

        Task<IEnumerable<dynamic>> ToDynamicAsync();
        Task<IEnumerable<T>> ToMappedObjectAsync<T>();
    }
}