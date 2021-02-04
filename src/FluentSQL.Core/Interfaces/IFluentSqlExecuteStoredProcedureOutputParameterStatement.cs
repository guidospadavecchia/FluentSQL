using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FluentSql.Core
{
    public interface IFluentSqlExecuteStoredProcedureOutputParameterStatement
    {
        string Name { get; }
        int? CommandTimeout { get; }

        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameters(IEnumerable<OutputParameter> parameters);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(OutputParameter parameter);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int size);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, byte precision, byte scale);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int? size = null, byte? precision = null, byte? scale = null);

        StoredProcedureWithOutputResult<int> ExecuteNonQuery();
        StoredProcedureWithOutputResult<IEnumerable<dynamic>> ExecuteToDynamic();
        StoredProcedureWithOutputResult<IEnumerable<T>> ExecuteToMappedObject<T>();

        Task<StoredProcedureWithOutputResult<int>> ExecuteNonQueryAsync();
        Task<StoredProcedureWithOutputResult<IEnumerable<dynamic>>> ExecuteToDynamicAsync();
        Task<StoredProcedureWithOutputResult<IEnumerable<T>>> ExecuteToMappedObjectAsync<T>();
    }
}
