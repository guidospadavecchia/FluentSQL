using System.Collections.Generic;
using System.Data;

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

        int ExecuteNonQuery(out Dictionary<string, object> outputParameters);
        IEnumerable<dynamic> ExecuteToDynamic(out Dictionary<string, object> outputParameters);
        IEnumerable<T> ExecuteToMappedObject<T>(out Dictionary<string, object> outputParameters);
    }
}
