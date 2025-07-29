namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class NpgsqlMigrationBuilderTest
{
    [Fact]
    public void IsNpgsql_when_using_Npgsql()
    {
        var migrationBuilder = new MigrationBuilder("HuaweiCloud.EntityFrameworkCore.GaussDB");
        Assert.True(migrationBuilder.IsNpgsql());
    }

    [Fact]
    public void Not_IsNpgsql_when_using_different_provider()
    {
        var migrationBuilder = new MigrationBuilder("Microsoft.EntityFrameworkCore.InMemory");
        Assert.False(migrationBuilder.IsNpgsql());
    }
}
