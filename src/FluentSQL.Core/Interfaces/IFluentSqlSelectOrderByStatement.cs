namespace FluentSQL.Core
{
    public interface IFluentSqlSelectOrderByStatement : IFluentSqlQueryEnd
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
        /// Indicates an ascending sort.
        /// </summary>
        IFluentSqlSelectOrderByAscendingDescendingStatement Ascending();
        /// <summary>
        /// Indicates a descending sort.
        /// </summary>
        IFluentSqlSelectOrderByAscendingDescendingStatement Descending();
    }
}