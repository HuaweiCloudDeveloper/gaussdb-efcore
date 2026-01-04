using dotenv.net;

namespace GetStarted
{
    /// <summary>
    /// EF Core 上下文类，用于配置和操作 GaussDB 数据库
    /// </summary>
    public class GaussDBDbContext : DbContext
    {
        /// <summary>
        /// 员工表，EF Core 会自动根据模型类进行映射
        /// </summary>
        public DbSet<Employee> Employees { get; set; } = null!;

        /// <summary>
        /// 连接字符串配置
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            DotEnv.Load(); // 加载 .env 文件中的环境变量

            // 从环境变量中读取连接字符串（需通过 dotenv 或系统环境变量设置）
            var connString = Environment.GetEnvironmentVariable("GaussDBConnString")
                             ?? throw new InvalidOperationException("未找到 GaussDBConnString 环境变量");

            // 使用 GaussDB 的 EF Core provider（确保你已安装 UseGaussDB 的 NuGet 扩展）
            optionsBuilder.UseGaussDB(connString);
        }

        /// <summary>
        /// 定义实体与表的映射关系
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("employees");              // 映射表名
            modelBuilder.Entity<Employee>().HasKey(e => e.Id);                // 设置主键
            modelBuilder.Entity<Employee>().Property(e => e.Name).HasMaxLength(128);  // 设置字段长度
        }
    }

}
