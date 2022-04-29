using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Application.Teachers.Dtos;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System.Linq;
using System.Linq.Dynamic.Core;

using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Teachers
{
    public class TeacherService : ITeacherService
    {
        private readonly IRepository<Teacher, int> _teacherRepository;

        public TeacherService(IRepository<Teacher, int> teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }
        public async Task<PagedResultDto<Teacher>> GetPagedTeacherList(GetTeacherInput input)
        {
            var query = _teacherRepository.GetAll();

            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText));
            }

            //统计查询数据的总条数，用于分页计算总页数
            var count = query.Count();
            //根据需求进行排序，然后进行分页逻辑的计算
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            //将查询结果转换为List集合，加载到内存中
            var models = await query.Include(a => a.OfficeLocation)
                .Include(a => a.CourseAssignments)
                    .ThenInclude(a => a.Course)
                    .ThenInclude(a => a.StudentCourses)
                    .ThenInclude(a => a.Student)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                    .ThenInclude(i => i.Department)
                .AsNoTracking().ToListAsync();

            var dtos = new PagedResultDto<Teacher>
            {
                TotalCount = count,
                CurrentPage = input.CurrentPage,
                MaxResultCount = input.MaxResultCount,
                Data = models,
                FilterText = input.FilterText,
                Sorting = input.Sorting
            };
            return dtos;
        }
    }
}
