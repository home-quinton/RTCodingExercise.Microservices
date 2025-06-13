using Catalog.API.Controllers;
using Catalog.API.Models;
using Catalog.API.Services;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Plates.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests;


public class CatalogControllerTests
{
    //static results
    private List<Plate> _dataListPlates = new List<Plate>
    {
        new Plate { Id = Guid.NewGuid(), Registration = "R12 TC3", Letters = "RTC", Numbers = 123, PurchasePrice = 9.99M, SalePrice = 99.99M }
    };

    private PagedResult<Plate>  _dataPagedResult = new PagedResult<Plate>
    {
        //Items = _dataListPlates,
        TotalCount = 50,
        PageNumber = 1,
        PageSize = 20,
        SortColumn = "Registration",
        SortDirection = "asc",
        Filter = ""
    };

    private readonly Mock<IPlatesService> _mockPlatesService;
    private readonly Mock<ILogger<CatalogController>> _mockLogger;
    private readonly CatalogController _controller;

    public CatalogControllerTests()
    {
        _mockPlatesService = new Mock<IPlatesService>();
        _mockLogger = new Mock<ILogger<CatalogController>>();

        _controller = new CatalogController(_mockPlatesService.Object, 
                                            _mockLogger.Object);

        _dataPagedResult.Items = _dataListPlates;
    }

    [Fact]
    public async Task GetPlates_ReturnsOkResult_WithPagedPlates()
    {
        // Arrange
        // use _dataPagedResult

        _mockPlatesService
            .Setup(s => s.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_dataPagedResult);

        // Act
        var result = await _controller.GetPlates();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
    }


    [Fact]
    public async Task GetPlates_ReturnsServiceError500_WhenFailure()
    {
        // Arrange
        var ex = new Exception("Failed");

        _mockPlatesService
            .Setup(s => s.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(ex);

        // Act
        var result = await _controller.GetPlates();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }


    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.NoContent)]
    public async Task PutAsync_ReturnsStatusCode_WhenPlateIsCreated(HttpStatusCode statusCode)
    {
        // Arrange
        //use _dataPagedResult
        var plateDTO = new PlateDTO { Registration = "RTC123", PurchasePrice = 1.99M, SalePrice = 2.99M };
        var apiStatus = new APIStatus(statusCode, "");

        _mockPlatesService.Setup(s => s.CreateAsync(It.IsAny<PlateDTO>())).ReturnsAsync(apiStatus);

        // Act
        var result = await _controller.PutAsync(plateDTO.Registration, plateDTO);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)statusCode, statusCodeResult.StatusCode);
    }

}
