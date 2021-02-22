namespace FluentSQL.Core
{
    public interface IFluentSqlSelectGroupByHavingStatement : IFluentSqlQueryEnd
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
        /// Applies the ORDER BY operator, sorting the results by the specified columns.
        /// </summary>
        /// <param name="columns">Columns to sort by.</param>
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}