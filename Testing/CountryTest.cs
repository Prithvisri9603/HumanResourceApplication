using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using HumanResourceApplication.Controllers;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using HumanResourceApplication.Utility;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Testing
{
    public class CountryTest
    {
        private readonly Mock<ICountryRepository> _mockCountryRepository;
        private readonly Mock<IValidator<CountryDTO>> _mockValidator;
        private readonly CountryController _controller;
        public CountryTest()
        {
            _mockCountryRepository = new Mock<ICountryRepository>();
            _mockValidator = new Mock<IValidator<CountryDTO>>();
            _controller=new CountryController(_mockCountryRepository.Object, _mockValidator.Object);
        }

        #region GetAllCountries Tests

        [Fact]
        public async Task GetAllCountries_ReturnsOk_WhenCountriesExist()
        {
            var mockCountries = new List<CountryDTO>
            {
                new CountryDTO { CountryId = "AR", CountryName = "Argentina", RegionId = 20 },
                new CountryDTO { CountryId = "BR", CountryName = "Brazil", RegionId = 20 }
            };
            _mockCountryRepository.Setup(repo => repo.GetAllCountries()).ReturnsAsync(mockCountries);
            var result = await _controller.GetAllCountries();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(mockCountries, okResult.Value);
        }

        [Fact]
        public async Task GetAllCountries_ReturnsNotFound_WhenNoCountriesExist()
        {
            _mockCountryRepository.Setup(repo => repo.GetAllCountries()).ReturnsAsync(new List<CountryDTO>());
            var result = await _controller.GetAllCountries();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var actualMessage = notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null)?.ToString();
            Assert.Equal("No countries found.", actualMessage);
        }

        #endregion

        #region GetCountryById Tests

        [Fact]
        public async Task GetCountryById_ReturnsOk_WhenCountryExists()
        {
         
            var mockCountry = new CountryDTO { CountryId = "AR", CountryName = "Argentina", RegionId = 20 };
            _mockCountryRepository.Setup(repo => repo.GetCountryById("AR")).ReturnsAsync(mockCountry);

          
            var result = await _controller.GetCountryById("AR");

        
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(mockCountry, okResult.Value);
        }

        [Fact]
        public async Task GetCountryById_ReturnsNotFound_WhenCountryDoesNotExist()
        {
            // Arrange
            _mockCountryRepository.Setup(repo => repo.GetCountryById("AR")).ReturnsAsync((CountryDTO)null);

            // Act
            var result = await _controller.GetCountryById("AR");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var actualMessage = notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null)?.ToString();

            Assert.Equal("Country not found.", actualMessage);
        }

        #endregion

        #region DeleteCountryById Tests

        [Fact]
        public async Task DeleteCountryById_ReturnsNoContent_WhenCountryExists()
        {
            // Arrange
            _mockCountryRepository.Setup(repo => repo.DeleteCountryById("AR")).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCountryById("AR");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCountryById_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            _mockCountryRepository.Setup(repo => repo.DeleteCountryById("AR")).ThrowsAsync(new System.Exception("Error deleting country."));

            // Act
            var result = await _controller.DeleteCountryById("AR");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error deleting country.", badRequestResult.Value);
        }

        #endregion

    }
}
