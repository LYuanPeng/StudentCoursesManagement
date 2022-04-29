using MockSchoolManagement.Application.Departments.Dtos;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Departments
{
    public interface IDepartmentsService
    {
        /// <summary>
        /// 获取学院的分页信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<Department>> GetPagedDepartmentList(GetDepartmentInput input);
    }
}
