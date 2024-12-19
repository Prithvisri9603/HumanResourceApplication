using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceApplication.Services
{
    public class JobHistoryServices : IJobHistoryRepository
    {
        private readonly HrContext _context;

        public JobHistoryServices(HrContext context)
        {
            _context = context;
        }

        public async Task<(int years, int months, int days)> FindExperienceOfEmployees(decimal id)
        {
            var jobHistories = await _context.JobHistories.
                         Where(j => j.EmployeeId == id).ToListAsync();

            if(!jobHistories.Any())
            {
                return (0,0,0);
            }

            int totalDays = 0;

            foreach (var job in jobHistories)
            {
                var startDate = job.StartDate.ToDateTime(TimeOnly.MinValue);
                var endDate = job.EndDate.ToDateTime(TimeOnly.MinValue);
                totalDays += (endDate - startDate).Days;

            }

            int years = totalDays / 365;
            int remainingDays = totalDays % 365;
            int months = remainingDays / 30;
            int days = remainingDays % 30;

            return (years, months, days);

        }
        /*
        public async Task<List<JobHistoryDTO>> EmployeesWithLessThanOneYearExp()
        {


        }

        public async Task AddJobHistory(JobHistoryDTO jobHistoryDTO)
        {

        }

        public async Task UpdateJobHistory(decimal id, DateOnly newEndDate)
        {

        }

        */
    }
}
