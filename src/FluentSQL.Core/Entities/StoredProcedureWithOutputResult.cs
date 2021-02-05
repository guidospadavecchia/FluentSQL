using System.Collections.Generic;

namespace FluentSQL.Core
{
    /// <summary>
    /// Represents the result of a Stored Procedure execution with output parameters.
    /// </summary>
    /// <typeparam name="T">The return object type.</typeparam>
    public sealed class StoredProcedureWithOutputResult<T>
    {
        /// <summary>
        /// The Stored Procedure execution result.
        /// </summary>
        public T ReturnValue { get; private set; }
        /// <summary>
        /// A collection of key-value pairs with the output parameters names and values.
        /// </summary>
        public Dictionary<string, object> OutputParameters { get; private set; }

        /// <summary>
        /// Creates a new Stored Procedure result with output parameters.
        /// </summary>
        /// <param name="returnValue">The Stored Procedure execution result.</param>
        /// <param name="outputParameters">A collection of key-value pairs with the output parameters names and values.</param>
        internal StoredProcedureWithOutputResult(T returnValue, Dictionary<string, object> outputParameters)
        {
            ReturnValue = returnValue;
            OutputParameters = outputParameters;
        }
    }
}
