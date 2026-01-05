using System.Data;
using dotenv.net;
using GetStarted;

DotEnv.Load(); // 加载 .env 文件中的环境变量

var connString = Environment.GetEnvironmentVariable("GaussDBConnString");
ArgumentNullException.ThrowIfNull(connString);
// ReSharper disable AccessToDisposedClosure
// Dispose will be handled
await using var conn = new GaussDBConnection(connString);
if (conn.State is ConnectionState.Closed)
{
    await conn.OpenAsync();
}

Console.WriteLine($@"Connection state: {conn.State}");


// 配置DbContext
var optionsBuilder = new DbContextOptionsBuilder<GaussDBDbContext>();
optionsBuilder.UseGaussDB(connString)
              .LogTo(Console.WriteLine) // 开启日志，便于排查详细错误
              .EnableSensitiveDataLogging(); // 开发环境临时开启，生产环境关闭

await using var ctx = new GaussDBDbContext(optionsBuilder.Options);

// ⚠️开发时建议使用：清空旧表并重新建表
try
{
    if (await ctx.Database.CanConnectAsync())
    {
        Console.WriteLine("📦 检测到数据库存在，准备删除...");
        await ctx.Database.EnsureDeletedAsync(); // 删除数据库
        Console.WriteLine("✅ 数据库已成功删除");
    }
    else
    {
        Console.WriteLine("ℹ️ 数据库不存在，无需删除");
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message + e.StackTrace);
}

// 然后创建数据库结构
var CreateSuccess = await ctx.Database.EnsureCreatedAsync(); // 创建数据库结构
Console.WriteLine("✅ 数据库结构已创建" + CreateSuccess);

if (!CreateSuccess)
{
    // ✅ 手动创建表
    await CreateTable(ctx);
}

// 插入初始数据
ctx.Employees?.AddRange(
    new Employee { Name = "John", Age = 30 },
    new Employee { Name = "Alice", Age = 16 },
    new Employee { Name = "Mike", Age = 24 }
);
await ctx.SaveChangesAsync(); // 提交更改
Console.WriteLine("✅ 初始数据已插入");

// 查询所有员工
await QueryTest(ctx);
// 更新 Alice 的年龄为 18
var alice = await ctx.Employees.FirstOrDefaultAsync(e => e.Name == "Alice");
if (alice != null)
{
    alice.Age = 18;
    await ctx.SaveChangesAsync();
    Console.WriteLine("✅ 已更新 Alice 的年龄为 18");
}

// 查询年龄大于等于 18 的员工
await QueryTest(ctx, e => e.Age >= 18);

// 删除年龄大于 10 的员工
var toDelete = await ctx.Employees.Where(e => e.Age > 10).ToListAsync();
ctx.Employees.RemoveRange(toDelete);
await ctx.SaveChangesAsync();
Console.WriteLine("✅ 删除了年龄 > 10 的员工");

// 查询剩余人数
var count = await ctx.Employees.CountAsync();
Console.WriteLine($"📊 当前员工总数: {count}");

Console.WriteLine("🎉 所有操作已完成！");


// 封装查询方法，可传入条件表达式
static async Task QueryTest(GaussDBDbContext ctx, Expression<Func<Employee, bool>> predicate = null)
{
    var query = ctx.Employees?.AsQueryable();

    // 如果有条件，则筛选
    if (predicate != null)
        query = query?.Where(predicate);

    if (query == null)
        return;

    var results = await query.OrderBy(s => s.Age).ToListAsync();
    foreach (var e in results)
    {
        Console.WriteLine($"👤 ID: {e.Id}, Name: {e.Name}, Age: {e.Age}");
    }
}

async Task CreateTable(GaussDBDbContext ctx)
{
    const string createSql = """
        CREATE TABLE IF NOT EXISTS employees (
            id INT PRIMARY KEY,
            name VARCHAR(128),
            age INT
        );
        """;

    await ctx.Database.ExecuteSqlRawAsync(createSql);
    Console.WriteLine("✅ 创建表 employees 完成");
}

