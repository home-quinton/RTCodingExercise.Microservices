using Microsoft.AspNetCore.Mvc;
using Moq;
using Plates.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Controllers;
using WebMVC.Services;
using WebMVC.ViewModels;
using Xunit;

namespace WebMVC.UnitTests;

public class PlatesControllerTests
{
    private readonly Mock<IPlateService> _mockPlateService;
    private readonly PlatesController _platesController;

    public PlatesControllerTests()
    {
        _mockPlateService = new Mock<IPlateService>();
        _platesController = new PlatesController(_mockPlateService.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithExpectedViewModel()
    {
        // Arrange
        var expectedVm = new PagedViewModel<PlateDTO>
        {
            Items = new List<PlateDTO>
            {
                new PlateDTO { Registration = "ABC123" },
                new PlateDTO { Registration = "XYZ789" }
            },
            CurrentPage = 1,
            TotalPages = 5            
        };

        _mockPlateService
            .Setup(s => s.GetPlates(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedVm);

        // Act
        var result = await _platesController.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<PagedViewModel<PlateDTO>>(viewResult.Model);
        Assert.Equal(expectedVm, model);
    }

    [Fact]
    public async Task Create_SuccessfulRedirectsToIndex()
    {
        // Arrange
        var model = new PlateDTO();
        _mockPlateService.Setup(s => s.Create(model)).ReturnsAsync(true);

        // Act
        var result = await _platesController.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_UnsuccessfulReturnsViewWithModel()
    {
        // Arrange
        var model = new PlateDTO();
        _mockPlateService.Setup(s => s.Create(model)).ReturnsAsync(false);

        // Act
        var result = await _platesController.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
    }

    [Fact]
    public async Task Create_ExceptionReturnsViewWithModel()
    {
        // Arrange
        var model = new PlateDTO();
        var ex = new Exception("DB Error");
        _mockPlateService.Setup(s => s.Create(model)).ThrowsAsync(ex);

        // Act
        var result = await _platesController.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
    }

}
