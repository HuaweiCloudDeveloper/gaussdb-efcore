namespace Microsoft.EntityFrameworkCore;

public class ModelBuilding101GaussDBTest : ModelBuilding101RelationalTestBase
{
    protected override DbContextOptionsBuilder ConfigureContext(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseGaussDB();
}
