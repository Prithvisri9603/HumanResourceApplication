using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Services
{
    public interface IJobRepository
    {
        
        Task<List<JobDTO>> GetAllJobs();
        Task AddJob(JobDTO jobDTO);

        Task UpdateJob(string jobId,JobDTO jobDTO);

        Task UpdateJobMinAndMaxSalary(string jobId,decimal newMin, decimal newMax);

        Task<JobDTO> GetJobById(string jobId);

    }
}
