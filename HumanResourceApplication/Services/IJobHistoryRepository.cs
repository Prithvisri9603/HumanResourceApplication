using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Services
{
    public interface IJobHistoryRepository
    {
        //GET
        Task<TimeSpan?> FindExperienceOfEmployees(decimal id);

        Task<List<JobHistoryDTO>> GetAllJobHistory();

        //Task<List<JobHistoryDTO>> EmployeesWithLessThanOneYearExp(decimal id);

        Task<TimeSpan?> GetTotalExperienceByEmployeeIdAsync(int empId);
        //POST
        Task AddJobHistory( decimal empId, DateOnly startDate, string jobId, decimal deptId );


        //PUT
        Task UpdateJobHistory(decimal id, DateOnly newStartDate, DateOnly newEndDate);
        
    }

}
