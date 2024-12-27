using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace HumanResourceApplication.Services
{
    public class JobServices : IJobRepository
    {
        private readonly HrContext _context;
        private readonly IMapper _mapper;

        public JobServices(HrContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        #region Fetch methods

        //Display all jobs as list
        public async Task <List<JobDTO>> GetAllJobs()
        {
            var jobs = await _context.Jobs.ToListAsync();
            var jobsList = _mapper.Map<List<JobDTO>>(jobs);
            return jobsList;

        }

        public async Task<JobDTO> GetJobById(string jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);

            if (job == null)
            {
                return null;
            }
            var jobDTO = _mapper.Map<JobDTO>(job);
            return jobDTO;
        }
        #endregion


        #region Add new Job
        //Add new Job
        public async Task AddJob(JobDTO jobDTO)
        {
            var job = _mapper.Map<Job>(jobDTO); 
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
        }

        #endregion


        #region Update Job details including MinMax Salary
        //Update with new Job details
        public async Task UpdateJob(string jobId, JobDTO jobDTO)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            
            if(job != null)
            {
                _context.Entry(job).CurrentValues.SetValues(jobDTO);
            }
            await _context.SaveChangesAsync();

        }
        


        public async Task UpdateJobMinAndMaxSalary(string jobID,decimal newMin, decimal newMax)
        {
            var job = await _context.Jobs.FindAsync(jobID);

            if (job != null)
            {
                job.MinSalary = newMin;
                job.MaxSalary = newMax;

                await _context.SaveChangesAsync();
            }

        }

        #endregion


    }
}
