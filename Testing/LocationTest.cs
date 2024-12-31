using Xunit;
using Moq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using HumanResourceApplication.Controllers;
using HumanResourceApplication.Services;
using HumanResourceApplication.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Testing
{
    public class LocationTest
    {
        private readonly Mock<ILocationRepository> _mockRepo;
        private readonly Mock<IValidator<LocationDTO>> _mockValidator;
        private readonly LocationController _controller;

        public LocationTest()
        {
            _mockRepo = new Mock<ILocationRepository>();
            _mockValidator = new Mock<IValidator<LocationDTO>>();
            _controller = new LocationController(_mockRepo.Object, _mockValidator.Object);
        }

        #region GetAllLocations Tests

        [Fact]
        public async Task GetAllLocations_ReturnsOkResult_WithLocations()
        {
            // Arrange
            var expectedLocations = new List<LocationDTO>
            {
                new LocationDTO
                {
                    LocationId = 1000,
                    StreetAddress = "123 Main St",
                    PostalCode = "12345",
                    City = "New York",
                    StateProvince = "NY",
                    CountryId = "US"
                },
                new LocationDTO
                {
                    LocationId = 2000,
                    StreetAddress = "456 Park Ave",
                    PostalCode = "67890",
                    City = "Los Angeles",
                    StateProvince = "CA",
                    CountryId = "US"
                }
            };
            _mockRepo.Setup(repo => repo.GetAllLocations())
                    .ReturnsAsync(expectedLocations);

            // Act
            var result = await _controller.GetAllLocations();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLocations = Assert.IsType<List<LocationDTO>>(okResult.Value);
            Assert.Equal(2, returnedLocations.Count);
            Assert.Equal("New York", returnedLocations[0].City);
            Assert.Equal("Los Angeles", returnedLocations[1].City);
        }

        [Fact]
        public async Task GetAllLocations_ReturnsEmptyList_WhenNoLocations()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllLocations())
                    .ReturnsAsync(new List<LocationDTO>());

            // Act
            var result = await _controller.GetAllLocations();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLocations = Assert.IsType<List<LocationDTO>>(okResult.Value);
            Assert.Empty(returnedLocations);
        }

        #endregion

        #region GetLocationById Tests

        [Fact]
        public async Task GetLocationById_ReturnsOkResult_WhenLocationExists()
        {
            // Arrange
            var locationId = 1000m;
            var expectedLocation = new LocationDTO
            {
                LocationId = locationId,
                StreetAddress = "123 Main St",
                PostalCode = "12345",
                City = "New York",
                StateProvince = "NY",
                CountryId = "US"
            };
            _mockRepo.Setup(repo => repo.GetLocationById(locationId))
                    .ReturnsAsync(expectedLocation);

            // Act
            var result = await _controller.GetLocationById(locationId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLocation = Assert.IsType<LocationDTO>(okResult.Value);
            Assert.Equal(locationId, returnedLocation.LocationId);
            Assert.Equal("New York", returnedLocation.City);
        }

        [Fact]
        public async Task GetLocationById_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            // Arrange
            var locationId = 9999m;
            _mockRepo.Setup(repo => repo.GetLocationById(locationId))
                    .ReturnsAsync((LocationDTO)null);

            // Act
            var result = await _controller.GetLocationById(locationId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Location with Id= {locationId} not found", notFoundResult.Value);
        }

        #endregion

        
    }
}