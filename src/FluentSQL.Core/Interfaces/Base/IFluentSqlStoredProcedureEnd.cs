using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlStoredProcedureEnd
    {
        int ExecuteNonQuery();
        IEnumerable<dynamic> ExecuteToDynamic();
        dynamic ExecuteToDynamicSingle();
        IEnumerable<T> ExecuteToMappedObject<T>();
        T ExecuteToMappedObjectSingle<T>();

        Task<int> ExecuteNonQueryAsync();
        Task<IEnumerable<dynamic>> ExecuteToDynamicAsync();
        Task<dynamic> ExecuteToDynamicSingleAsync();
        Task<IEnumerable<T>> ExecuteToMappedObjectAsync<T>();
        Task<T> ExecuteToMappedObjectSingleAsync<T>();
    }
}
