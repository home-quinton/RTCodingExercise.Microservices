using Catalog.API.Services;
using Catalog.Domain;
using Moq;
using Plates.Shared;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests;
public class PlateServiceTests
{
    private readonly Mock<IPlateRepository> _mockPlateRepository;
    private readonly IPlatesService _platesService;

    public PlateServiceTests()
	{
        _mockPlateRepository = new Mock<IPlateRepository>();
        _platesService = new PlatesService(_mockPlateRepository.Object);
    }

    //at this stage no point doin unit test for GetAsync() as it's a simple pass through method

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_WhenEmptyRequest()
    {
        //arrange
        var plateDTO = new PlateDTO { Registration = "" };

        //act
        var result = await _platesService.CreateAsync(plateDTO);

        //assert
        Assert.Equal((int)HttpStatusCode.BadRequest, result.Status);
    }

    [Fact]
    public async Task CreateAsync_ReturnsConflict_WhenRegExists()
    {
        //arrange
        var plateDTO = new PlateDTO { Registration = "AAA" };
        _mockPlateRepository.Setup(s => s.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        //act
        var result = await _platesService.CreateAsync(plateDTO);

        //assert
        Assert.Equal((int)HttpStatusCode.Conflict, result.Status);
    }

    [Fact]
    public async Task CreateAsync_ReturnsServerError_WhenServiceFailure()
    {
        //arrange
        var plateDTO = new PlateDTO { Registration = "AAA" };
        var ex = new Exception("Failed");
        _mockPlateRepository.Setup(s => s.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockPlateRepository.Setup(s => s.CreateAsync(It.IsAny<Plate>())).ThrowsAsync(ex);

        //act
        var result = await _platesService.CreateAsync(plateDTO);

        //assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, result.Status);
    }

    [Fact]
    public async Task CreateAsync_ReturnsOK_WhenNewReg()
    {
        //arrange
        var plateDTO = new PlateDTO { Registration = "AAA" };
        _mockPlateRepository.Setup(s => s.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockPlateRepository.Setup(s => s.CreateAsync(It.IsAny<Plate>())).Returns(Task.CompletedTask);

        //act
        var result = await _platesService.CreateAsync(plateDTO);

        //assert
        Assert.Equal((int)HttpStatusCode.NoContent, result.Status);
    }

}
