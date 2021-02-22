using System;
using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlUpdateStatement
    {
        /// <summary>
        /// Indicates the assignments to be done by the UPDATE statement using a collection of key-value pairs.
        /// </summary>
        /// <param name="assignments">Collection of new values. Each <see cref="KeyValuePair{string, object}"/> specifies the name of the column and its new value respectively.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="assignments"/> is null or empty.</exception>
        IFluentSqlUpdateSetStatement Set(Dictionary<string, object> assignments);
    }
}
