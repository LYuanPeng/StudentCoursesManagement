using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Students;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Application.Students.Dtos;

namespace MockSchoolManagement.Controllers
{
    //[AllowAnonymous]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IRepository<Student, int> _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStudentService _studentService;

        //使用构造函数注入的方式注入IStudentRepository
        public HomeController(IRepository<Student, int> studentRepository, IWebHostEnvironment webHostEnvironment,
            IStudentService studentService)
        {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
            _studentService = studentService;
        }

        //返回学生的名字
        [Route("")]
        [Route("/")]
        public async Task<IActionResult> Index(GetStudentInput input)
        {
            //获取分页结果
            var dtos = await _studentService.GetPaginatedResult(input);
            return View(dtos);
        }


        //public ObjectResult Details()
        //{
        //    Student model = _studentRepository.GetStudent(1);
        //    //return Json(model);
        //    return new ObjectResult(model);
        //}

        //?使路由模板中的id参数为可选，如果要使它为必选，删除?即可
        [Route("Home/Details/{id?}")]
        //[Route("")]
        // [Route("/{id?}")]
        public ViewResult Details(int id)
        {
            Student student = _studentRepository.FirstOrDefault(s => s.Id == id);

            // 使用ViewData将PageTitle和Student模型传递给View
            //ViewData["PageTitle"] = "Student Details";
            //ViewData["Student"] = model;
            //return View();

            // 将PageTitle和Student模型对象存储再ViewBag
            //我们正再使用动态属性PageTitle和Student
            //ViewBag.PageTitle = "学生详情";
            //ViewBag.Student = model;
            //return View();

            //ViewBag.PageTitle = "学生详情";
            //return View(model);

            // 判断学生信息是否存在
            if (student == null)
            {
                //Response.StatusCode = 404;
                //return View("StudentNotFound", id);
                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试。";
                return View("NotFound");
            }

            //实例化HomeDetailsViewModel并存储Student详细信息和PageTitle
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Student = student,
                PageTitle = "学生详情"
            };
            //将ViewModel对象传递给View()方法
            return View(homeDetailsViewModel);
        }


        #region 创建学生
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                //如果传入模型对象中的Photo属性不为null，并且Count>0，则表示用户至少选择一个要上传的文件 
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    // 循环每个选定的文件
                    foreach (IFormFile photo in model.Photos)
                    {
                        //必须将图片文件上传到wwwroot的images文件夹中
                        //而要获取wwwroot文件夹的路径，我们需要注入ASP.NET Core提供的WebHostEnvironment服务
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                        //为了确保文件名是唯一的，我们在文件名后附加一个新的GUID值和一个下划线
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        //使用IFormFile接口提供的CopyTo()方法将文件复制到wwwroot/images/avatars文件夹
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                    
                }
                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Major = model.Major,
                    EnrollmentDate = model.EnrollmentDate,
                    // 将文件名保存在Student对象的PhotoPath属性中
                    //它将被保存到数据库Students的表中
                    PhotoPath = uniqueFileName
                };
                _studentRepository.Insert(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });

            }
            return View();
        }
        #endregion

        #region 编辑学生
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Student student = _studentRepository.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", id);
            }
            StudentEditViewModel studentEditViewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Major = student.Major,
                ExistingPhotoPath = student.PhotoPath,
                EnrollmentDate = student.EnrollmentDate
            };
            return View(studentEditViewModel);
        }

        //通过模型绑定，作为操作方法的参数
        //StudentEditViewModel会接收来自POST请求的Edit表单数据
        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            //检查提供的数据是否有效，如果没有通过验证，需要重新编辑学生信息
            //这样用户就可以更正并重新提交编辑表单
            if (ModelState.IsValid)
            {
                //从数据库中查询正在编辑的学生信息
                Student student = _studentRepository.FirstOrDefault(s => s.Id == model.Id);
                // 从模型对象中的数据更新student对象
                student.Name = model.Name;
                student.Email = model.Email;
                student.Major = model.Major;
                student.EnrollmentDate = model.EnrollmentDate;

                //如果用户想要更改图片，那么可以上传新图片文件，它会被模型对象上的Photos属
                //性接收
                //如果用户没有上传图片，那么我们会保留现有的图片文件信息
                //因为兼容了多图上传，所以将这里的!=null判断修改为判断Photos的总数是否
                //大于0
                if (model.Photos.Count > 0)
                {
                    //如果上传了新的图片，则必须显示新的图片信息
                    //因此我们会检查当前学生信息中是否有图片，如果有，则会删除它
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
                            "images","avatars",model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    //我们将新的图片文件保存到wwwroot/images/avatars文件夹中，并且会更新
                    //Student对象中的PhotoPath属性，最终都会将它们保存到数据库中
                    student.PhotoPath = ProcessUploadedFile(model);
                }
                //调用仓储服务中的Update()方法，保存Studnet对象中的数据，更新数据库表中的信息
                Student updatedstudent = _studentRepository.Update(student);
                return RedirectToAction("index");
            }
            return View(model);
        }
        #endregion

        #region 删除学生
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var student = await _studentRepository.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的学生信息";
                return View("NotFound");
            }

            await _studentRepository.DeleteAsync(s => s.Id == id);
            return RedirectToAction("index");
        }
        #endregion



        /// <summary>
        /// 将图片保存到指定的路径中，并返回唯一的文件名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessUploadedFile(StudentCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photos.Count > 0)
            {
                foreach (var photo in model.Photos)
                {
                    //必须将图片文件上传到wwwroot的images/avatars文件夹中
                    //而要获取wwwroot文件夹的路径，我们需要注入ASP.NET Core提供的webHostEnvironment服务
                    //通过webHostEnvironment服务去获取wwwroot文件夹的路径
                    string  uploadsFolder  =  Path.Combine(_webHostEnvironment.WebRootPath,"images", "avatars");
                    //为了确保文件名是唯一的，我们在文件名后附加一个新的GUID值和一个下划线
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder + uniqueFileName);
                    //因为使用了非托管资源，所以需要手动进行释放
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        //使用IFormFile接口提供的CopyTo()方法将文件复制到
                        //wwwroot/images/avatars文件夹
                        //photo.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }


        public async Task<ActionResult> About()
        {
            //获取IQueryable类型的Student，然后通过student.EnrollmentDate进行分组
            var data = from student in _studentRepository.GetAll()
                       group student by student.EnrollmentDate into dateGroup

                       select new EnrollmentDateGroupDto()
                       {
                           EnrollmentDate = dateGroup.Key,
                           StudentCount = dateGroup.Count()
                       };

            var dtos = await data.AsNoTracking().ToListAsync();
            return View(dtos);
        }
    }
}
