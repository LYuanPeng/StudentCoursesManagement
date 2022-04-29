using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using MockSchoolManagement.Models;

namespace MockSchoolManagement.ViewModels.Department
{
    public class DepartmentCreateViewModel
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "学院名称")]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "预算")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "成立时间")]
        public DateTime StartDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Display(Name = "负责人")]
        public SelectList TeacherList { get; set; }

        public int? TeacherID { get; set; }

        public Models.Teacher Administrator { get; set; }
    }
}
