using System.Collections.Generic;

namespace FluentSQL.Core
{
    public interface IFluentSqlInsertStatement
    {
        IFluentSqlInsertValuesStatement Values(Dictionary<string, object> values);
    }
}
