using Microsoft.EntityFrameworkCore.TestModels.ConcurrencyModel;

namespace Microsoft.EntityFrameworkCore;

public class F1BytesGaussDBFixture : F1GaussDBFixtureBase<byte[]>
{
    protected override void BuildModelExternal(ModelBuilder modelBuilder)
    {
        base.BuildModelExternal(modelBuilder);

        modelBuilder.Entity<Chassis>().Property<byte[]>("Version").HasConversion<BytesToUIntConverter>(new ArrayStructuralComparer<byte>());
        modelBuilder.Entity<Driver>().Property<byte[]>("Version").HasConversion<BytesToUIntConverter>(new ArrayStructuralComparer<byte>());
        modelBuilder.Entity<Team>().Property<byte[]>("Version").HasConversion<BytesToUIntConverter>(new ArrayStructuralComparer<byte>());
        modelBuilder.Entity<Sponsor>().Property<byte[]>("Version").HasConversion<BytesToUIntConverter>(new ArrayStructuralComparer<byte>());
        modelBuilder.Entity<TitleSponsor>()
            .OwnsOne(
                s => s.Details, eb =>
                {
                    eb.Property<byte[]>("Version").IsRowVersion().HasConversion<BytesToUIntConverter>(new ArrayStructuralComparer<byte>());
                });
    }

    private class BytesToUIntConverter() : ValueConverter<byte[], uint>(
        bytes => BitConverter.ToUInt32(bytes),
        num => BitConverter.GetBytes(num),
        mappingHints: null);
}

public class F1GaussDBFixture : F1GaussDBFixtureBase<uint>
{
    protected override void BuildModelExternal(ModelBuilder modelBuilder)
    {
        base.BuildModelExternal(modelBuilder);

        // TODO: This is a hack to work around, remove in 8.0 after https://github.com/dotnet/efcore/pull/29401
        modelBuilder.Entity<Chassis>().Property<uint>("Version").HasConversion((ValueConverter?)null);
        modelBuilder.Entity<Driver>().Property<uint>("Version").HasConversion((ValueConverter?)null);
        modelBuilder.Entity<Team>().Property<uint>("Version").HasConversion((ValueConverter?)null);
        modelBuilder.Entity<Sponsor>().Property<uint>("Version").HasConversion((ValueConverter?)null);
        modelBuilder.Entity<TitleSponsor>()
            .OwnsOne(
                s => s.Details, eb =>
                {
                    eb.Property<uint>("Version").IsRowVersion().HasConversion((ValueConverter?)null);
                });
    }
}

public abstract class F1GaussDBFixtureBase<TRowVersion> : F1RelationalFixture<TRowVersion>
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    public override TestHelpers TestHelpers
        => GaussDBTestHelpers.Instance;
}
