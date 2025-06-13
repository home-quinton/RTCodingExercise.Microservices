using Catalog.Domain;
using Microsoft.Extensions.Options;
using Moq;
using Plates.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebMVC.Clients;
using WebMVC.Config;
using WebMVC.Services;
using Xunit;

namespace WebMVC.UnitTests;

public class PlateServiceTests
{
    PlateServiceConfig _config = new PlateServiceConfig
    {
        BaseAddress = "http://testbase/",
        GetSortedUrl = "testSortedurl",
        PutUrl = "testPuturl",
        GetUrl = "testGeturl",
        Timeout = 30
    };

    private readonly Mock<IPlateClient> _mockPlateClient;
    private readonly Mock<IOptions<PlateServiceConfig>> _mockConfig;
    private readonly PlateService _plateService;

    public PlateServiceTests()
    {
        _mockPlateClient = new Mock<IPlateClient>();
        _mockConfig = new Mock<IOptions<PlateServiceConfig>>();
        _mockConfig.Setup(o => o.Value).Returns(_config);

        _plateService = new PlateService(_mockPlateClient.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task GetPlates_ValidResponse_ReturnsMappedViewModel()
    {
        // Arrange
        var salePrice = 2000;
        var resultData = new PagedResult<Plate>
        {
            PageNumber = 1,
            SortColumn = "Registration",
            SortDirection = "asc",
            Filter = "",
            Items = new List<Plate>
            {
                new Plate { Registration = "ABC123", PurchasePrice = 1000, SalePrice = salePrice }
            }
        };

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(resultData), Encoding.UTF8, "application/json")
        };

        _mockPlateClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

        // Act
        var result = await _plateService.GetPlates(1, 20, "Registration", "asc", "test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(salePrice * 1.2M, result.Items.First().SalePrice); // markup 20%
    }

    [Fact]
    public async Task GetPlates_InvalidResponse_ReturnsEmptyViewModel()
    {
        // Arrange
        var resultData = new PagedResult<Plate>();

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        _mockPlateClient.Setup(s => s.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

        // Act
        var result = await _plateService.GetPlates(1, 20, "Registration", "asc", "test");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Theory]
    [InlineData(HttpStatusCode.OK, true)]
    [InlineData(HttpStatusCode.InternalServerError, false)]
    public async Task Create_ReturnsExpectedResult(HttpStatusCode statusCode, bool expectedResult)
    {
        // Arrange
        var plateDTO = new PlateDTO();

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = statusCode
        };

        _mockPlateClient.Setup(s => s.PutAsJsonAsync<PlateDTO>(It.IsAny<string>(), It.IsAny<PlateDTO>())).ReturnsAsync(httpResponse);

        // Act
        var result = await _plateService.Create(plateDTO);

        // Assert
        Assert.Equal(expectedResult, result);
    }

}
