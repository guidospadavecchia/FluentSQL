using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlStoredProcedureWithOutputEnd
    {
        StoredProcedureWithOutputResult<int> ExecuteNonQuery();
        StoredProcedureWithOutputResult<IEnumerable<dynamic>> ExecuteToDynamic();
        StoredProcedureWithOutputResult<dynamic> ExecuteToDynamicSingle();
        StoredProcedureWithOutputResult<IEnumerable<T>> ExecuteToMappedObject<T>();
        StoredProcedureWithOutputResult<T> ExecuteToMappedObjectSingle<T>();

        Task<StoredProcedureWithOutputResult<int>> ExecuteNonQueryAsync();
        Task<StoredProcedureWithOutputResult<IEnumerable<dynamic>>> ExecuteToDynamicAsync();
        Task<StoredProcedureWithOutputResult<dynamic>> ExecuteToDynamicSingleAsync();
        Task<StoredProcedureWithOutputResult<IEnumerable<T>>> ExecuteToMappedObjectAsync<T>();
        Task<StoredProcedureWithOutputResult<T>> ExecuteToMappedObjectSingleAsync<T>();
    }
}
