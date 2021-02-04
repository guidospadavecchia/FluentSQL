using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectWhereStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();

        Task<IEnumerable<dynamic>> ToDynamicAsync();
        Task<IEnumerable<T>> ToMappedObjectAsync<T>();
    }
}
