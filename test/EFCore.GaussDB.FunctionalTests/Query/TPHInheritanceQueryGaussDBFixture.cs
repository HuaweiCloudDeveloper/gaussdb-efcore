using Microsoft.EntityFrameworkCore.TestModels.InheritanceModel;

namespace Microsoft.EntityFrameworkCore.Query;

public class TPHInheritanceQueryGaussDBFixture : TPHInheritanceQueryFixture
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        modelBuilder.Entity<AnimalQuery>().HasNoKey().ToSqlQuery("""
            SELECT * FROM "Animals"
            """);
    }
}
