using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectTopStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectFromStatement From(string table);
        IFluentSqlSelectFromStatement From(string table, string tableAlias);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}
