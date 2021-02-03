using System.Collections.Generic;

namespace FluentSql.Core
{
    public interface IFluentSqlUpdateStatement
    {
        IFluentSqlUpdateSetStatement Set(Dictionary<string, object> assignments);
        IFluentSqlUpdateSetStatement Set(params string[] assignments);
    }
}
