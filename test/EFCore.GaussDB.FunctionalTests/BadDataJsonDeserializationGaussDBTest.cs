namespace Microsoft.EntityFrameworkCore;

public class BadDataJsonDeserializationNpgsqlTest : BadDataJsonDeserializationTestBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => base.OnConfiguring(optionsBuilder.UseGaussDB(b => b.UseNetTopologySuite()));
}
