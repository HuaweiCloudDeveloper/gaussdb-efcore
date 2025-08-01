﻿namespace Microsoft.EntityFrameworkCore.Query;

public class OwnedQueryGaussDBTest(OwnedQueryGaussDBTest.OwnedQueryGaussDBFixture fixture)
    : OwnedQueryRelationalTestBase<OwnedQueryGaussDBTest.OwnedQueryGaussDBFixture>(fixture)
{
    public class OwnedQueryGaussDBFixture : RelationalOwnedQueryFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // We default to mapping DateTime to 'timestamp with time zone', but the seeding data has Unspecified DateTimes which aren't
            // supported.
            modelBuilder.Entity<OwnedPerson>(
                eb => eb.OwnsMany(
                    p => p.Orders, ob => ob.IndexerProperty<DateTime>("OrderDate").HasColumnType("timestamp without time zone")));
        }
    }
}
