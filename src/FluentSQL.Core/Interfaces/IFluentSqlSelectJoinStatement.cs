using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlSelectJoinStatement
    {
        IFluentSqlSelectJoinOnStatement On(string condition);
        IFluentSqlSelectJoinOnStatement On(string condition, Dictionary<string, object> parameters);
    }
}
