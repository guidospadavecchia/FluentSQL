using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FluentSql.Core
{
    public interface IFluentSqlExecuteStoredProcedureParameterStatement
    {
        string Name { get; }
        int? CommandTimeout { get; }

        IFluentSqlExecuteStoredProcedureParameterStatement WithParameters(Dictionary<string, object> parameters);
        IFluentSqlExecuteStoredProcedureParameterStatement WithParameter(string name, object value);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameters(IEnumerable<OutputParameter> parameters);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(OutputParameter parameter);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int size);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, byte precision, byte scale);
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int? size = null, byte? precision = null, byte? scale = null);

        int ExecuteNonQuery();
        IEnumerable<dynamic> ExecuteToDynamic();
        IEnumerable<T> ExecuteToMappedObject<T>();

        Task<int> ExecuteNonQueryAsync();
        Task<IEnumerable<dynamic>> ExecuteToDynamicAsync();
        Task<IEnumerable<T>> ExecuteToMappedObjectAsync<T>();
    }
}
