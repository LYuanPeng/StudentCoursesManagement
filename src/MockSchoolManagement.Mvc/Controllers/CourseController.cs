using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels.Course;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<Department, int> _deparmentRepository;
        private readonly IRepository<CourseAssignment, int> _courseAssignmentRepository;

        public CourseController(ICourseService courseService,
            IRepository<Course, int> courseRepository,
            IRepository<Department, int> deparmentRepository,
            IRepository<CourseAssignment, int> courseAssignmentRepository)
        {
            _courseService = courseService;
            _courseRepository = courseRepository;
            _deparmentRepository = deparmentRepository;
            _courseAssignmentRepository = courseAssignmentRepository;
        }

        //不填写[HttpGet]默认为处理GET请求
        public async Task<ActionResult> Index(GetCourseInput input)
        {
            var models = await _courseService.GetPaginatedResult(input);
            return View(models);
        }


        #region 添加课程
        public ActionResult Create()
        {
            var dtos = DepartmentsDropDownList();
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos
            };

            //将DepartmentsDropDownList()方法的SelectList返回值添加到courseCreateViewModel中
            //传递到视图中
            return View(courseCreateViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CourseCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course
                {
                    CourseID = input.CourseID,
                    Title = input.Title,
                    Credits = input.Credits,
                    DepartmentID = input.DepartmentID
                };

                await _courseRepository.InsertAsync(course);

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var models = _deparmentRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "DepartmentID", "Name", selectedDepartment);
            return dtos;
        }
        #endregion


        #region 编辑课程

        public IActionResult Edit(int? courseId)
        {
            if (!courseId.HasValue)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试。";
                return View("NotFound");
            }

            var course = _courseRepository.FirstOrDefault(a => a.CourseID == courseId);

            if (course == null)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试。";
                return View("NotFound");
            }

            //将学院列表中选中的值修改为true
            var dtos = DepartmentsDropDownList(course.DepartmentID);
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos,
                CourseID = course.CourseID,
                Credits = course.Credits,
                Title = course.Title,
                DepartmentID = course.DepartmentID
            };
            return View(courseCreateViewModel);
        }

        [HttpPost]
        public IActionResult Edit(CourseCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var course = _courseRepository.FirstOrDefault(a => a.CourseID == input.CourseID);

                if (course != null)
                {
                    course.CourseID = input.CourseID;
                    course.Credits = input.Credits;
                    course.Title = input.Title;
                    course.DepartmentID = input.DepartmentID;
                    _courseRepository.Update(course);
                }
                else
                {
                    ViewBag.ErrorMessage = $"课程编号{input.CourseID}的信息不存在，请重试。";
                    return View("NotFound");
                }
            }
            return View(input);
        }
        #endregion


        #region 课程详情

        public async Task<ViewResult> Details(int courseId)
        {
            var course = await _courseRepository.GetAll()
                .Include(a => a.Department).FirstOrDefaultAsync(a => a.CourseID == courseId);

            //判断学生信息是否存在
            if (course == null)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试。";
                return View("NotFound");
            }

            return View(course);
        }


        #endregion


        #region 删除课程

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _courseRepository.FirstOrDefaultAsync(a => a.CourseID == id);
            if (model == null)
            {
                ViewBag.ErrorMessage = $"课程编号{id}的信息不存在，请重试。";
                return View("NotFound");
            }
            await _courseAssignmentRepository.DeleteAsync(a => a.CourseID == model.CourseID);
            await _courseRepository.DeleteAsync(a => a.CourseID == id);
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
