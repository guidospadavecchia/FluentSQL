using System;
using System.Collections.Generic;

namespace FluentSQL.Core
{
    /// <summary>
    /// Represents an UPDATE statement.
    /// </summary>
    public interface IFluentSqlUpdateStatement
    {
        /// <summary>
        /// Indicates the assignments to be done by the UPDATE statement using a collection of key-value pairs.
        /// </summary>
        /// <param name="assignments">Collection of new values. Each key-value pair specifies the name of the column and its new value respectively.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="assignments"/> is null or empty.</exception>
        IFluentSqlUpdateSetStatement Set(Dictionary<string, object> assignments);
    }
}
