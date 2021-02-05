﻿using System.Threading.Tasks;

namespace FluentSQL.Core
{
    public interface IFluentSqlDeleteStatement
    {
        string Query { get; }
        int? CommandTimeout { get; }

        IFluentSqlNonQueryWhereStatement Where(string condition);

        int Execute();
        Task<int> ExecuteAsync();
    }
}
