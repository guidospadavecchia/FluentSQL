using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlQueryEnd
    {
        /// <summary>
        /// Executes the query and returns a collection of dynamic objects.
        /// </summary>
        /// <returns>Collection of dynamic objects.</returns>
        IEnumerable<dynamic> ToDynamic();
        /// <summary>
        /// Executes the query asynchronously and returns a collection of dynamic objects.
        /// </summary>
        /// <returns>A task with a collection of dynamic objects.</returns>
        Task<IEnumerable<dynamic>> ToDynamicAsync();
        /// <summary>
        /// Executes the query and maps the result to a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <returns>Collection of <typeparamref name="T"/> typed objects.</returns>
        IEnumerable<T> ToMappedObject<T>();
        /// <summary>
        /// Executes the query asynchronously and maps the result to a collection of <typeparamref name="T"/> typed objects.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <returns>A task with a collection of <typeparamref name="T"/> typed objects.</returns>
        Task<IEnumerable<T>> ToMappedObjectAsync<T>();
        /// <summary>
        /// Executes the query and returns a single dynamic object.
        /// </summary>
        /// <returns>Single dynamic object or default.</returns>
        dynamic ToDynamicSingle();
        /// <summary>
        /// Executes the query asynchronously and returns a single dynamic object.
        /// </summary>
        /// <returns>A task with a single dynamic object, or default.</returns>
        Task<dynamic> ToDynamicSingleAsync();
        /// <summary>
        /// Executes the query and maps the result to a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <returns>A single <typeparamref name="T"/> typed object or default.</returns>
        T ToMappedObjectSingle<T>();
        /// <summary>
        /// Executes the query asynchronously and maps the result to a single <typeparamref name="T"/> typed object.
        /// </summary>
        /// <typeparam name="T">Type of the return object.</typeparam>
        /// <returns>A task with a single <typeparamref name="T"/> typed object, or default.</returns>
        Task<T> ToMappedObjectSingleAsync<T>();
    }
}
