using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlStoredProcedureEnd
    {
        /// <summary>
        /// Executes the Stored Procedure as a non-query, and returns the affected rows.
        /// </summary>
        /// <returns>Affected rows.</returns>
        int ExecuteNonQuery();
        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a collection of dynamic objects.
        /// </summary>
        /// <returns>A collection of dynamic objects.</returns>
        IEnumerable<dynamic> ExecuteToDynamic();
        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a single dynamic object.
        /// </summary>
        /// <returns>A single dynamic object or default.</returns>
        dynamic ExecuteToDynamicSingle();
        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>A collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        IEnumerable<T> ExecuteToMappedObject<T>();
        /// <summary>
        /// Executes the Stored Procedure as a query, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>A single <typeparamref name="T"/> typed object, or default.</returns>
        T ExecuteToMappedObjectSingle<T>();

        /// <summary>
        /// Executes the Stored Procedure as a non-query asynchronously, and returns the affected rows.
        /// </summary>
        /// <returns>A task with the affected rows.</returns>
        Task<int> ExecuteNonQueryAsync();
        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a collection of dynamic objects.
        /// </summary>
        /// <returns>A task with a collection of dynamic objects.</returns>
        Task<IEnumerable<dynamic>> ExecuteToDynamicAsync();
        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a single dynamic object.
        /// </summary>
        /// <returns>A task with a single dynamic object, or default.</returns>
        Task<dynamic> ExecuteToDynamicSingleAsync();
        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects, representing a row each.</returns>
        Task<IEnumerable<T>> ExecuteToMappedObjectAsync<T>();
        /// <summary>
        /// Executes the Stored Procedure as a query asynchronously, and returns a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to map to.</typeparam>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        Task<T> ExecuteToMappedObjectSingleAsync<T>();
    }
}
