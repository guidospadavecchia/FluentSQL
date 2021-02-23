using System.Collections.Generic;

namespace FluentSQL.Core
{
    /// <summary>
    /// Represents a SELECT statement with a GROUP BY clause.
    /// </summary>
    public interface IFluentSqlSelectGroupByStatement : IFluentSqlQueryEnd
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
        /// Applies the HAVING keyword after a GROUP BY, using the specified condition.
        /// </summary>
        /// <param name="condition">Filter GROUP BY condition.</param>
        IFluentSqlSelectGroupByHavingStatement Having(string condition);
        /// <summary>
        /// Applies the HAVING keyword after a GROUP BY, using the specified condition.
        /// </summary>
        /// <param name="condition">Filter GROUP BY condition.</param>
        /// <param name="parameters">Collection of key-value pairs containing parameter names and values.</param>
        IFluentSqlSelectGroupByHavingStatement Having(string condition, Dictionary<string, object> parameters);
        /// <summary>
        /// Applies the HAVING keyword after a GROUP BY, using the specified condition.
        /// </summary>
        /// <param name="condition">Filter GROUP BY condition.</param>
        /// <param name="parameters">Object containing parameter names and values.</param>
        IFluentSqlSelectGroupByHavingStatement Having(string condition, object parameters);
        /// <summary>
        /// Applies the ORDER BY operator, sorting the results by the specified columns.
        /// </summary>
        /// <param name="columns">Columns to sort by.</param>
        IFluentSqlSelectOrderByStatement OrderBy(params string[] columns);
    }
}