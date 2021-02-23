namespace FluentSQL.Core
{
    /// <summary>
    /// Represents a SELECT statement.
    /// </summary>
    public interface IFluentSqlSelectStatement : IFluentSqlQueryEnd
    {
        /// <summary>
        /// Returns the string representing the query in it's current state.
        /// </summary>
        string Query { get; }
        /// <summary>
        /// Maximum allowed time for a query to finish executing.         
        /// </summary>
        int? Timeout { get; }

        /// <summary>
        /// Applies the TOP operator to the current query.
        /// </summary>
        /// <param name="rows">Number of rows to return.</param>
        IFluentSqlSelectTopStatement Top(int rows);
        /// <summary>
        /// Applies the DISTINCT operator to the current query.
        /// </summary>
        IFluentSqlSelectDistinctStatement Distinct();
        /// <summary>
        /// Applies the FROM operator indicating the table to query.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        IFluentSqlSelectFromStatement From(string table);
        /// <summary>
        /// Applies the FROM operator indicating the table to query.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <param name="tableAlias">Alias of the table.</param>
        IFluentSqlSelectFromStatement From(string table, string tableAlias);
    }
}
