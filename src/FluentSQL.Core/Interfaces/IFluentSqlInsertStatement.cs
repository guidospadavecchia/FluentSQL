using System;
using System.Collections.Generic;

namespace FluentSQL.Core
{
    /// <summary>
    /// Represents an INSERT statement.
    /// </summary>
    public interface IFluentSqlInsertStatement
    {
        /// <summary>
        /// Indicates the columns and values to insert.
        /// </summary>
        /// <param name="values">Collection of key-value pairs containing parameter names and values to be inserted.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> is null or empty.</exception>
        IFluentSqlInsertValuesStatement Values(Dictionary<string, object> values);
        /// <summary>
        /// Indicates the columns and values to insert.
        /// </summary>
        /// <param name="valuesObject">Object containing properties with parameter names and values to be inserted.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="valuesObject"/> is null or empty.</exception>
        IFluentSqlInsertValuesStatement Values(object valuesObject);
    }
}
