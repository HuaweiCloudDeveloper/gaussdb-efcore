using System.ComponentModel.DataAnnotations.Schema;

namespace GetStarted
{
    /// <summary>
    /// 定义员工实体类，映射到 GaussDB 数据库中的 "employees" 表
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 员工姓名，最长 128 字符
        /// </summary>
        [Column("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// 员工年龄
        /// </summary>
        [Column("age")]
        public int Age { get; set; }
    }

}
