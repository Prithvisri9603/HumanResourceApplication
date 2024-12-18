using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Services
{
    public interface IJobRepository
    {
        
        Task<List<JobDTO>> GetAllJobs();
        Task AddJob(JobDTO jobDTO);

        Task UpdateJob(int jobId,JobDTO jobDTO);

        // Task UpdateJobMinAndMaxSalary(decimal newMin, decimal newMax);

    }
}
