using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectGroupByHavingStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();

        Task<IEnumerable<dynamic>> ToDynamicAsync();
        Task<IEnumerable<T>> ToMappedObjectAsync<T>();
    }
}