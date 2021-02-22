using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlUpdateStatement
    {
        IFluentSqlUpdateSetStatement Set(Dictionary<string, object> assignments);
    }
}
