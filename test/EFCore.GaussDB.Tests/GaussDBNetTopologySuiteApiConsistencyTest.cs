namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class GaussDBNetTopologySuiteApiConsistencyTest(
    GaussDBNetTopologySuiteApiConsistencyTest.GaussDBNetTopologySuiteApiConsistencyFixture fixture)
    : ApiConsistencyTestBase<GaussDBNetTopologySuiteApiConsistencyTest.GaussDBNetTopologySuiteApiConsistencyFixture>(fixture)
{
    protected override void AddServices(ServiceCollection serviceCollection)
        => serviceCollection.AddEntityFrameworkGaussDBNetTopologySuite();

    protected override Assembly TargetAssembly
        => typeof(GaussDBNetTopologySuiteServiceCollectionExtensions).Assembly;

    public class GaussDBNetTopologySuiteApiConsistencyFixture : ApiConsistencyFixtureBase
    {
        public override HashSet<Type> FluentApiTypes { get; } =
            [typeof(GaussDBNetTopologySuiteDbContextOptionsBuilderExtensions), typeof(GaussDBNetTopologySuiteServiceCollectionExtensions)];
    }
}
