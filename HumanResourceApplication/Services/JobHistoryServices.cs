using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceApplication.Services
{
    public class JobHistoryServices : IJobHistoryRepository
    {
        private readonly HrContext _context;
        private readonly IMapper _mapper;

        public JobHistoryServices(HrContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<JobHistoryDTO>>GetAllJobHistory()
        {
            var jobHistory = await _context.JobHistories.ToListAsync();
            var joblist = _mapper.Map<List<JobHistoryDTO>>(jobHistory);
            return joblist;
        }

        public async Task<JobHistoryDTO> GetJobHistoryByEmployeeIdAndJob(decimal empid, string jobId)
        {
            // Assuming JobHistory is a DbSet in your DbContext
            var jobHistory = await _context.JobHistories
                                           .FirstOrDefaultAsync(jh => jh.EmployeeId == empid && jh.JobId == jobId);

            if (jobHistory == null)
            {
                return null;
            }

            // Map the JobHistory model to JobHistoryDTO (using AutoMapper or manual mapping)
            return new JobHistoryDTO
            {
                EmployeeId = jobHistory.EmployeeId,
                StartDate = jobHistory.StartDate,
                JobId = jobHistory.JobId,
                DepartmentId = jobHistory.DepartmentId
            };
        }

        public async Task<TimeSpan?> FindExperienceOfEmployees(decimal id)
        {
            var jobHistories = await _context.JobHistories.
                         Where(j => j.EmployeeId == id).ToListAsync();

            if(!jobHistories.Any())
            {
                return null;
            }

            TimeSpan totalExperience = jobHistories.Aggregate(TimeSpan.Zero, (sum, job) =>
            sum + (job.EndDate.ToDateTime(TimeOnly.MinValue) - job.StartDate.ToDateTime(TimeOnly.MinValue)));

            return totalExperience;
        }
        public async Task AddJobHistory(decimal empId, DateOnly startDate, string jobId, decimal deptId)
        {
            // Check if the employee exists
            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == empId);
            if (!employeeExists)
            {
                throw new Exception($"Employee with ID {empId} does not exist.");
            }

            // Check if the job history entry already exists
            var existingJobHistory = await _context.JobHistories
                .AnyAsync(jh => jh.EmployeeId == empId && jh.StartDate == startDate);
            if (existingJobHistory)
            {
                throw new Exception("Job history entry already exists for the given employee and start date.");
            }

            // Add new job history entry
            var jobHistory = new JobHistory
            {
                EmployeeId = empId,
                StartDate = startDate,
                JobId = jobId,
                DepartmentId = deptId,
                EndDate = DateOnly.FromDateTime(DateTime.Now) // Assuming current date as end date
            };

            _context.JobHistories.Add(jobHistory);
            await _context.SaveChangesAsync();
        }

        //public async Task AddJobHistory(decimal empId, DateOnly startDate, string jobId, decimal deptId)
        //{

        //        var jobHistory = new JobHistory
        //        {
        //            EmployeeId = empId,
        //            StartDate = startDate,
        //            EndDate = DateOnly.MinValue,
        //            JobId = jobId,
        //            DepartmentId = deptId
        //        };
        //        _context.JobHistories.Add(jobHistory);
        //        await _context.SaveChangesAsync();

        //}

        public async Task UpdateJobHistory(decimal id,DateOnly newStartDate, DateOnly newEndDate)
        {
            var res = _context.JobHistories.Where(jb => ( jb.EmployeeId == id && jb.StartDate == newStartDate )).FirstOrDefault();
            if (res != null)
            {
                res.EndDate = newEndDate;
                await _context.SaveChangesAsync();
            }
        }

        /*
        public async Task<List<JobHistoryDTO>> EmployeesWithLessThanOneYearExp(decimal id)
        {
            var jobHisList = await _context.JobHistories.Where(jb => jb.EmployeeId == id).ToListAsync();

            TimeSpan totalExperience = TimeSpan.Zero;

            foreach (var job in jobHisList)
            {
                var endDate = job.EndDate.ToDateTime(TimeOnly.MinValue); 
                var startDate = job.StartDate.ToDateTime(TimeOnly.MinValue);
                totalExperience += (endDate - startDate);
            }

            // Convert the total experience to years, months, and days
            int years = totalExperience.Days / 365;
            int months = (totalExperience.Days % 365) / 30;
            int days = (totalExperience.Days % 365) % 30;

        }
        */
        public async Task<TimeSpan?> GetTotalExperienceByEmployeeIdAsync(int empId)
        {
            var jobHistories = await _context.JobHistories
                                             .Where(j => j.EmployeeId == empId)
                                             .ToListAsync();

            if (!jobHistories.Any())
            {
                return null;
            }

            // Calculate the total experience by summing up the durations

            TimeSpan totalExperience = jobHistories
        .Aggregate(TimeSpan.Zero, (sum, job) =>
            sum + (job.EndDate.ToDateTime(TimeOnly.MinValue) - job.StartDate.ToDateTime(TimeOnly.MinValue))
        );

            return totalExperience;
        }


    }
}
