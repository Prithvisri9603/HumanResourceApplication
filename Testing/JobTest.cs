using FluentValidation;
using FluentValidation.Results;
using HumanResourceApplication.Controllers;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using HumanResourceApplication.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public class JobTest
    {
        private readonly Mock<IJobRepository> _repo;
        private readonly Mock<IValidator<JobDTO>> _mockValidator;
        private readonly JobController _controller;
        public JobTest()
        {
            _repo = new Mock<IJobRepository>();
            _mockValidator = new Mock<IValidator<JobDTO>>();
            _controller = new JobController(_repo.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task GetAllJobs_ReturnsOk_WhenJobsExists()
        {
            //Arrange
            var mockJobs = new List<JobDTO>
            {
                new JobDTO { JobId = "AC_ACCOUNT", JobTitle = "Public Accountant", MinSalary = 4200, MaxSalary=9000 },
                new JobDTO { JobId = "AC_MGR", JobTitle = "Accounting Manager", MinSalary = 8200, MaxSalary=16000}
            };

            _repo.Setup(repo => repo.GetAllJobs()).ReturnsAsync(mockJobs);

            //Act
            var result = await _controller.GetJobs();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mockJobs, okResult.Value);

        }

        [Fact]
        public async Task GetAllJobs_ReturnsNotFound_WhenNoJobExist()
        {
            //Arrange
            _repo.Setup(repo => repo.GetAllJobs()).ReturnsAsync(new List<JobDTO>());

            //Act
            var result = await _controller.GetJobs();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var actualMessage = notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null)?.ToString();
            Assert.Equal("No job found", actualMessage);
        }

        [Fact]
        public async Task GetJobById_ReturnsOk_WhenJobExists()
        {
            // Arrange
            var jobId = "AC_ACCOUNT";
            var expectedJob = new JobDTO
            {
                JobId = jobId,
                JobTitle = "Public Accountant",
                MinSalary = 4200,
                MaxSalary = 9000
            };

            _repo.Setup(repo => repo.GetJobById(jobId)).ReturnsAsync(expectedJob);

            // Act
            var result = await _controller.GetJobById(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualJob = Assert.IsType<JobDTO>(okResult.Value);

            Assert.Equal(expectedJob.JobId, actualJob.JobId);
            Assert.Equal(expectedJob.JobTitle, actualJob.JobTitle);
            Assert.Equal(expectedJob.MinSalary, actualJob.MinSalary);
            Assert.Equal(expectedJob.MaxSalary, actualJob.MaxSalary);
        }
        [Fact]
        public async Task GetJobById_ReturnsNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var jobId = "JOB123";
            _repo.Setup(repo => repo.GetJobById(jobId)).ReturnsAsync((JobDTO)null);

            // Act
            var result = await _controller.GetJobById(jobId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var actualMessage = notFoundResult.Value?.ToString();

            Assert.Equal($"JobID {jobId} is not found", actualMessage);
        }

       


    }
}
