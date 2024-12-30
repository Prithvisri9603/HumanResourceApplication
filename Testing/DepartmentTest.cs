using FluentValidation;
using FluentValidation.Results;
using HumanResourceApplication.Controllers;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public class DepartmentTest
    {
        private readonly Mock<IDepartmentRepository> _repo;
        private readonly Mock<IValidator<DepartmentDTO>> _mockValidator;
        private readonly DepartmentController _controller;

        public DepartmentTest()
        {
            _repo = new Mock<IDepartmentRepository>();
            _mockValidator = new Mock<IValidator<DepartmentDTO>>();
            _controller = new DepartmentController(_repo.Object, _mockValidator.Object);
        }



        [Fact]
        public async Task AddDepartment_ReturnsOk_WhenValidationSucceeds()
        {
            // Arrange
            var department = new DepartmentDTO
            {
                DepartmentId = 10,
                DepartmentName = "Administration",
                ManagerId = 200,
                LocationId = 1700
            };

            // Mock validation to succeed
            _mockValidator.Setup(v => v.Validate(department))
                          .Returns(new FluentValidation.Results.ValidationResult()); // Validation passes

            // Mock repository behavior
            _repo.Setup(repo => repo.AddDepartment(department)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddDepartment(department);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Record created successfully", okResult.Value);
        }



        [Fact]
        public async Task AddDepartment_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var department = new DepartmentDTO
            {
                DepartmentId = 0, // Invalid: DepartmentId is empty (default for decimal)
                DepartmentName = "Administration",
                ManagerId = null, // Invalid: ManagerId is null
                LocationId = null // Invalid: LocationId is null
            };

            // Mock validation to fail
            var validationFailures = new List<ValidationFailure>
    {
        new ValidationFailure("DepartmentId", "Department Id is required"),
        new ValidationFailure("ManagerId", "'Manager Id' must not be empty."),
        new ValidationFailure("LocationId", "'Location Id' must not be empty.")
    };

            _mockValidator.Setup(v => v.Validate(department))
                          .Returns(new FluentValidation.Results.ValidationResult(validationFailures)); // Validation fails

            // Act
            var result = await _controller.AddDepartment(department);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation failed", badRequestResult.Value); // Matches the controller's BadRequest message
        }



        [Fact]
        public async Task DeletedepartmentById_When_Validation_Fails()
        {
            // Arrange
            _repo.Setup(repo => repo.DeleteDepartmentById(10)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDepartmentById(10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Record deleted succesfully", okResult.Value);
        }


        [Fact]
        public async Task DeleteCountryById_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            _repo.Setup(repo => repo.DeleteDepartmentById(0)).ThrowsAsync(new System.Exception("Error deleting country."));

            // Act
            var result = await _controller.DeleteDepartmentById(0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error deleting department.", badRequestResult.Value);



        }
    }
}

