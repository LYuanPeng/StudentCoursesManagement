using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Students
{
    public interface IStudentService
    {
        Task<PagedResultDto<Student>> GetPaginatedResult(GetStudentInput input);
    }
}
