﻿using System.Data;
using System.Data.Common;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class TestPostgisConnection(RelationalConnectionDependencies dependencies, DbDataSource? dataSource = null)
    : GaussDBRelationalConnection(dependencies, dataSource)
{
    public string ErrorCode { get; set; } = "XX000";
    public Queue<bool?> OpenFailures { get; } = new();
    public int OpenCount { get; set; }
    public Queue<bool?> CommitFailures { get; } = new();
    public Queue<bool?> ExecutionFailures { get; } = new();
    public int ExecutionCount { get; set; }

    public override bool Open(bool errorsExpected = false)
    {
        PreOpen();

        return base.Open(errorsExpected);
    }

    public override Task<bool> OpenAsync(CancellationToken cancellationToken, bool errorsExpected = false)
    {
        PreOpen();

        return base.OpenAsync(cancellationToken, errorsExpected);
    }

    private void PreOpen()
    {
        if (DbConnection.State == ConnectionState.Open)
        {
            return;
        }

        OpenCount++;
        if (OpenFailures.Count <= 0)
        {
            return;
        }

        var fail = OpenFailures.Dequeue();

        if (fail.HasValue)
        {
            throw new PostgresException("Simulated failure", "ERROR", "ERROR", ErrorCode);
        }
    }
}
