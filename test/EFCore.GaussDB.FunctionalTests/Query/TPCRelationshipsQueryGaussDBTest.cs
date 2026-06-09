namespace Microsoft.EntityFrameworkCore.Query;

public class TPCRelationshipsQueryGaussDBTest
    : TPCRelationshipsQueryTestBase<TPCRelationshipsQueryGaussDBTest.TPCRelationshipsQueryGaussDBFixture>
{
    public TPCRelationshipsQueryGaussDBTest(
        TPCRelationshipsQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override Task Include_reference_with_inheritance_reverse(bool async)
        => TestEnvironment.IsDistributed
            ? Task.CompletedTask
            : base.Include_reference_with_inheritance_reverse(async);

    public override Task Include_reference_with_inheritance_with_filter_reverse(bool async)
        => TestEnvironment.IsDistributed
            ? Task.CompletedTask
            : base.Include_reference_with_inheritance_with_filter_reverse(async);

    public override Task Nested_include_with_inheritance_reference_reference_reverse(bool async)
        => TestEnvironment.IsDistributed
            ? Task.CompletedTask
            : base.Nested_include_with_inheritance_reference_reference_reverse(async);

    public override Task Nested_include_with_inheritance_collection_reference_reverse(bool async)
        => TestEnvironment.IsDistributed
            ? Task.CompletedTask
            : base.Nested_include_with_inheritance_collection_reference_reverse(async);

    public override Task Nested_include_with_inheritance_collection_reference_reverse_split(bool async)
        => TestEnvironment.IsDistributed
            ? Task.CompletedTask
            : base.Nested_include_with_inheritance_collection_reference_reverse_split(async);

    public class TPCRelationshipsQueryGaussDBFixture : TPCRelationshipsQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
