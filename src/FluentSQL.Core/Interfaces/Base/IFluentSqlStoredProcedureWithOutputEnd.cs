using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    /// <summary>
    /// Represents a Stored Procedure with output parameters ready to be executed.
    /// </summary>
    public interface IFluentSqlStoredProcedureWithOutputEnd
    {
        /// <summary>
        /// Executes the Stored Procedure as a non-query, returning the affected rows and the output parameters.
        /// </summary>
        /// <returns>An object containing the affected rows and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<int> ExecuteNonQuery();
        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a collection of dynamic objects and a collection of output parameters.
        /// </summary>
        /// <returns>An object containing a collection of dynamic objects and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<IEnumerable<dynamic>> ExecuteToDynamic();
        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a single dynamic object and a collection of output parameters.
        /// </summary>
        /// <returns>An object containing a single dynamic object and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<dynamic> ExecuteToDynamicSingle();
        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a collection of <typeparamref name="T"/> typed objects and a collection of output parameters.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>An object containing a collection of <typeparamref name="T"/> typed objects representing a row each, and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<IEnumerable<T>> ExecuteToMappedObject<T>();
        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a single <typeparamref name="T"/> typed object and a collection of output parameters.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>An object containing a single <typeparamref name="T"/> typed object, and a collection of key-value pairs with the output parameters.</returns>
        StoredProcedureWithOutputResult<T> ExecuteToMappedObjectSingle<T>();

        /// <summary>
        /// Executes the Stored Procedure as a non-query asynchronously, returning the affected rows and the output parameters.
        /// </summary>
        /// <returns>A task with an object containing the affected rows and a collection of key-value pairs with the output parameters.</returns>
        Task<StoredProcedureWithOutputResult<int>> ExecuteNonQueryAsync();
        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a collection of dynamic objects and a collection of output parameters.
        /// </summary>
        /// <returns>A task with an object containing a collection of dynamic objects and a collection of key-value pairs with the output parameters.</returns>
        Task<StoredProcedureWithOutputResult<IEnumerable<dynamic>>> ExecuteToDynamicAsync();
        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a single dynamic object and a collection of output parameters.
        /// </summary>
        /// <returns>A task with an object containing a single dynamic object and a collection of key-value pairs with the output parameters.</returns>
        Task<StoredProcedureWithOutputResult<dynamic>> ExecuteToDynamicSingleAsync();
        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects and a collection of output parameters.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>An object containing a collection of <typeparamref name="T"/> typed objects representing a row each, and a collection of key-value pairs with the output parameters.</returns>
        Task<StoredProcedureWithOutputResult<IEnumerable<T>>> ExecuteToMappedObjectAsync<T>();
        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a single <typeparamref name="T"/> typed object and a collection of output parameters.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>An object containing a single <typeparamref name="T"/> typed object, and a collection of key-value pairs with the output parameters.</returns>
        Task<StoredProcedureWithOutputResult<T>> ExecuteToMappedObjectSingleAsync<T>();
    }
}
