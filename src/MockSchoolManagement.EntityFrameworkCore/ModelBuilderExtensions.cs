using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;

namespace MockSchoolManagement.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //指定实体在数据库种生成的名称
            modelBuilder.Entity<Course>().ToTable("Course", "School");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse", "School");
            modelBuilder.Entity<Student>().ToTable("Student", "School");
            modelBuilder.Entity<Teacher>().ToTable("Teacher", "School");
            modelBuilder.Entity<Department>().ToTable("Department", "School");
            modelBuilder.Entity<OfficeLocation>().ToTable("OfficeLocation", "School");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment", "School");



            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseID, c.TeacherID });

        }
    }
}
