using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentCourseId { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Course Course { get; set; }
        public Student Student { get; set; }

        [DisplayFormat(NullDisplayText = "无成绩")]
        public Grade? Grade { get; set; }
    }

    public enum Grade
    { 
        A, B, C, D, F
    }
}
