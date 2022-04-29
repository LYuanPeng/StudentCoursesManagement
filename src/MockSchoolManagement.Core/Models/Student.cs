using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public MajorEnum? Major { get; set; }

        public string Email { get; set; }

        public string PhotoPath { get; set; }


        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime EnrollmentDate { get; set; }

        /// <summary>
        /// 导航属性
        /// </summary>
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
