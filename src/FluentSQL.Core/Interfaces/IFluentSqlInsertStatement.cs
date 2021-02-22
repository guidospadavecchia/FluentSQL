using System;
using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlInsertStatement
    {
        /// <summary>
        /// Indicates the columns and values to insert.
        /// </summary>
        /// <param name="values">Collection of key-value pairs containing parameter names and values to be inserted.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> is null or empty.</exception>
        IFluentSqlInsertValuesStatement Values(Dictionary<string, object> values);
    }
}
