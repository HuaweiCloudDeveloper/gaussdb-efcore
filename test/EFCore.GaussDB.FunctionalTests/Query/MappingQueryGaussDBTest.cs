namespace Microsoft.EntityFrameworkCore.Query;

// ReSharper disable once UnusedMember.Global
public class MappingQueryGaussDBTest : MappingQueryTestBase<MappingQueryGaussDBTest.MappingQueryGaussDBFixture>
{
    public MappingQueryGaussDBTest(MappingQueryGaussDBFixture fixture)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    public class MappingQueryGaussDBFixture : MappingQueryFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBNorthwindTestStoreFactory.Instance;

        protected override string DatabaseSchema { get; } = "public";

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<MappedCustomer>(
                e =>
                {
                    e.Property(c => c.CompanyName2).Metadata.SetColumnName("CompanyName");
                    e.Metadata.SetTableName("Customers");
                    e.Metadata.SetSchema("public");
                });

            modelBuilder.Entity<MappedEmployee>()
                .Property(c => c.EmployeeID)
                .HasColumnType("int");
        }
    }
}
