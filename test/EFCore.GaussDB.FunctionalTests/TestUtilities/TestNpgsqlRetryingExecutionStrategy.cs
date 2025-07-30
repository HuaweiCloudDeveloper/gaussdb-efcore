using HuaweiCloud.EntityFrameworkCore.GaussDB;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class TestGaussDBRetryingExecutionStrategy : GaussDBRetryingExecutionStrategy
{
    private const bool ErrorNumberDebugMode = false;

    private static readonly string[] AdditionalSqlStates = ["XX000"];

    public TestGaussDBRetryingExecutionStrategy()
        : base(
            new DbContext(
                new DbContextOptionsBuilder()
                    .EnableServiceProviderCaching(false)
                    .UseGaussDB(TestEnvironment.DefaultConnection).Options),
            DefaultMaxRetryCount, DefaultMaxDelay, AdditionalSqlStates)
    {
    }

    public TestGaussDBRetryingExecutionStrategy(DbContext context)
        : base(context, DefaultMaxRetryCount, DefaultMaxDelay, AdditionalSqlStates)
    {
    }

    public TestGaussDBRetryingExecutionStrategy(DbContext context, TimeSpan maxDelay)
        : base(context, DefaultMaxRetryCount, maxDelay, AdditionalSqlStates)
    {
    }

    // ReSharper disable once UnusedMember.Global
    public TestGaussDBRetryingExecutionStrategy(ExecutionStrategyDependencies dependencies)
        : base(dependencies, DefaultMaxRetryCount, DefaultMaxDelay, AdditionalSqlStates)
    {
    }

    protected override bool ShouldRetryOn(Exception? exception)
    {
        if (base.ShouldRetryOn(exception))
        {
            return true;
        }

#pragma warning disable 162
        if (ErrorNumberDebugMode && exception is PostgresException postgresException)
        {
            var message = $"Didn't retry on {postgresException.SqlState}";
            throw new InvalidOperationException(message, exception);
        }
#pragma warning restore 162

        return exception is InvalidOperationException { Message: "Internal .Net Framework Data Provider error 6." };
    }

    public new virtual TimeSpan? GetNextDelay(Exception lastException)
    {
        ExceptionsEncountered.Add(lastException);
        return base.GetNextDelay(lastException);
    }
}
