using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSql : IDisposable
    {
        int? CommandTimeout { get; }
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
        IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery);
        IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery, object parameters);
        int ExecuteCustomNonQuery(string sqlQuery);
        int ExecuteCustomNonQuery(string sqlQuery, object parameters);

        Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery);
        Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery, object parameters);
        Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery);
        Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery, object parameters);
        Task<int> ExecuteCustomNonQueryAsync(string sqlQuery);
        Task<int> ExecuteCustomNonQueryAsync(string sqlQuery, object parameters);

        void BeginTransaction();
        void CommitTransaction();
    }
}
