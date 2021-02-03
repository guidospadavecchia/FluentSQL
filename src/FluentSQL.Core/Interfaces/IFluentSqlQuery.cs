using System;
using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlQuery : IDisposable
    {
        int? CommandTimeout { get; }

        IFluentSqlQuery SetTimeout(int seconds);

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
    }
}
