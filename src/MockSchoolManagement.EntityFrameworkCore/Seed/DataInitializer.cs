using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MockSchoolManagement.Models;
using System;
using System.Linq;

namespace MockSchoolManagement.Infrastructure.Data
{
    public static class DataInitializer
    {
        public static IApplicationBuilder UseDataInitializer(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                #region 学生种子信息
                if (dbcontext.Students.Any())
                {
                    return builder; // 数据已经初始化了
                }

                var students = new[]
                {
                    new Student { Name = "张三", Major = MajorEnum.ComputerScience,
                        Email = "zhangsan@52abp.com", EnrollmentDate = DateTime.Parse("2016-09-01")},
                    new Student { Name = "李四", Major = MajorEnum.Math,
                        Email = "lisi@52abp.com", EnrollmentDate = DateTime.Parse("2017-09-01")},
                    new  Student{ Name = "王五", Major = MajorEnum.ElectronicCommerce,
                        Email = "wangwu@52abp.com", EnrollmentDate = DateTime.Parse("2012-09-01") }
                };

                foreach (Student item in students)
                {
                    dbcontext.Students.Add(item);
                }
                dbcontext.SaveChanges();

                #endregion


                #region 学院种子数据

                var teachers = new[]
                {
                    new Teacher{ Name = "张老师", HireDate = DateTime.Parse("1995-03-11") },
                    new Teacher{ Name = "王老师", HireDate = DateTime.Parse ("2003-03-11")},
                    new Teacher{ Name = "李老师", HireDate = DateTime.Parse ("1990-03-11")},
                    new Teacher{ Name = "赵老师", HireDate = DateTime.Parse ("1985-03-11")},
                    new Teacher{ Name = "刘老师", HireDate = DateTime.Parse ("2003-03-11")},
                    new Teacher{ Name = "胡老师", HireDate = DateTime.Parse ("2003-03-11")} 
                };

                foreach (var i in teachers)
                {
                    dbcontext.Teachers.Add(i);
                }
                dbcontext.SaveChanges();


                #endregion


                #region 部门种子数据

                var departments = new[]
                {
                    new Department
                    { 
                        Name = "a", Budget = 350000, StartDate = DateTime.Parse("2017-09-01"),
                        TeacherID = teachers.Single(i => i.Name == "刘老师").Id
                    },
                    new Department
                    {
                        Name = "b", Budget = 100000, StartDate = DateTime.Parse("2017-09-01"),
                        TeacherID = teachers.Single(i => i.Name == "赵老师").Id
                    },
                    new Department
                    {
                        Name = "c", Budget = 350000, StartDate = DateTime.Parse("2017-09-01"),
                        TeacherID = teachers.Single(i => i.Name == "胡老师").Id
                    },
                    new Department
                    {
                        Name = "d", Budget = 100000, StartDate = DateTime.Parse("2017-09-01"),
                        TeacherID = teachers.Single(i => i.Name == "王老师").Id
                    },
                };

                foreach (var d in departments)
                    dbcontext.Departments.Add(d);
                dbcontext.SaveChanges();

                #endregion


                #region 课程种子数据

                if (dbcontext.Courses.Any())
                {
                    return builder; // 数据已经初始化了
                }

                var courses = new[]
                {
                    new Course
                    {
                        CourseID = 1050, Title = "数学", Credits = 3,
                        DepartmentID = departments.Single(s => s.Name == "b").DepartmentID
                    },
                    new Course
                    { 
                        CourseID = 4022, Title = "政治", Credits = 3,
                        DepartmentID = departments.Single(s => s.Name == "c").DepartmentID
                    },
                    new Course
                    { 
                        CourseID = 4041, Title = "物理", Credits = 3,
                        DepartmentID = departments.Single(s => s.Name == "b").DepartmentID
                    },
                    new Course
                    { 
                        CourseID = 1045, Title = "化学", Credits = 4,
                        DepartmentID = departments.Single(s => s.Name == "d").DepartmentID
                    },
                    new Course
                    { 
                        CourseID = 3141, Title = "生物", Credits = 4,
                        DepartmentID = departments.Single(s => s.Name == "a").DepartmentID
                    },
                    new Course
                    { 
                        CourseID = 2021, Title = "英语", Credits = 3,
                        DepartmentID = departments.Single(s => s.Name == "a").DepartmentID
                    },
                    new Course
                    { 
                        CourseID = 2042, Title = "历史", Credits = 4,
                        DepartmentID = departments.Single(s => s.Name == "c").DepartmentID
                    }
                };

                foreach (var c in courses)
                {
                    dbcontext.Courses.Add(c);
                }
                dbcontext.SaveChanges();

                #endregion


                #region 办公室分配的种子数据

                var OfficeLocations = new[]
                {
                    new OfficeLocation
                    {
                        TeacherId = teachers.Single(i => i.Name == "刘老师").Id,
                        Location = "X楼"
                    },
                    new OfficeLocation
                    {
                        TeacherId = teachers.Single(i => i.Name == "胡老师").Id,
                        Location = "Y楼"
                    },
                    new OfficeLocation
                    {
                        TeacherId = teachers.Single(i => i.Name == "王老师").Id,
                        Location = "Z楼"
                    }
                };

                foreach (var o in OfficeLocations)
                    dbcontext.OfficeLocations.Add(o);
                dbcontext.SaveChanges();

                #endregion


                #region 为教师分配课程的种子数据
                var courseTeachers = new[]
                {
                    new CourseAssignment
                    {
                        CourseID = courses.Single(c => c.Title == "数学").CourseID,
                        TeacherID = teachers.Single(i => i.Name == "赵老师").Id
                    },
                    new CourseAssignment
                    {
                        CourseID = courses.Single(c => c.Title == "数学").CourseID,
                        TeacherID = teachers.Single(i => i.Name == "王老师").Id
                    },
                    new CourseAssignment
                    {
                        CourseID = courses.Single(c => c.Title == "政治").CourseID,
                        TeacherID = teachers.Single(i => i.Name == "胡老师").Id
                    },
                    new CourseAssignment
                    {
                        CourseID = courses.Single(c => c.Title == "化学").CourseID,
                        TeacherID = teachers.Single(i => i.Name == "王老师").Id
                    },
                    new CourseAssignment
                    {
                        CourseID = courses.Single(c => c.Title == "生物").CourseID,
                        TeacherID = teachers.Single(i => i.Name == "刘老师").Id
                    },
                    new CourseAssignment
                    {
                        CourseID = courses.Single(c => c.Title == "英语").CourseID,
                        TeacherID = teachers.Single(i => i.Name == "刘老师").Id
                    },
                    new CourseAssignment
                    {
                        CourseID = courses.Single(c => c.Title == "物理").CourseID,
                        TeacherID = teachers.Single(i => i.Name == "赵老师").Id
                    },
                    new CourseAssignment
                    {
                        CourseID = courses.Single(c => c.Title == "历史").CourseID,
                        TeacherID = teachers.Single(i => i.Name == "胡老师").Id
                    },
                };

                foreach (var ci in courseTeachers)
                    dbcontext.CourseAssignments.Add(ci);
                dbcontext.SaveChanges();

                #endregion


                #region 课程学生关联种子数据

                var studentCourses = new[]
                {
                    new StudentCourse
                    {
                        StudentID = students.Single(s => s.Name == "张三").Id,
                        CourseID = courses.Single(c => c.Title == "数学").CourseID,
                        Grade = Grade.A
                    }
                };

                foreach (var sc in studentCourses)
                {
                    dbcontext.StudentCourses.Add(sc);
                }
                dbcontext.SaveChanges();

                #endregion


                #region 用户种子数据

                if (dbcontext.Users.Any())
                {
                    return builder;// 数据已经初始化了
                }

                var user = new ApplicationUser
                { 
                    Email = "ltm@ddxc.org",
                    UserName = "ltm@ddxc.org",
                    EmailConfirmed = true,
                    City = "上海" 
                };

                userManager.CreateAsync(user, "bb123456").Wait();// 等待异步方法执行完毕
                dbcontext.SaveChanges();

                var adminRole = "Adimin";
                var role = new IdentityRole { Name = adminRole };
                dbcontext.Roles.Add(role);
                dbcontext.SaveChanges();

                dbcontext.UserRoles.Add(new IdentityUserRole<string>
                {
                    RoleId = role.Id,
                    UserId = user.Id
                });
                dbcontext.SaveChanges();

                #endregion

                return builder;
            }
        }
    }
}
