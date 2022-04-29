using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Courses
{
    public interface ICourseService
    {
        Task<PagedResultDto<Course>> GetPaginatedResult(GetCourseInput input);
    }
}
