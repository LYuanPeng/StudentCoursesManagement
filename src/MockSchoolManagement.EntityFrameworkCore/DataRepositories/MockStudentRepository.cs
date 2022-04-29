using MockSchoolManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace MockSchoolManagement.DataRepositories
{
    public class MockStudentRepository : IStudentRepository
    {
        private List<Student> _studentList;
        public MockStudentRepository()
        {
            _studentList = new List<Student>()
            {
                new Student() {Id = 1,Name = "张三",Major = MajorEnum.ComputerScience, Email = "zhangsan@52abp.com" },
                new Student() {Id = 2,Name = "李四",Major = MajorEnum.ElectronicCommerce,Email = "lisi@52abp.com" },
                new Student() {Id = 3,Name = "赵六",Major = MajorEnum.Math,Email = "zhaoliu@52abp.com" }
            };
        }

        public Student GetStudentById(int id)
        {
            // 写逻辑实现查询学生详情信息
            return _studentList.FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _studentList;
        }

        public Student Insert(Student student)
        {
            student.Id = _studentList.Max(a => a.Id) + 1;
            _studentList.Add(student);
            return student;
        }
    

        public Student Update(Student updateStudent)
        {
            Student student = _studentList.FirstOrDefault(s => s.Id == updateStudent.Id);
            if (student != null)
            {
                student.Name = updateStudent.Name;
                student.Email = updateStudent.Email;
                student.Major = updateStudent.Major;
            }
            return student;
        }

        public Student Delete(int id)
        {
            Student student = _studentList.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                _studentList.Remove(student);
            }
            return student;
        }
    }
}
