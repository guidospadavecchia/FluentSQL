using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectOrderByAscendingDescendingStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();

        Task<IEnumerable<dynamic>> ToDynamicAsync();
        Task<IEnumerable<T>> ToMappedObjectAsync<T>();
    }
}