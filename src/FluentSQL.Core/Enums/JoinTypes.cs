namespace FluentSQL.Core
{
    /// <summary>
    /// Specifies the different types of joins.
    /// </summary>
    public enum JoinTypes
    {
        /// <summary>
        /// Indicates an INNER JOIN. Returns records that have matching values in both tables.
        /// </summary>
        Inner,
        /// <summary>
        /// Indicates a LEFT JOIN. Returns all records from the left table, and the matched records from the right table.
        /// </summary>
        Left,
        /// <summary>
        /// Indicates a RIGHT JOIN. Returns all records from the right table, and the matched records from the left table.
        /// </summary>
        Right,
        /// <summary>
        /// Indicates a FULL OUTER JOIN. Returns all records when there is a match in either left or right table.
        /// </summary>
        FullOuter
    }
}
