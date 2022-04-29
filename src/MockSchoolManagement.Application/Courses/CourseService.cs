using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System.Linq;
using System.Linq.Dynamic.Core;

using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course, int> _courseRepository;

        public CourseService(IRepository<Course, int> courseRepositor)
        {
            _courseRepository = courseRepositor;
        }

        public async Task<PagedResultDto<Course>> GetPaginatedResult(GetCourseInput input)
        {
            var query = _courseRepository.GetAll();
            //模糊查询
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(p => p.Title.Contains(input.FilterText));
                input.CurrentPage = 1;
            }

            //统计查询数据的总条数，用于分页计算总页数
            var count = query.Count();
            //根据需求进行排序，然后进行分页逻辑的计算
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);

            //将查询结果转换未List集合，加载到内存中
            var models = await query.Include(a => a.Department).AsNoTracking().ToListAsync();

            var dtos = new PagedResultDto<Course>
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
