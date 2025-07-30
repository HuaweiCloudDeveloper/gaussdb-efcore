using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Update.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Update.Internal;

// ReSharper disable once CheckNamespace
namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Tests.Update;

public class GaussDBModificationCommandBatchTest
{
    [Fact]
    public void AddCommand_returns_false_when_max_batch_size_is_reached()
    {
        var typeMapper = new GaussDBTypeMappingSource(
            TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
            TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>(),
            new GaussDBSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()),
            new GaussDBSingletonOptions());

        var batch = new GaussDBModificationCommandBatch(
            new ModificationCommandBatchFactoryDependencies(
                new RelationalCommandBuilderFactory(
                    new RelationalCommandBuilderDependencies(
                        typeMapper,
                        new ExceptionDetector(),
                        new LoggingOptions())),
                new GaussDBSqlGenerationHelper(
                    new RelationalSqlGenerationHelperDependencies()),
                new GaussDBUpdateSqlGenerator(
                    new UpdateSqlGeneratorDependencies(
                        new GaussDBSqlGenerationHelper(
                            new RelationalSqlGenerationHelperDependencies()),
                        typeMapper)),
                new CurrentDbContext(new FakeDbContext()),
                new FakeRelationalCommandDiagnosticsLogger(),
                new FakeDiagnosticsLogger<DbLoggerCategory.Update>()),
            maxBatchSize: 1);

        Assert.True(
            batch.TryAddCommand(
                CreateModificationCommand("T1", null, false)));
        Assert.False(
            batch.TryAddCommand(
                CreateModificationCommand("T1", null, false)));
    }

    private class FakeDbContext : DbContext;

    private static INonTrackedModificationCommand CreateModificationCommand(
        string name,
        string schema,
        bool sensitiveLoggingEnabled)
        => new ModificationCommandFactory().CreateNonTrackedModificationCommand(
            new NonTrackedModificationCommandParameters(name, schema, sensitiveLoggingEnabled));
}
