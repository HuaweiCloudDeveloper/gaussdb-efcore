namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class GaussDBMigrationBuilderTest
{
    [Fact]
    public void IsGaussDB_when_using_GaussDB()
    {
        var migrationBuilder = new MigrationBuilder("HuaweiCloud.EntityFrameworkCore.GaussDB");
        Assert.True(migrationBuilder.IsGaussDB());
    }

    [Fact]
    public void Not_IsGaussDB_when_using_different_provider()
    {
        var migrationBuilder = new MigrationBuilder("Microsoft.EntityFrameworkCore.InMemory");
        Assert.False(migrationBuilder.IsGaussDB());
    }
}
