using Catalog.API.Data;
using Catalog.API.Services;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests;

public class PlateRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly PlateRepository _plateRepository;

    public PlateRepositoryTests()
    {
        _context = GetInMemoryDbContext();
        _plateRepository = new PlateRepository(_context);
    }


    [Theory]
    [InlineData("asc", "", 3, "ABC123")]        //ascending
    [InlineData("dec", "", 3, "GHI789")]        //descending
    [InlineData("asc", "DE", 1, "DEF456")]      //filter - letters
    [InlineData("asc", "456", 1, "DEF456")]     //filter - numbers
    [InlineData("asc", "ABC1", 1, "ABC123")]    //filter - name
    public async Task GetAsync_ShouldReturnPagedResult(string sortDir, string filter, int resultItems, string resultFirstReg)
    {
        // Arrange
        var pageNum = 1;
        var pageSize = 20;
        var sortCol = "Registration";

        // Act
        var result = await _plateRepository.GetAsync(pageNum, pageSize, sortCol, sortDir, filter);

        var firstPlate = result.Items.FirstOrDefault();

        // Assert
        Assert.Equal(resultItems, result.Items.Count);
        Assert.Equal(resultItems, result.TotalCount);
        Assert.Equal(pageNum, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(resultFirstReg, firstPlate.Registration);
    }

    [Fact]
    public async Task CreatetAsync_ShouldSaveNewReg()
    {
        //arrange
        var plate = new Plate
        {
            Id = Guid.NewGuid(),
            Registration = "XYZ000",
            Letters = "XYZ",
            Numbers = 0
        };

        var beforeResult = await _plateRepository.GetAsync(filter: "XYZ");

        //act
        await _plateRepository.CreateAsync(plate);

        var afterResult = await _plateRepository.GetAsync(filter: "XYZ");

        //assert
        Assert.Equal(0, beforeResult.TotalCount);
        Assert.Equal(1, afterResult.TotalCount);

        await DeleteReg(plate);       //remove from test data (is this needed? is ctor run for every test??)
    }



    /// <summary>
    /// create an in-memory test db
    /// </summary>
    /// <returns></returns>
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
            .Options;

        var context = new ApplicationDbContext(options);

        // Seed test data
        context.Plates.AddRange(new List<Plate>
        {
            new Plate { Id = Guid.NewGuid(), Registration = "ABC123", Letters = "ABC", Numbers = 123 },
            new Plate { Id = Guid.NewGuid(), Registration = "DEF456", Letters = "DEF", Numbers = 456 },
            new Plate { Id = Guid.NewGuid(), Registration = "GHI789", Letters = "GHI", Numbers = 789 }
        });

        context.SaveChanges();
        var count = context.Plates.CountAsync().Result;
        Console.WriteLine($"Plate count: {count}");
        return context;
    }

    private async Task DeleteReg(Plate plate)
    {
        _context.Plates.Remove(plate);
        await _context.SaveChangesAsync();
    }
}
