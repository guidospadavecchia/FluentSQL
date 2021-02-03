using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlSelectDistinctStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlSelectTopStatement Top(int rows);
        IFluentSqlSelectFromStatement From(string table);
        IFluentSqlSelectFromStatement From(string table, string tableAlias);

        IEnumerable<dynamic> ToDynamic();
        IEnumerable<T> ToMappedObject<T>();
    }
}
