using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using HumanResourceApplication.Controllers;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Testing
{
    public class Job_HistoryTest
    {
        private readonly Mock<IJobHistoryRepository> _mockRepo;
        private readonly Mock<IValidator<JobHistoryDTO>> _mockValidator;
        private readonly JobHistoryController _controller;

        public Job_HistoryTest()
        {
            _mockRepo = new Mock<IJobHistoryRepository>();
            _mockValidator = new Mock<IValidator<JobHistoryDTO>>();
            _controller = new JobHistoryController(_mockRepo.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task GetAllJobHistory_ReturnsOk_WhenJobHistories()
        {
            var mockJobHistory = new List<JobHistoryDTO>
            { 
                new JobHistoryDTO { EmployeeId = 101 , StartDate = new DateOnly(2018,07,01) , EndDate = new DateOnly(2024,12,23)},
                new JobHistoryDTO { EmployeeId = 102, StartDate = new DateOnly(2019,07,01), EndDate = new DateOnly(2014,12,20)}
            };

            _mockRepo.Setup(repo => repo.GetAllJobHistory()).ReturnsAsync(mockJobHistory);
            var result = await _controller.GetAllJobHistory();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mockJobHistory, okResult.Value);
        }

        [Fact]
        public async Task GetAllJobHistory_ReturnsNotFound_WhenNoJobHistoriesExist()
        {
            _mockRepo.Setup(repo => repo.GetAllJobHistory()).ReturnsAsync(new List<JobHistoryDTO>());
            var result = await _controller.GetAllJobHistory();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var actualMessage = notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null)?.ToString();

            Assert.Equal("No job history found.", actualMessage);

        }

        [Fact]
        public async Task AddJobHistory_ReturnsOk_WhenValidationSucceeds()
        {
            //Arrange 
            var empId = 101;
            var startDate = new DateOnly(2018, 07, 01);
            var jobId = "SA_REP";
            var deptId = 90;

            var jobHistoryDTO = new JobHistoryDTO
            {
                EmployeeId = empId,
                StartDate = startDate,
                JobId = jobId,
                DepartmentId = deptId
            };
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<JobHistoryDTO>(), default))
                .ReturnsAsync(new ValidationResult());

            _mockRepo.Setup(repo => repo.AddJobHistory(empId, startDate, jobId, deptId)).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.AddJobHistory(empId,startDate,jobId, deptId);

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Record Created Successfully", okResult.Value);
        }


        [Fact]
        public async Task AddJobHistory_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var empId = 101;
            var startDate = new DateOnly(2018, 07, 01);
            var jobId = "SA_REP";
            var deptId = 10;

            var jobHistory = new JobHistoryDTO
            {
                EmployeeId = empId,
                StartDate = startDate,
                JobId = jobId,
                DepartmentId = deptId
            };

            var validationErrors = new List<ValidationFailure>
                {
                     new ValidationFailure("JobId", "JobId is required."),
                    new ValidationFailure("DepartmentId", "DepartmentId must be greater than 0.")
                };

            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<JobHistoryDTO>(), default))
                .ReturnsAsync(new ValidationResult(validationErrors)); 

            // Act
            var result = await _controller.AddJobHistory(empId, startDate, jobId, deptId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);

            Assert.Equal(2, errors.Count());
            Assert.Contains(errors, e => e.PropertyName == "JobId" && e.ErrorMessage == "JobId is required.");
            Assert.Contains(errors, e => e.PropertyName == "DepartmentId" && e.ErrorMessage == "DepartmentId must be greater than 0.");
        }


    }
}
