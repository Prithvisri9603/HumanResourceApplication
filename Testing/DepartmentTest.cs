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



        #region GetDepartmentByEmp Tests
        [Fact]
        public async Task GetDepartment_ShouldReturnOk_WhenDepartmentsExist()
        {
            // Arrange
            var departments = new List<DepartmentDTO>
            {
                new DepartmentDTO { DepartmentName = "HR" },
                new DepartmentDTO { DepartmentName = "IT" }
            };
            _repo
               .Setup(repo => repo.GetDepartment())
                .ReturnsAsync(departments);
            // Act
            var result = await _controller.GetDepartment();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<DepartmentDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAllDeptDetailsForEmp_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            decimal employeeId = 12345;
            _repo .Setup(repo => repo.GetAllDeptDetailsForEmp(employeeId)).ThrowsAsync(new Exception("Database error")); // Simulate an exception

            // Act
            var result = await _controller.GetAllDeptDetailsForEmp(employeeId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;

            // Use dynamic or cast to the anonymous type
            Assert.NotNull(returnValue);
            Assert.Equal("An error occurred.", returnValue.GetType().GetProperty("Message")?.GetValue(returnValue)?.ToString());
        }



        #endregion




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

