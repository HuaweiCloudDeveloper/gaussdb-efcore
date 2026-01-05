namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class GaussDBNodaTimeApiConsistencyTest(GaussDBNodaTimeApiConsistencyTest.GaussDBNodaTimeApiConsistencyFixture fixture)
    : ApiConsistencyTestBase<GaussDBNodaTimeApiConsistencyTest.GaussDBNodaTimeApiConsistencyFixture>(fixture)
{
    protected override void AddServices(ServiceCollection serviceCollection)
        => serviceCollection.AddEntityFrameworkGaussDBNodaTime();

    protected override Assembly TargetAssembly
        => typeof(GaussDBNodaTimeServiceCollectionExtensions).Assembly;

    public class GaussDBNodaTimeApiConsistencyFixture : ApiConsistencyFixtureBase
    {
        public override HashSet<Type> FluentApiTypes { get; } =
            [typeof(GaussDBNodaTimeDbContextOptionsBuilderExtensions), typeof(GaussDBNodaTimeServiceCollectionExtensions)];
    }
}
