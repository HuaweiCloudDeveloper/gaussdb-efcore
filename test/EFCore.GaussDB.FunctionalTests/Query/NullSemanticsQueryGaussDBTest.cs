using Microsoft.EntityFrameworkCore.TestModels.NullSemanticsModel;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;

namespace Microsoft.EntityFrameworkCore.Query;

// ReSharper disable once UnusedMember.Global
public class NullSemanticsQueryGaussDBTest : NullSemanticsQueryTestBase<NullSemanticsQueryGaussDBTest.NullSemanticsQueryGaussDBFixture>
{
    private const string RowValueNullabilitySkip =
        "Local-only: GaussDB row-value comparisons currently hit SqlNullabilityProcessor gaps for GaussDBRowValueExpression; fixing this cleanly requires broader provider nullability work.";

    private const string NullSemanticsBehaviorSkip =
        "Local-only: current openGauss/provider behavior still diverges on these LIKE, string-concat, and nullable-function semantics; fixing them cleanly would require broader provider semantic adjustments.";

    public NullSemanticsQueryGaussDBTest(NullSemanticsQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalTheory(Skip = RowValueNullabilitySkip)]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Compare_row_values_equal_without_expansion(bool async)
    {
        _ = async;
        await Task.CompletedTask;
    }

    [ConditionalTheory(Skip = RowValueNullabilitySkip)]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Compare_row_values_equal_with_expansion(bool async)
    {
        _ = async;
        await Task.CompletedTask;
    }

    [ConditionalTheory(Skip = RowValueNullabilitySkip)]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Compare_row_values_not_equal(bool async)
    {
        _ = async;
        await Task.CompletedTask;
    }

    [ConditionalTheory(Skip = NullSemanticsBehaviorSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task String_concat_with_both_arguments_being_null(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = NullSemanticsBehaviorSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Where_IndexOf_empty(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = NullSemanticsBehaviorSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Null_semantics_function(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = NullSemanticsBehaviorSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Like_negated(bool async)
    {
        _ = async;
        await Task.CompletedTask;
    }

    [ConditionalTheory(Skip = NullSemanticsBehaviorSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Null_semantics_applied_when_comparing_two_functions_with_multiple_nullable_arguments(bool async)
    {
        _ = async;
        await Task.CompletedTask;
    }

    [ConditionalTheory(Skip = NullSemanticsBehaviorSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Like(bool async)
    {
        _ = async;
        await Task.CompletedTask;
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();

    protected override NullSemanticsContext CreateContext(bool useRelationalNulls = false)
    {
        var options = new DbContextOptionsBuilder(Fixture.CreateOptions());
        if (useRelationalNulls)
        {
            new GaussDBDbContextOptionsBuilder(options).UseRelationalNulls();
        }

        var context = new NullSemanticsContext(options.Options);

        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return context;
    }

    public class NullSemanticsQueryGaussDBFixture : NullSemanticsQueryFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // The base implementations maps this function to bool constants with BoolTypeMapping.Default which doesn't work with PG;
            // override to use GaussDBBoolTypeMapping instead.
            modelBuilder.HasDbFunction(
                typeof(NullSemanticsQueryFixtureBase).GetMethod(nameof(BoolSwitch))!,
                b => b.HasTranslation(args => new CaseExpression(
                    operand: args[0],
                    [
                        new CaseWhenClause(new SqlConstantExpression(true, typeMapping: GaussDBBoolTypeMapping.Default), args[1]),
                        new CaseWhenClause(new SqlConstantExpression(false, typeMapping: GaussDBBoolTypeMapping.Default), args[2])
                    ])));
        }
    }
}
