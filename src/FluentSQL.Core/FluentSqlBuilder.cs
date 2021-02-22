using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    /// <summary>
    /// Fluent API for SQL queries.
    /// </summary>
    public sealed class FluentSqlBuilder : IFluentSql, IFluentSqlSelectStatement, IFluentSqlSelectDistinctStatement, IFluentSqlSelectTopStatement, IFluentSqlSelectFromStatement, IFluentSqlSelectFromWithNoLockStatement, IFluentSqlSelectWhereStatement, IFluentSqlSelectOrderByStatement, IFluentSqlSelectOrderByAscendingDescendingStatement, IFluentSqlSelectGroupByStatement, IFluentSqlSelectGroupByHavingStatement, IFluentSqlSelectJoinStatement, IFluentSqlSelectJoinOnStatement, IFluentSqlSelectJoinOnWithNoLockStatement, IFluentSqlInsertStatement, IFluentSqlInsertValuesStatement, IFluentSqlUpdateStatement, IFluentSqlUpdateSetStatement, IFluentSqlNonQueryWhereStatement, IFluentSqlDeleteStatement, IFluentSqlExecuteStoredProcedureStatement, IFluentSqlExecuteStoredProcedureParameterStatement, IFluentSqlExecuteStoredProcedureOutputParameterStatement
    {
        #region Vars

        private bool _disposed;
        private readonly string _connectionString = null;
        private int? _timeout = null;
        private static int? _globalTimeout = null;
        private string _query = null;
        private string _spName = null;
        private IDbConnection _connection = null;
        private IDbTransaction _transaction = null;
        private bool _inTransaction;
        private DynamicParameters _queryParameters;
        private Dictionary<string, object> _spParameters = new Dictionary<string, object>();
        private List<OutputParameter> _spOutputParameters = new List<OutputParameter>();

        #endregion

        #region Properties

        /// <summary>
        /// Maximum allowed time for a query to finish executing.         
        /// </summary>
        public int? Timeout
        {
            get
            {
                return _timeout ?? _globalTimeout;
            }
        }

        /// <summary>
        /// Returns the string representing the query in it's current state.
        /// </summary>
        public string Query
        {
            get
            {
                return _query;
            }
        }

        /// <summary>
        /// Name of the Stored Procedure to be executed.
        /// </summary>
        public string Name
        {
            get
            {
                return _spName;
            }
        }

        /// <summary>
        /// Indicates whether a transaction is currently active.
        /// </summary>
        public bool InTransaction
        {
            get
            {
                return _inTransaction;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="FluentSqlBuilder"/>.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        private FluentSqlBuilder(string connectionString) => _connectionString = connectionString;

        /// <summary>
        /// Configures the connection to the database and returns a <see cref="IFluentSql"/> object.
        /// </summary>
        /// <param name="connectionString">Cadena de conexión al servidor de base de datos.</param>
        /// <returns>Un objeto <see cref="IFluentSql"/> instanciado.</returns>
        public static IFluentSql Connect(string connectionString) => new FluentSqlBuilder(connectionString);

        #endregion

        #region Dispose

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        try
                        {
                            _transaction.Rollback();
                        }
                        finally
                        {
                            _transaction.Dispose();
                            _transaction = null;
                        }
                    }

                    if (_connection != null)
                    {
                        try
                        {
                            if (_connection.State == ConnectionState.Open)
                            {
                                _connection.Close();
                            }
                        }
                        finally
                        {
                            _connection.Dispose();
                            _connection = null;
                        }
                    }

                    _inTransaction = false;
                    _spParameters = null;
                    _spOutputParameters = null;
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        #region Initial Configuration

        /// <summary>
        /// Sets the maximum time to wait for a query to run before throwing an error. 
        /// This applies globally to all instances of <see cref="IFluentSql"/>. 
        /// To set a different timeout for each query, use <see cref="IFluentSql.SetTimeout(int)"/>.
        /// </summary>
        /// <param name="seconds"></param>
        public static void SetGlobalTimeout(int seconds) => _globalTimeout = seconds;

        /// <summary>
        /// Sets the maximum time to wait for a query to run before throwing an error.
        /// </summary>
        /// <param name="seconds">Maximum seconds to allow.</param>
        public IFluentSql SetTimeout(int seconds)
        {
            _timeout = seconds;
            return this;
        }

        #endregion

        #region Transactions

        /// <summary>
        /// Starts a new transaction. Changes must be committed by calling CommitTransaction().
        /// </summary>
        public void BeginTransaction()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }

            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            if (_transaction == null)
            {
                _transaction = _connection.BeginTransaction();
            }

            _inTransaction = true;
        }

        /// <summary>
        /// Commits the changes from the current transaction.
        /// </summary>
        public void CommitTransaction()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                }
            }
            catch (Exception)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }
                throw;
            }
            finally
            {
                _inTransaction = false;

                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        /// <summary>
        /// Discards all changes made in the current transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }
            }
            finally
            {
                _inTransaction = false;

                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        #endregion

        #region CRUD

        #region Select

        /// <summary>
        /// Initializes a SELECT statement with the specified columns.
        /// </summary>
        /// <param name="columns">Columnas a devolver en la sentencia SELECT.</param>
        public IFluentSqlSelectStatement Select(params string[] columns)
        {
            _queryParameters = new DynamicParameters();
            _query = $"SELECT {string.Join(", ", columns)}";
            return this;
        }

        /// <summary>
        /// Initializes a SELECT statement to return all columns.
        /// </summary>
        public IFluentSqlSelectStatement SelectAll()
        {
            _queryParameters = new DynamicParameters();
            _query = "SELECT *";
            return this;
        }

        /// <summary>
        /// Applies the DISTINCT operator to the current query.
        /// </summary>
        public IFluentSqlSelectDistinctStatement Distinct()
        {
            _query = new Regex("SELECT").Replace(_query, "SELECT DISTINCT", 1);
            return this;
        }

        /// <summary>
        /// Applies the TOP operator to the current query.
        /// </summary>
        /// <param name="rows">Number of rows to return.</param>
        public IFluentSqlSelectTopStatement Top(int rows)
        {
            _query = new Regex("SELECT").Replace(_query, $"SELECT TOP {rows}", 1);
            return this;
        }

        /// <summary>
        /// Applies the FROM operator indicating the table to query.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        public IFluentSqlSelectFromStatement From(string table)
        {
            _query = $"{_query} FROM {table}";
            return this;
        }

        /// <summary>
        /// Applies the FROM operator indicating the table to query.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <param name="tableAlias">Alias of the table.</param>
        public IFluentSqlSelectFromStatement From(string table, string tableAlias)
        {
            _query = $"{_query} FROM {table} {tableAlias}";
            return this;
        }

        /// <summary>
        /// Applies the WITH (NOLOCK) modifier, to read records without blocking the table.
        /// </summary>
        public IFluentSqlSelectFromWithNoLockStatement WithNoLock()
        {
            _query = $"{_query} WITH (NOLOCK)";
            return this;
        }

        /// <summary>
        /// Applies the JOIN operator specified by the <paramref name="joinType"/>, against the specified <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table to join with.</param>
        /// <param name="joinType">The type of join to apply.</param>
        public IFluentSqlSelectJoinStatement Join(string table, JoinTypes joinType)
        {
            string joinTypeName = joinType switch
            {
                JoinTypes.Inner => "INNER",
                JoinTypes.Left => "LEFT",
                JoinTypes.Right => "RIGHT",
                JoinTypes.FullOuter => "FULL OUTER",
                _ => "INNER",
            };
            _query = $"{_query} {joinTypeName} JOIN {table}";
            return this;
        }

        /// <summary>
        /// Applies the JOIN operator specified by the <paramref name="joinType"/>, against the specified <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table to join with.</param>
        /// <param name="tableAlias">Alias of the table to join with.</param>
        /// <param name="joinType">The type of join to apply.</param>
        public IFluentSqlSelectJoinStatement Join(string table, string tableAlias, JoinTypes joinType)
        {
            string joinTypeName = joinType switch
            {
                JoinTypes.Inner => "INNER",
                JoinTypes.Left => "LEFT",
                JoinTypes.Right => "RIGHT",
                JoinTypes.FullOuter => "FULL OUTER",
                _ => "INNER",
            };
            _query = $"{_query} {joinTypeName} JOIN {table} {tableAlias}";
            return this;
        }

        /// <summary>
        /// Applies the ON keyword after a JOIN, using the specified condition.
        /// </summary>
        /// <param name="condition">Join condition.</param>
        public IFluentSqlSelectJoinOnStatement On(string condition)
        {
            _query = $"{_query} ON {condition}";
            return this;
        }

        /// <summary>
        /// Applies the WITH (NOLOCK) modifier, to read records without blocking the table.
        /// </summary>
        IFluentSqlSelectJoinOnWithNoLockStatement IFluentSqlSelectJoinOnStatement.WithNoLock()
        {
            _query = $"{_query} WITH (NOLOCK)";
            return this;
        }

        /// <summary>
        /// Applies the WHERE operator using the specified condition.
        /// </summary>
        /// <param name="condition">Filter condition.</param>
        public IFluentSqlSelectWhereStatement Where(string condition)
        {
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        /// <summary>
        /// Applies the GROUP BY operator for the specified columns.
        /// </summary>
        /// <param name="columns">Columns to group by.</param>
        public IFluentSqlSelectGroupByStatement GroupBy(params string[] columns)
        {
            _query = $"{_query} GROUP BY {string.Join(", ", columns)}";
            return this;
        }

        /// <summary>
        /// Applies the HAVING keyword after a GROUP BY, using the specified condition.
        /// </summary>
        /// <param name="condition">Filter GROUP BY condition.</param>
        public IFluentSqlSelectGroupByHavingStatement Having(string condition)
        {
            _query = $"{_query} HAVING {condition}";
            return this;
        }

        /// <summary>
        /// Applies the ORDER BY operator, sorting the results by the specified columns.
        /// </summary>
        /// <param name="columns">Columnas a ordenar.</param>
        public IFluentSqlSelectOrderByStatement OrderBy(params string[] columns)
        {
            _query = $"{_query} ORDER BY {string.Join(", ", columns)}";
            return this;
        }

        /// <summary>
        /// Indicates an ascending sort.
        /// </summary>
        public IFluentSqlSelectOrderByAscendingDescendingStatement Ascending()
        {
            _query = $"{_query} ASC";
            return this;
        }

        /// <summary>
        /// Indicates a descending sort.
        /// </summary>
        public IFluentSqlSelectOrderByAscendingDescendingStatement Descending()
        {
            _query = $"{_query} DESC";
            return this;
        }

        #endregion

        #region Insert

        /// <summary>
        /// Initializes an INSERT statement against the specified table.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        public IFluentSqlInsertStatement InsertInto(string table)
        {
            _queryParameters = new DynamicParameters();
            _query = $"INSERT INTO {table}";
            return this;
        }

        /// <summary>
        /// Indicates the columns and values to insert.
        /// </summary>
        /// <param name="values">Collection of key-value pairs containing parameter names and values to be inserted.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> is null or empty.</exception>
        public IFluentSqlInsertValuesStatement Values(Dictionary<string, object> values)
        {
            if (values == null || values.Count == 0)
            {
                throw new ArgumentException("Debe ejecutar la consulta con al menos un valor");
            }

            _query = $"{_query} ({string.Join(", ", values.Select(x => x.Key))})";

            string insertValues = string.Empty;
            foreach (var value in values)
            {
                string valueParameter = value.Key.StartsWith("@") ? value.Key : $"@{value.Key}";
                insertValues += $"{valueParameter}, ";
            }
            insertValues = insertValues.Remove(insertValues.Length - 2);

            AddQueryParameters(values);
            _query = $" {_query} VALUES ({insertValues})";
            return this;
        }

        #endregion

        #region Update

        /// <summary>
        /// Initializes an UPDATE statement against the specified table.
        /// </summary>
        /// <param name="table">Nombre de la tabla.</param>
        public IFluentSqlUpdateStatement Update(string table)
        {
            _queryParameters = new DynamicParameters();
            _query = $"UPDATE {table}";
            return this;
        }

        /// <summary>
        /// Indicates the assignments to be done by the UPDATE statement using a collection of key-value pairs.
        /// </summary>
        /// <param name="assignments">Collection of new values. Each <see cref="KeyValuePair{string, object}"/> specifies the name of the column and its new value respectively.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="assignments"/> is null or empty.</exception>
        public IFluentSqlUpdateSetStatement Set(Dictionary<string, object> assignments)
        {
            if (assignments == null || assignments.Count == 0)
            {
                throw new ArgumentException("Query must have at least one assignment");
            }

            string values = string.Empty;
            foreach (var assignment in assignments)
            {
                string nameParameter = assignment.Key.StartsWith("@") ? assignment.Key.Remove(0, 1) : assignment.Key;
                string valueParameter = assignment.Key.StartsWith("@") ? assignment.Key : $"@{assignment.Key}";
                values += $"{nameParameter} = {valueParameter}, ";
            }
            values = values.Remove(values.Length - 2);

            AddQueryParameters(assignments);
            _query = $"{_query} SET {values}";
            return this;
        }

        /// <summary>
        /// Applies the WHERE operator using the specified condition.
        /// </summary>
        /// <param name="condition">Filter condition.</param>
        /// <param name="parameters">Collection of key-value pairs containing parameter names and values.</param>
        IFluentSqlNonQueryWhereStatement IFluentSqlUpdateSetStatement.Where(string condition, Dictionary<string, object> parameters)
        {
            AddQueryParameters(parameters);
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Initializes a DELETE statement against the specified table.
        /// </summary>
        /// <param name="table">Table name.</param>
        /// <returns></returns>
        public IFluentSqlDeleteStatement DeleteFrom(string table)
        {
            _queryParameters = new DynamicParameters();
            _query = $"DELETE FROM {table}";
            return this;
        }

        /// <summary>
        /// Applies the WHERE operator using the specified condition.
        /// </summary>
        /// <param name="condition">Filter condition.</param>
        /// <param name="parameters">Collection of key-value pairs containing parameter names and values.</param>
        IFluentSqlNonQueryWhereStatement IFluentSqlDeleteStatement.Where(string condition, Dictionary<string, object> parameters)
        {
            AddQueryParameters(parameters);
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        #endregion

        #region End Methods

        #region Sync

        /// <summary>
        /// Executes the query and returns a collection of dynamic objects.
        /// </summary>
        /// <returns>Collection of dynamic objects.</returns>
        public IEnumerable<dynamic> ToDynamic()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.Query(_query, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the query and returns a single dynamic object.
        /// </summary>
        /// <returns>Single dynamic object or default.</returns>
        public dynamic ToDynamicSingle()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.QueryFirstOrDefault(_query, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the query and maps the result to a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <returns>Collection of <typeparamref name="T"/> typed objects.</returns>
        public IEnumerable<T> ToMappedObject<T>()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.Query<T>(_query, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the query and maps the result to a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <returns>A single <typeparamref name="T"/> typed object or default.</returns>
        public T ToMappedObjectSingle<T>()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.QueryFirstOrDefault<T>(_query, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <returns>Affected rows.</returns>
        public int Execute()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.Execute(_query, _queryParameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Executes the query asynchronously and returns a collection of dynamic objects.
        /// </summary>
        /// <returns>A task with a collection of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> ToDynamicAsync()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryAsync(_query, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the query asynchronously and returns a single dynamic object.
        /// </summary>
        /// <returns>A task with a single dynamic object, or default.</returns>
        public async Task<dynamic> ToDynamicSingleAsync()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryFirstOrDefaultAsync(_query, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the query asynchronously and maps the result to a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects.</returns>
        public async Task<IEnumerable<T>> ToMappedObjectAsync<T>()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryAsync<T>(_query, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the query asynchronously and maps the result to a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        public async Task<T> ToMappedObjectSingleAsync<T>()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryFirstOrDefaultAsync<T>(_query, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>A task with the affected rows.</returns>
        public async Task<int> ExecuteAsync()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.ExecuteAsync(_query, _queryParameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion

        #endregion

        #region Private

        /// <summary>
        /// Adds the specified parameters to the query parameter collection.
        /// </summary>
        /// <param name="parameters">Parameters to add.</param>
        private void AddQueryParameters(Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    string nameParameter = parameter.Key.StartsWith("@") ? parameter.Key : $"@{parameter.Key}";
                    _queryParameters.Add(nameParameter, parameter.Value);
                }
            }
        }

        #endregion

        #endregion

        #region StoredProcedure

        /// <summary>
        /// Initializes a new Store Procedure execution.
        /// </summary>
        /// <param name="name">Name of the Stored Procedure.</param>
        public IFluentSqlExecuteStoredProcedureStatement StoreProcedure(string name)
        {
            _spName = name;
            _spParameters.Clear();
            _spOutputParameters.Clear();
            return this;
        }

        /// <summary>
        /// Adds the specified <paramref name="parameters"/> to the collection.
        /// </summary>
        /// <param name="parameters">Collection of key-value pairs, with the parameter name and value respectively.</param>
        public IFluentSqlExecuteStoredProcedureParameterStatement WithParameters(Dictionary<string, object> parameters)
        {
            foreach (var parameter in parameters)
            {
                _spParameters.Add(parameter.Key, parameter.Value);
            }
            return this;
        }

        /// <summary>
        /// Adds the specified parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the parameter..</param>
        /// <param name="value">Value of the parameter.</param>
        public IFluentSqlExecuteStoredProcedureParameterStatement WithParameter(string name, object value)
        {
            _spParameters.Add(name, value);
            return this;
        }

        /// <summary>
        /// Adds the specified output <paramref name="parameters"/> to the collection.
        /// </summary>
        /// <param name="parameters">Collection of <see cref="OutputParameter"/> objects.</param>
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameters(IEnumerable<OutputParameter> parameters)
        {
            _spOutputParameters.AddRange(parameters);
            return this;
        }

        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="parameter">An <see cref="OutputParameter"/> instance.</param>
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(OutputParameter parameter)
        {
            _spOutputParameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type)
        {
            _spOutputParameters.Add(new OutputParameter(name, type));
            return this;
        }

        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="size">Size of the output parameter. Must be specified if <paramref name="type"/> is any variant of <see cref="DbType.String"/> or <see cref="DbType.Binary"/>.</param>
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int size)
        {
            _spOutputParameters.Add(new OutputParameter(name, type, size));
            return this;
        }

        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="precision">Numeric precision of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        /// <param name="scale">Numeric scale of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, byte precision, byte scale)
        {
            _spOutputParameters.Add(new OutputParameter(name, type, precision, scale));
            return this;
        }

        /// <summary>
        /// Adds the specified output parameter to the collection.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="size">Size of the output parameter. This parameter must be specified if <paramref name="type"/> is any variant of <see cref="DbType.String"/> or <see cref="DbType.Binary"/>.</param>
        /// <param name="precision">Numeric precision of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        /// <param name="scale">Escala numérica del parámetro. Must be specified when the parameter is any kind of number with decimals.</param>
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int? size = null, byte? precision = null, byte? scale = null)
        {
            _spOutputParameters.Add(new OutputParameter(name, type, size, precision, scale));
            return this;
        }

        #region End Methods

        #region Sync

        /// <summary>
        /// Executes the Stored Procedure as a non-query, and returns the affected rows.
        /// </summary>
        /// <returns>Affected rows.</returns>
        public int ExecuteNonQuery()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.Execute(_spName, parameters, _transaction, Timeout, CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a collection of dynamic objects.
        /// </summary>
        /// <returns>A collection of dynamic objects.</returns>
        public IEnumerable<dynamic> ExecuteToDynamic()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.Query(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a single dynamic object.
        /// </summary>
        /// <returns>A single dynamic object or default.</returns>
        public dynamic ExecuteToDynamicSingle()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.QueryFirstOrDefault(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>A collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        public IEnumerable<T> ExecuteToMappedObject<T>()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.Query<T>(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>A single <typeparamref name="T"/> typed object, or default.</returns>
        public T ExecuteToMappedObjectSingle<T>()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.QueryFirstOrDefault<T>(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a non-query, returning the affected rows and the output parameters.
        /// </summary>
        /// <returns>An object containing the affected rows and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<int> IFluentSqlStoredProcedureWithOutputEnd.ExecuteNonQuery()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);            
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                int affectedRows = connection.Execute(_spName, parameters, _transaction, Timeout, CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<int>(affectedRows, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a collection of dynamic objects and a collection of output parameters.
        /// </summary>
        /// <returns>An object containing a collection of dynamic objects and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<IEnumerable<dynamic>> IFluentSqlStoredProcedureWithOutputEnd.ExecuteToDynamic()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                IEnumerable<dynamic> result = connection.Query(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<IEnumerable<dynamic>>(result, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a single dynamic object and a collection of output parameters.
        /// </summary>
        /// <returns>An object containing a single dynamic object and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<dynamic> IFluentSqlStoredProcedureWithOutputEnd.ExecuteToDynamicSingle()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                var result = connection.QueryFirstOrDefault(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<dynamic>(result, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a collection of <typeparamref name="T"/> typed objects and a collection of output parameters.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>An object containing a collection of <typeparamref name="T"/> typed objects representing a row each, and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<IEnumerable<T>> IFluentSqlStoredProcedureWithOutputEnd.ExecuteToMappedObject<T>()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                IEnumerable<T> result = connection.Query<T>(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<IEnumerable<T>>(result, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a single <typeparamref name="T"/> typed object and a collection of output parameters.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>An object containing a single <typeparamref name="T"/> typed object, and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<T> IFluentSqlStoredProcedureWithOutputEnd.ExecuteToMappedObjectSingle<T>()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                var result = connection.QueryFirstOrDefault<T>(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<T>(result, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Executes the Stored Procedure as a non-query asynchronously, and returns the affected rows.
        /// </summary>
        /// <returns>A task with the affected rows.</returns>
        public async Task<int> ExecuteNonQueryAsync()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.ExecuteAsync(_spName, parameters, _transaction, Timeout, CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a collection of dynamic objects.
        /// </summary>
        /// <returns>A task with a collection of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> ExecuteToDynamicAsync()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryAsync(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a single dynamic object.
        /// </summary>
        /// <returns>A task with a single dynamic object, or default.</returns>
        public async Task<dynamic> ExecuteToDynamicSingleAsync()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryFirstOrDefaultAsync(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        public async Task<IEnumerable<T>> ExecuteToMappedObjectAsync<T>()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryAsync<T>(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        public async Task<T> ExecuteToMappedObjectSingleAsync<T>()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryFirstOrDefaultAsync<T>(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a non-query asynchronously, returning the affected rows and the output parameters.
        /// </summary>
        /// <returns>A task with an object containing the affected rows and a collection of key-value pairs with the output parameters.</returns>
        async Task<StoredProcedureWithOutputResult<int>> IFluentSqlStoredProcedureWithOutputEnd.ExecuteNonQueryAsync()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                int affectedRows = await connection.ExecuteAsync(_spName, parameters, _transaction, Timeout, CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<int>(affectedRows, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a collection of dynamic objects and a collection of output parameters.
        /// </summary>
        /// <returns>A task with an object containing a collection of dynamic objects and a collection of key-value pairs with the output parameters.</returns>
        async Task<StoredProcedureWithOutputResult<IEnumerable<dynamic>>> IFluentSqlStoredProcedureWithOutputEnd.ExecuteToDynamicAsync()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                IEnumerable<dynamic> result = await connection.QueryAsync(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<IEnumerable<dynamic>>(result, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a single dynamic object and a collection of output parameters.
        /// </summary>
        /// <returns>A task with an object containing a single dynamic object and a collection of key-value pairs with the output parameters.</returns>
        async Task<StoredProcedureWithOutputResult<dynamic>> IFluentSqlStoredProcedureWithOutputEnd.ExecuteToDynamicSingleAsync()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                var result = await connection.QueryFirstOrDefaultAsync(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<dynamic>(result, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects and a collection of output parameters.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>An object containing a collection of <typeparamref name="T"/> typed objects representing a row each, and a collection of key-value pairs with the output parameters.</returns>
        async Task<StoredProcedureWithOutputResult<IEnumerable<T>>> IFluentSqlStoredProcedureWithOutputEnd.ExecuteToMappedObjectAsync<T>()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                IEnumerable<T> result = await connection.QueryAsync<T>(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<IEnumerable<T>>(result, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a single <typeparamref name="T"/> typed object and a collection of output parameters.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>An object containing a single <typeparamref name="T"/> typed object, and a collection of key-value pairs with the output parameters.</returns>
        async Task<StoredProcedureWithOutputResult<T>> IFluentSqlStoredProcedureWithOutputEnd.ExecuteToMappedObjectSingleAsync<T>()
        {
            var parameters = CreateDynamicParameters();
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                T result = await connection.QueryFirstOrDefaultAsync<T>(_spName, parameters, _transaction, commandTimeout: Timeout, commandType: CommandType.StoredProcedure);
                return new StoredProcedureWithOutputResult<T>(result, GetOutputParameters(parameters));
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion

        #endregion

        #region Private

        /// <summary>
        /// Creats a <see cref="DynamicParameters"/> collection including all parameters set in query building.
        /// </summary>
        /// <returns>A <see cref="DynamicParameters"/> collection.</returns>
        private DynamicParameters CreateDynamicParameters()
        {
            var parameters = new DynamicParameters();
            foreach (var parameter in _spParameters)
            {
                string nameParameter = parameter.Key.StartsWith("@") ? parameter.Key : $"@{parameter.Key}";
                parameters.Add(nameParameter, parameter.Value);
            }
            foreach (OutputParameter outputParameter in _spOutputParameters)
            {
                string outputParameterName = outputParameter.Name.StartsWith("@") ? outputParameter.Name : $"@{outputParameter.Name}";
                parameters.Add(outputParameterName, null, outputParameter.Type, ParameterDirection.Output, outputParameter.Size, outputParameter.Precision, outputParameter.Scale);
            }

            return parameters;
        }

        /// <summary>
        /// Returns a collection of key-value pairs containing the output parameters and their values.
        /// </summary>
        /// <param name="parameters">The <see cref="DynamicParameters"/> collection.</param>
        /// <returns>A collection of output parameters.</returns>
        private Dictionary<string, object> GetOutputParameters(DynamicParameters parameters)
        {
            Dictionary<string, object> outputParameters = new Dictionary<string, object>();
            foreach (OutputParameter outputParameter in _spOutputParameters)
            {
                if (parameters.ParameterNames.Any(p => p.Trim() == outputParameter.Name.Trim()))
                {
                    object value = parameters.Get<object>(outputParameter.Name);
                    outputParameters.Add(outputParameter.Name, value);
                }
            }

            return outputParameters;
        }

        #endregion

        #endregion

        #region Customs Querys

        #region Sync

        /// <summary>
        /// Executes a custom non-query.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>Affected rows.</returns>
        public int ExecuteCustomNonQuery(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Execute(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom non-query.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>Affected rows.</returns>
        public int ExecuteCustomNonQuery(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Execute(sqlQuery, parameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A collection of dynamic objects.</returns>
        public IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Query(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A collection of dynamic objects.</returns>
        public IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Query(sqlQuery, parameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A single dynamic object.</returns>
        public dynamic ExecuteCustomQuerySingle(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.QueryFirstOrDefault(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A single dynamic object.</returns>
        public dynamic ExecuteCustomQuerySingle(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.QueryFirstOrDefault(sqlQuery, parameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        public IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Query<T>(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        public IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Query<T>(sqlQuery, parameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A single <typeparamref name="T"/> typed object, or default.</returns>
        public T ExecuteCustomQuerySingle<T>(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.QueryFirstOrDefault<T>(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A single <typeparamref name="T"/> typed object, or default.</returns>
        public T ExecuteCustomQuerySingle<T>(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.QueryFirstOrDefault<T>(sqlQuery, parameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Executes a custom non-query asynchronously.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with the affected rows.</returns>
        public async Task<int> ExecuteCustomNonQueryAsync(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.ExecuteAsync(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom non-query asynchronously.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with the affected rows.</returns>
        public async Task<int> ExecuteCustomNonQueryAsync(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.ExecuteAsync(sqlQuery, parameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with a collection of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryAsync(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with a collection of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryAsync(sqlQuery, parameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query asynchronously, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with a single dynamic object, or default.</returns>
        public async Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryFirstOrDefaultAsync(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query asynchronously, and returns a single dynamic object.
        /// </summary>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with a single dynamic object.</returns>
        public async Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryFirstOrDefaultAsync(sqlQuery, parameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        public async Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryAsync<T>(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        public async Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryAsync<T>(sqlQuery, parameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query asynchronously, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        public async Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, transaction: _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a custom query asynchronously, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <param name="sqlQuery">Custom SQL query to execute.</param>
        /// <param name="parameters">Object with properties matching the ones specified inside <paramref name="sqlQuery"/>.</param>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        public async Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery, object parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, parameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
