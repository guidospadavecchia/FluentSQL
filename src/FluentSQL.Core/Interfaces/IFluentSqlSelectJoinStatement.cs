﻿namespace FluentSQL.Core
{
    public interface IFluentSqlSelectJoinStatement
    {
        IFluentSqlSelectJoinOnStatement On(string condition);
    }
}
