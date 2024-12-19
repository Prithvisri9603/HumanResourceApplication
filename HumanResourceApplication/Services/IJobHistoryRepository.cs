using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Services
{
    public interface IJobHistoryRepository
    {
        //GET
        Task<(int years, int months, int days)> FindExperienceOfEmployees(decimal id);
        /*
        Task<List<JobHistoryDTO>> EmployeesWithLessThanOneYearExp();


        //POST
        Task AddJobHistory( JobHistoryDTO jobHistoryDTO );


        //PUT
        Task UpdateJobHistory(decimal id, DateOnly newEndDate);
        */
    }

}
