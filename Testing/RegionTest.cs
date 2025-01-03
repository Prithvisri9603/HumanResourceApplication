﻿using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

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

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace Testing

{

    public class RegionTest

    {

        private readonly Mock<IRegionRepository> _mockRegionRepository;

        private readonly Mock<IValidator<RegionDTO>> _mockValidator;

        private readonly RegionController _controller;

        public RegionTest()

        {

            _mockRegionRepository = new Mock<IRegionRepository>();

            _mockValidator = new Mock<IValidator<RegionDTO>>();

            _controller = new RegionController(_mockRegionRepository.Object, _mockValidator.Object);

        }

        #region Get All Region

        [Fact]

        public async Task GetAllRegion_ReturnsOk_WhenRegionExist()

        {

            //Arrange

            var mockRegion = new List<RegionDTO>

            {

                new RegionDTO { RegionId = 10, RegionName = "Europe"},

                new RegionDTO { RegionId= 30, RegionName = "Asia"}

            };

            _mockRegionRepository.Setup(repo => repo.ListAllRegion()).ReturnsAsync(mockRegion);

            // Act

            var result = await _controller.ListAllRegion();

            // Assert: Check the response type

            var okResult = Assert.IsType<OkObjectResult>(result);

            // Assert: Check the response value

            //var regions = Assert.IsType<List<RegionDTO>>(okResult);

            Assert.Equal(mockRegion, okResult.Value);

        }


        //[Fact]


        [Fact]

        public async Task GetAllRegion_ReturnsNotFound_WhenNoRegionExist()

        {

            // Arrange: Simulate no regions in the repository

            _mockRegionRepository.Setup(repo => repo.ListAllRegion()).ReturnsAsync(new List<RegionDTO>());

            // Act: Call the controller method

            var result = await _controller.ListAllRegion();

            // Assert: Verify NotFoundObjectResult is returned

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Assert: Verify the error message

            var actualMessage = notFoundResult.Value.GetType().GetProperty("Message")?.GetValue(notFoundResult.Value, null)?.ToString();

            Assert.Equal("No regions found.", actualMessage);

        }


        #endregion

        


        

        #region get regionbyid

        [Fact]

        public async Task GetRegionId_ReturnsOk_WhenRegionExists()

        {

            var mockRegion = new RegionDTO { RegionId = 10, RegionName = "Europe" };

            _mockRegionRepository.Setup(repo => repo.GetRegionById(10)).ReturnsAsync(mockRegion);


            var result = await _controller.GetRegionById(10);


            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(mockRegion, okResult.Value);

        }

        [Fact]

        public async Task GetRegionById_ReturnsNotFound_WhenRegionDoesNotExist()

        {

            // Arrange

            _mockRegionRepository.Setup(repo => repo.GetRegionById(70)).ReturnsAsync((RegionDTO)null);

            // Act

            var result = await _controller.GetRegionById(70);

            // Assert

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            var actualMessage = notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null)?.ToString();

            Assert.Equal("Region not found.", actualMessage);

        }


        #endregion




       

        #region delete region

        [Fact]

        public async Task DeleteRegionById_ReturnsOk_WhenRegionExists()

        {

            // Arrange

            decimal regionId = 10;

            var region = new RegionDTO { RegionId = regionId, RegionName = "Europe" };

            //_mockRegionRepository.Setup(repo => repo.GetRegionById(regionId))

            //                     .ReturnsAsync(region);

            //_mockRegionRepository.Setup(repo => repo.GetRegionById(regionId)).ReturnsAsync(new RegionDTO());

            _mockRegionRepository.Setup(repo => repo.DeleteRegionById(regionId))

                                 .Returns(Task.CompletedTask);

            // Act

            // var result = await _controller.GetRegionById(regionId);

            var result1 = await _controller.DeleteById(regionId);

            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result1);

            Assert.Equal("Region deleted successfully", okResult.Value);

        }

        [Fact]

        public async Task DeleteRegionById_ReturnsBadRequest_WhenExceptionOccurs()

        {

            // Arrange

            _mockRegionRepository.Setup(repo => repo.DeleteRegionById(10)).ThrowsAsync(new System.Exception("Error deleting"));

            // Act

            var result = await _controller.DeleteById(10);

            // Assert

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Error deleting", badRequestResult.Value);

        }


        #endregion

    }

}

