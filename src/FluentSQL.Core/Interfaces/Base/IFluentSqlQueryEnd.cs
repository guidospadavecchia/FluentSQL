using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlQueryEnd
    {
        IEnumerable<dynamic> ToDynamic();
        Task<IEnumerable<dynamic>> ToDynamicAsync();
        IEnumerable<T> ToMappedObject<T>();
        Task<IEnumerable<T>> ToMappedObjectAsync<T>();
        dynamic ToDynamicSingle();
        Task<dynamic> ToDynamicSingleAsync();
        T ToMappedObjectSingle<T>();
        Task<T> ToMappedObjectSingleAsync<T>();
    }
}
