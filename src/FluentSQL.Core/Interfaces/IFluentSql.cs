using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSql : IDisposable
    {
        /// <summary>
        /// Maximum allowed time for a query to finish executing.         
        /// </summary>
        int? Timeout { get; }
        bool InTransaction { get; }

        IFluentSql SetTimeout(int seconds);

        IFluentSqlSelectStatement Select(params string[] columns);
        IFluentSqlSelectStatement SelectAll();
        IFluentSqlInsertStatement InsertInto(string table);
        IFluentSqlUpdateStatement Update(string table);
        IFluentSqlDeleteStatement DeleteFrom(string table);
        IFluentSqlExecuteStoredProcedureStatement StoreProcedure(string spName);

        IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery);
        IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery, object parameters);
        IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery, Dictionary<string, object> parameters);
        dynamic ExecuteCustomQuerySingle(string sqlQuery);
        dynamic ExecuteCustomQuerySingle(string sqlQuery, object parameters);
        dynamic ExecuteCustomQuerySingle(string sqlQuery, Dictionary<string, object> parameters);
        IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery);
        IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery, object parameters);
        IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery, Dictionary<string, object> parameters);
        T ExecuteCustomQuerySingle<T>(string sqlQuery);
        T ExecuteCustomQuerySingle<T>(string sqlQuery, object parameters);
        T ExecuteCustomQuerySingle<T>(string sqlQuery, Dictionary<string, object> parameters);
        int ExecuteCustomNonQuery(string sqlQuery);
        int ExecuteCustomNonQuery(string sqlQuery, object parameters);
        int ExecuteCustomNonQuery(string sqlQuery, Dictionary<string, object> parameters);

        Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery);
        Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery, object parameters);
        Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery, Dictionary<string, object> parameters);
        Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery);
        Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery, object parameters);
        Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery, Dictionary<string, object> parameters);
        Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery);
        Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery, object parameters);
        Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery, Dictionary<string, object> parameters);
        Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery);
        Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery, object parameters);
        Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery, Dictionary<string, object> parameters);
        Task<int> ExecuteCustomNonQueryAsync(string sqlQuery);
        Task<int> ExecuteCustomNonQueryAsync(string sqlQuery, object parameters);
        Task<int> ExecuteCustomNonQueryAsync(string sqlQuery, Dictionary<string, object> parameters);

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
