namespace FluentSQL.Core
{
    /// <summary>
    /// Represents a SELECT statement with an ORDER BY ASCENDING/DESCENDING clause.
    /// </summary>
    public interface IFluentSqlSelectOrderByAscendingDescendingStatement : IFluentSqlQueryEnd
    {
        /// <summary>
        /// Returns the string representing the query in it's current state.
        /// </summary>
        string Query { get; }
        /// <summary>
        /// Maximum allowed time for a query to finish executing.         
        /// </summary>
        int? Timeout { get; }
    }
}