namespace MockSchoolManagement.Application.Dtos
{
    public class GetStudentInput : PagedSortedAndFilterInput
    {
        public GetStudentInput()
        {
            Sorting = "Id";
        }
    }
}
