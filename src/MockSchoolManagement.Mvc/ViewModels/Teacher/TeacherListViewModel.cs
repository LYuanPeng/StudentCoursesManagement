using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using System.Collections.Generic;

namespace MockSchoolManagement.ViewModels
{
    public class TeacherListViewModel
    {
        public PagedResultDto<Models.Teacher> Teachers { get; set; }
        public List<Models.Course> Courses { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }

        /// <summary>
        /// 选中的教师ID
        /// </summary>
        public int SelectedId { get; set; }

        /// <summary>
        /// 选中的课程ID
        /// </summary>
        public int SelectedCourseId { get; set; }
    }
}
