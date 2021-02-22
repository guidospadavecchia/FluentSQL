namespace FluentSQL.Core
{
    public interface IFluentSqlSelectTopStatement : IFluentSqlQueryEnd
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
        /// Applies the FROM operator indicating the table to query.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        IFluentSqlSelectFromStatement From(string table);
        /// <summary>
        /// Applies the FROM operator indicating the table to query.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <param name="tableAlias">Alias of the table.</param>
        IFluentSqlSelectFromStatement From(string table, string tableAlias);
    }
}
