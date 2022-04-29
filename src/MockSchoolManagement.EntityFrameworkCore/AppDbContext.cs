using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using System.Linq;

namespace MockSchoolManagement.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<OfficeLocation> OfficeLocations { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        //DbContextOptions实例负责承载应用中的配置信息，如连接字符串、数据库提供商等内容。
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            // 获取当前系统中所有领域模型上的外键列表
            var foreignKeys = modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys());
            foreach (var foreignKey in foreignKeys)
            {
                //将它们的删除行为配置为Restrict，即无操作
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
