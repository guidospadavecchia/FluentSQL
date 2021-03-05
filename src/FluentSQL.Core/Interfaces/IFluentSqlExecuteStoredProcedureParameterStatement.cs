using System.Collections.Generic;
using System.Data;

namespace FluentSQL.Core
{
    /// <summary>
    /// Represents a Stored Procedure execution with parameters.
    /// </summary>
    public interface IFluentSqlExecuteStoredProcedureParameterStatement : IFluentSqlStoredProcedureEnd
    {
        /// <summary>
        /// Name of the Stored Procedure to be executed.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Maximum allowed time for a query to finish executing.         
        /// </summary>
        int? Timeout { get; }

        /// <summary>
        /// Adds the specified <paramref name="parameters"/> to the collection.
        /// </summary>
        /// <param name="parameters">Collection of key-value pairs, with the parameter name and value respectively.</param>
        IFluentSqlExecuteStoredProcedureParameterStatement WithParameters(Dictionary<string, object> parameters);
        /// <summary>
        /// Adds the specified <paramref name="parametersObject"/> to the collection.
        /// </summary>
        /// <param name="parametersObject">Object with properties containing parameter names and values.</param>
        IFluentSqlExecuteStoredProcedureParameterStatement WithParameters(object parametersObject);
        /// <summary>
        /// Adds the specified parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the parameter..</param>
        /// <param name="value">Value of the parameter.</param>
        IFluentSqlExecuteStoredProcedureParameterStatement WithParameter(string name, object value);
        /// <summary>
        /// Adds the specified output <paramref name="parameters"/> to the collection.
        /// </summary>
        /// <param name="parameters">Collection of <see cref="OutputParameter"/> objects.</param>
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameters(IEnumerable<OutputParameter> parameters);
        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="parameter">An <see cref="OutputParameter"/> instance.</param>
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(OutputParameter parameter);
        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type);
        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="size">Size of the output parameter. Must be specified if <paramref name="type"/> is any variant of <see cref="DbType.String"/> or <see cref="DbType.Binary"/>.</param>
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int size);
        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="precision">Numeric precision of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        /// <param name="scale">Numeric scale of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, byte precision, byte scale);
        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="size">Size of the output parameter. This parameter must be specified if <paramref name="type"/> is any variant of <see cref="DbType.String"/> or <see cref="DbType.Binary"/>.</param>
        /// <param name="precision">Numeric precision of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        /// <param name="scale">Escala numérica del parámetro. Must be specified when the parameter is any kind of number with decimals.</param>
        IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int? size = null, byte? precision = null, byte? scale = null);
    }
}
