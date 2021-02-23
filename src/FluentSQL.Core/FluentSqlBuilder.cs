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

        /// <inheritdoc />
        public int? Timeout
        {
            get
            {
                return _timeout ?? _globalTimeout;
            }
        }

        /// <inheritdoc />
        public string Query
        {
            get
            {
                return _query;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get
            {
                return _spName;
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IFluentSql SetTimeout(int seconds)
        {
            _timeout = seconds;
            return this;
        }

        #endregion

        #region Transactions

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IFluentSqlSelectStatement Select(params string[] columns)
        {
            _queryParameters = new DynamicParameters();
            _query = $"SELECT {string.Join(", ", columns)}";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectStatement SelectAll()
        {
            _queryParameters = new DynamicParameters();
            _query = "SELECT *";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectDistinctStatement Distinct()
        {
            _query = new Regex("SELECT").Replace(_query, "SELECT DISTINCT", 1);
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectTopStatement Top(int rows)
        {
            _query = new Regex("SELECT").Replace(_query, $"SELECT TOP {rows}", 1);
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectFromStatement From(string table)
        {
            _query = $"{_query} FROM {table}";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectFromStatement From(string table, string tableAlias)
        {
            _query = $"{_query} FROM {table} {tableAlias}";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectFromWithNoLockStatement WithNoLock()
        {
            _query = $"{_query} WITH (NOLOCK)";
            return this;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IFluentSqlSelectJoinOnStatement On(string condition)
        {
            _query = $"{_query} ON {condition}";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectJoinOnStatement On(string condition, Dictionary<string, object> parameters)
        {
            AddQueryParameters(parameters);
            return On(condition);
        }

        /// <inheritdoc />
        public IFluentSqlSelectJoinOnStatement On(string condition, object parameters)
        {
            AddQueryParameters(parameters);
            return On(condition);
        }

        /// <inheritdoc />
        IFluentSqlSelectJoinOnWithNoLockStatement IFluentSqlSelectJoinOnStatement.WithNoLock()
        {
            _query = $"{_query} WITH (NOLOCK)";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectWhereStatement Where(string condition)
        {
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectWhereStatement Where(string condition, Dictionary<string, object> parameters)
        {
            AddQueryParameters(parameters);
            return Where(condition);
        }

        /// <inheritdoc />
        public IFluentSqlSelectWhereStatement Where(string condition, object parameters)
        {
            AddQueryParameters(parameters);
            return Where(condition);
        }

        /// <inheritdoc />
        public IFluentSqlSelectGroupByStatement GroupBy(params string[] columns)
        {
            _query = $"{_query} GROUP BY {string.Join(", ", columns)}";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectGroupByHavingStatement Having(string condition)
        {
            _query = $"{_query} HAVING {condition}";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectGroupByHavingStatement Having(string condition, Dictionary<string, object> parameters)
        {
            AddQueryParameters(parameters);
            return Having(condition);
        }

        /// <inheritdoc />
        public IFluentSqlSelectGroupByHavingStatement Having(string condition, object parameters)
        {
            AddQueryParameters(parameters);
            return Having(condition);
        }

        /// <inheritdoc />
        public IFluentSqlSelectOrderByStatement OrderBy(params string[] columns)
        {
            _query = $"{_query} ORDER BY {string.Join(", ", columns)}";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectOrderByAscendingDescendingStatement Ascending()
        {
            _query = $"{_query} ASC";
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlSelectOrderByAscendingDescendingStatement Descending()
        {
            _query = $"{_query} DESC";
            return this;
        }

        #endregion

        #region Insert

        /// <inheritdoc />
        public IFluentSqlInsertStatement InsertInto(string table)
        {
            _queryParameters = new DynamicParameters();
            _query = $"INSERT INTO {table}";
            return this;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IFluentSqlUpdateStatement Update(string table)
        {
            _queryParameters = new DynamicParameters();
            _query = $"UPDATE {table}";
            return this;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        IFluentSqlNonQueryWhereStatement IFluentSqlUpdateSetStatement.Where(string condition)
        {
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        /// <inheritdoc />
        IFluentSqlNonQueryWhereStatement IFluentSqlUpdateSetStatement.Where(string condition, Dictionary<string, object> parameters)
        {
            AddQueryParameters(parameters);
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        /// <inheritdoc />
        IFluentSqlNonQueryWhereStatement IFluentSqlUpdateSetStatement.Where(string condition, object parameters)
        {
            AddQueryParameters(parameters);
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        #endregion

        #region Delete

        /// <inheritdoc />
        public IFluentSqlDeleteStatement DeleteFrom(string table)
        {
            _queryParameters = new DynamicParameters();
            _query = $"DELETE FROM {table}";
            return this;
        }

        /// <inheritdoc />
        IFluentSqlNonQueryWhereStatement IFluentSqlDeleteStatement.Where(string condition)
        {
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        /// <inheritdoc />
        IFluentSqlNonQueryWhereStatement IFluentSqlDeleteStatement.Where(string condition, Dictionary<string, object> parameters)
        {
            AddQueryParameters(parameters);
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        /// <inheritdoc />
        IFluentSqlNonQueryWhereStatement IFluentSqlDeleteStatement.Where(string condition, object parameters)
        {
            AddQueryParameters(parameters);
            _query = $"{_query} WHERE {condition}";
            return this;
        }

        #endregion

        #region End Methods

        #region Sync

        /// <inheritdoc />
        public IEnumerable<dynamic> ToDynamic()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.Query(_query, _queryParameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public dynamic ToDynamicSingle()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.QueryFirstOrDefault(_query, _queryParameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<T> ToMappedObject<T>()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.Query<T>(_query, _queryParameters, _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public T ToMappedObjectSingle<T>()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection.QueryFirstOrDefault<T>(_query, _queryParameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<IEnumerable<dynamic>> ToDynamicAsync()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryAsync(_query, _queryParameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public async Task<dynamic> ToDynamicSingleAsync()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryFirstOrDefaultAsync(_query, _queryParameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> ToMappedObjectAsync<T>()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryAsync<T>(_query, _queryParameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public async Task<T> ToMappedObjectSingleAsync<T>()
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return await connection.QueryFirstOrDefaultAsync<T>(_query, _queryParameters, _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Adds the specified parameters to the query parameter collection.
        /// </summary>
        /// <param name="parameters">Parameters to add.</param>
        private void AddQueryParameters(object parameters)
        {
            _queryParameters.AddDynamicParams(parameters);
        }

        #endregion

        #endregion

        #region StoredProcedure

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureStatement StoreProcedure(string name)
        {
            _spName = name;
            _spParameters.Clear();
            _spOutputParameters.Clear();
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureParameterStatement WithParameters(Dictionary<string, object> parameters)
        {
            foreach (var parameter in parameters)
            {
                _spParameters.Add(parameter.Key, parameter.Value);
            }
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureParameterStatement WithParameter(string name, object value)
        {
            _spParameters.Add(name, value);
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameters(IEnumerable<OutputParameter> parameters)
        {
            _spOutputParameters.AddRange(parameters);
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(OutputParameter parameter)
        {
            _spOutputParameters.Add(parameter);
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type)
        {
            _spOutputParameters.Add(new OutputParameter(name, type));
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int size)
        {
            _spOutputParameters.Add(new OutputParameter(name, type, size));
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, byte precision, byte scale)
        {
            _spOutputParameters.Add(new OutputParameter(name, type, precision, scale));
            return this;
        }

        /// <inheritdoc />
        public IFluentSqlExecuteStoredProcedureOutputParameterStatement WithOutputParameter(string name, DbType type, int? size = null, byte? precision = null, byte? scale = null)
        {
            _spOutputParameters.Add(new OutputParameter(name, type, size, precision, scale));
            return this;
        }

        #region End Methods

        #region Sync

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public int ExecuteCustomNonQuery(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Execute(sqlQuery, new DynamicParameters(parameters), _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IEnumerable<dynamic> ExecuteCustomQuery(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Query(sqlQuery, new DynamicParameters(parameters), _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public dynamic ExecuteCustomQuerySingle(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.QueryFirstOrDefault(sqlQuery, new DynamicParameters(parameters), _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IEnumerable<T> ExecuteCustomQuery<T>(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.Query<T>(sqlQuery, new DynamicParameters(parameters), _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public T ExecuteCustomQuerySingle<T>(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection.QueryFirstOrDefault<T>(sqlQuery, new DynamicParameters(parameters), _transaction, commandTimeout: Timeout);
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<int> ExecuteCustomNonQueryAsync(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.ExecuteAsync(sqlQuery, new DynamicParameters(parameters), _transaction, Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryAsync(sqlQuery, new DynamicParameters(parameters), _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<dynamic> ExecuteCustomQuerySingleAsync(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryFirstOrDefaultAsync(sqlQuery, new DynamicParameters(parameters), _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<IEnumerable<T>> ExecuteCustomQueryAsync<T>(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryAsync<T>(sqlQuery, new DynamicParameters(parameters), _transaction, commandTimeout: Timeout);
            }
            finally
            {
                if (_transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<T> ExecuteCustomQuerySingleAsync<T>(string sqlQuery, Dictionary<string, object> parameters)
        {
            IDbConnection connection = _transaction?.Connection ?? new SqlConnection(_connectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new DynamicParameters(parameters), _transaction, commandTimeout: Timeout);
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
