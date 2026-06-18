# GaussDB Entity Framework Core provider for GaussDB

[![stable](https://img.shields.io/nuget/v/HuaweiCloud.GaussDB.EntityFrameworkCore.svg?label=stable)](https://www.nuget.org/packages/HuaweiCloud.GaussDB.EntityFrameworkCore/)
[![next patch](https://img.shields.io/myget/npgsql/v/HuaweiCloud.GaussDB.EntityFrameworkCore.svg?label=next%20patch)](https://www.myget.org/feed/npgsql/package/nuget/HuaweiCloud.GaussDB.EntityFrameworkCore)
[![daily builds (vnext)](https://img.shields.io/myget/npgsql-vnext/v/HuaweiCloud.GaussDB.EntityFrameworkCore.svg?label=vNext)](https://www.myget.org/feed/npgsql-vnext/package/nuget/HuaweiCloud.GaussDB.EntityFrameworkCore)
[![build](https://github.com/HuaweiCloudDeveloper/gaussdb-efcore/actions/workflows/build.yml/badge.svg)](https://github.com/HuaweiCloudDeveloper/gaussdb-efcore/actions/workflows/build.yml)

HuaweiCloud.GaussDB.EntityFrameworkCore is the open source EF Core provider for GaussDB. It allows you to interact with GaussDB via the most widely-used .NET O/RM from Microsoft, and use familiar LINQ syntax to express queries. It's built on top of [GaussDB](https://github.com/HuaweiCloudDeveloper/gaussdb-dotnet).

The provider looks and feels just like any other Entity Framework Core provider. Here's a quick sample to get you started:

```csharp
await using var ctx = new BlogContext();
await ctx.Database.EnsureDeletedAsync();
await ctx.Database.EnsureCreatedAsync();

// Insert a Blog
ctx.Blogs.Add(new() { Name = "FooBlog" });
await ctx.SaveChangesAsync();

// Query all blogs who's name starts with F
var fBlogs = await ctx.Blogs.Where(b => b.Name.StartsWith("F")).ToListAsync();

public class BlogContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseGaussDB(@"Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase");
}

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

Aside from providing general EF Core support for GaussDB, the provider also exposes some GaussDB-specific capabilities, allowing you to query JSON, array or range columns, as well as many other advanced features. For more information, see the [the GaussDB site](https://doc.hcs.huawei.com/db/zh-cn/index.html). For information about EF Core in general, see the [EF Core website](https://docs.microsoft.com/ef/core/).

## Package naming

Use `HuaweiCloud.GaussDB.EntityFrameworkCore` for the main EF Core provider package. The NuGet package IDs use the `HuaweiCloud.GaussDB.*` prefix, while the .NET namespaces and public APIs remain unchanged. Existing code such as `using HuaweiCloud.EntityFrameworkCore.GaussDB;` and `UseGaussDB(...)` does not need to change.

If you used the previous package IDs, update `PackageReference` entries as follows and remove the old package references to avoid duplicate assembly references.

| Previous package ID | Current package ID |
| --- | --- |
| `HuaweiCloud.EntityFrameworkCore.GaussDB` | `HuaweiCloud.GaussDB.EntityFrameworkCore` |
| `HuaweiCloud.EntityFrameworkCore.GaussDB.NodaTime` | `HuaweiCloud.GaussDB.EntityFrameworkCore.NodaTime` |
| `HuaweiCloud.EntityFrameworkCore.GaussDB.NetTopologySuite` | `HuaweiCloud.GaussDB.EntityFrameworkCore.NetTopologySuite` |
| `HuaweiCloud.Driver.GaussDB` | `HuaweiCloud.GaussDB.Driver` |
| `HuaweiCloud.Driver.GaussDB.NodaTime` | `HuaweiCloud.GaussDB.Driver.NodaTime` |
| `HuaweiCloud.Driver.GaussDB.NetTopologySuite` | `HuaweiCloud.GaussDB.Driver.NetTopologySuite` |
| `HuaweiCloud.Driver.GaussDB.DependencyInjection` | `HuaweiCloud.GaussDB.Driver.DependencyInjection` |

## Testing

To run the full database-backed test suite against a remote GaussDB instance, see [Standard Full Test Guide](FULL_TEST_GUIDE.md).

## Related packages

* Spatial plugin to work with GaussDB PostGIS: [HuaweiCloud.GaussDB.EntityFrameworkCore.NetTopologySuite](https://www.nuget.org/packages/HuaweiCloud.GaussDB.EntityFrameworkCore.NetTopologySuite)
* NodaTime plugin to use better date/time types with GaussDB: [HuaweiCloud.GaussDB.EntityFrameworkCore.NodaTime](https://www.nuget.org/packages/HuaweiCloud.GaussDB.EntityFrameworkCore.NodaTime)
* The underlying GaussDB ADO.NET provider is [GaussDB](https://www.nuget.org/packages/HuaweiCloud.GaussDB.Driver/).
