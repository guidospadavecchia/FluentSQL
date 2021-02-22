using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    /// <summary>
    /// Fluent API for SQL queries.
    /// </summary>
    public interface IFluentSql : IDisposable
    {
        /// <summary>
        /// Maximum allowed time for a query to finish executing.         
        /// </summary>
        int? Timeout { get; }
        /// <summary>
        /// Indicates whether a transaction is currently active.
        /// </summary>
        bool InTransaction { get; }

        /// <summary>
        /// Sets the maximum time to wait for a query to run before throwing an error.
        /// </summary>
        /// <param name="seconds">Maximum seconds to allow.</param>
        IFluentSql SetTimeout(int seconds);

        /// <summary>
        /// Initializes a SELECT statement with the specified columns.
        /// </summary>
        /// <param name="columns">Columnas a devolver en la sentencia SELECT.</param>
        IFluentSqlSelectStatement Select(params string[] columns);
        /// <summary>
        /// Initializes a SELECT statement to return all columns.
        /// </summary>
        IFluentSqlSelectStatement SelectAll();
        /// <summary>
        /// Initializes an INSERT statement against the specified table.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        IFluentSqlInsertStatement InsertInto(string table);
        /// <summary>
        /// Initializes an UPDATE statement against the specified table.
        /// </summary>
        /// <param name="table">Nombre de la tabla.</param>
        IFluentSqlUpdateStatement Update(string table);
        /// <summary>
        /// Initializes a DELETE statement against the specified table.
        /// </summary>
        /// <param name="table">Table name.</param>
        IFluentSqlDeleteStatement DeleteFrom(string table);
        /// <summary>
        /// Initializes a new Store Procedure execution.
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure.</param>
        IFluentSqlExecuteStoredProcedureStatement StoreProcedure(string spName);

        /// <summary>
        /// Executes a custom query, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A collection of dynamic objects.</returns>
        IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery);
        /// <summary>
        /// Executes a custom query, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A collection of dynamic objects.</returns>
        IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom query, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A collection of dynamic objects.</returns>
        IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a custom query, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A single dynamic object.</returns>
        dynamic ExecuteCustomQuerySingle(string sqlQuery);
        /// <summary>
        /// Executes a custom query, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A single dynamic object.</returns>
        dynamic ExecuteCustomQuerySingle(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom query, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A single dynamic object.</returns>
        dynamic ExecuteCustomQuerySingle(string sqlQuery, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a custom query, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery);
        /// <summary>
        /// Executes a custom query, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom query, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a custom query, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A single <typeparamref name="T"/> typed object, or default.</returns>
        T ExecuteCustomQuerySingle<T>(string sqlQuery);
        /// <summary>
        /// Executes a custom query, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A single <typeparamref name="T"/> typed object, or default.</returns>
        T ExecuteCustomQuerySingle<T>(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom query, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A single <typeparamref name="T"/> typed object, or default.</returns>
        T ExecuteCustomQuerySingle<T>(string sqlQuery, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a custom non-query.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>Affected rows.</returns>
        int ExecuteCustomNonQuery(string sqlQuery);
        /// <summary>
        /// Executes a custom non-query.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>Affected rows.</returns>
        int ExecuteCustomNonQuery(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom non-query.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>Affected rows.</returns>
        int ExecuteCustomNonQuery(string sqlQuery, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with a collection of dynamic objects.</returns>
        Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with a collection of dynamic objects.</returns>
        Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A task with a collection of dynamic objects.</returns>
        Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with a single dynamic object, or default.</returns>
        Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with a single dynamic object.</returns>
        Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A task with a single dynamic object.</returns>
        Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom query asynchronously, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a custom non-query asynchronously.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with the affected rows.</returns>
        Task<int> ExecuteCustomNonQueryAsync(string sqlQuery);
        /// <summary>
        /// Executes a custom non-query asynchronously.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with the affected rows.</returns>
        Task<int> ExecuteCustomNonQueryAsync(string sqlQuery, object parameters);
        /// <summary>
        /// Executes a custom non-query asynchronously.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Collection of key-value pairs containing the parameter names and values.</param>
        /// <returns>A task with the affected rows.</returns>
        Task<int> ExecuteCustomNonQueryAsync(string sqlQuery, Dictionary<string, object> parameters);

        /// <summary>
        /// Starts a new transaction. Changes must be committed by calling CommitTransaction().
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// Commits the changes from the current transaction.
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// Discards all changes made in the current transaction.
        /// </summary>
        void RollbackTransaction();
    }
}
