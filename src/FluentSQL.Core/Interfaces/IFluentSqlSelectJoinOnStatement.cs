using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectJoinOnStatement : IFluentSqlQueryEnd
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
        /// Applies the WITH (NOLOCK) modifier, to read records without blocking the table.
        /// </summary>
        IFluentSqlSelectJoinOnWithNoLockStatement WithNoLock();
        /// <summary>
        /// Applies the WHERE operator using the specified condition.
        /// </summary>
        /// <param name="condition">Filter condition.</param>
        IFluentSqlSelectWhereStatement Where(string condition);
        /// <summary>
        /// Applies the WHERE operator using the specified condition.
        /// </summary>
        /// <param name="condition">Filter condition.</param>
        /// <param name="parameters">Collection of key-value pairs containing parameter names and values.</param>
        IFluentSqlSelectWhereStatement Where(string condition, Dictionary<string, object> parameters);
        /// <summary>
        /// Applies the WHERE operator using the specified condition.
        /// </summary>
        /// <param name="condition">Filter condition.</param>
        /// <param name="parameters">Object containing parameter names and values.</param>
        IFluentSqlSelectWhereStatement Where(string condition, object parameters);
        /// <summary>
        /// Applies the GROUP BY operator for the specified columns.
        /// </summary>
        /// <param name="columns">Columns to group by.</param>
        IFluentSqlSelectGroupByStatement GroupBy(params string[] columns);
        /// <summary>
        /// Applies the ORDER BY operator, sorting the results by the specified columns.
        /// </summary>
        /// <param name="columns">Columns to sort by.</param>
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}
