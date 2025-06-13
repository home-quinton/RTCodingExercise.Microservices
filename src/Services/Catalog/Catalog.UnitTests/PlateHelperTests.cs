using Catalog.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Catalog.UnitTests;
public class PlateHelperTests
{
    [Theory]
    [InlineData("", 0)]
    [InlineData("REG", 0)]
    [InlineData("ABC123", 123)]
    [InlineData("NO456PLATE", 456)]
    [InlineData("1234", 1234)]
    public void GetDigits_ShouldExtractDigitsCorrectly(string registration, int expected)
    {
        // Act
        var result = PlateHelper.GetDigits(registration);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("RTC123", "RTC")]
    [InlineData("123ABC", "IRB")]
    [InlineData("4B5", "ABS")]
    [InlineData("9Z", "GZ")]
    [InlineData("C", "C")]
    public void GetLetters_ShouldGetLettersWithConversions(string registration, string expected)
    {
        // Act
        var result = PlateHelper.GetLetters(registration, expected.Length);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData('1', 'I')] // '1' -> "IL", first char = I
    [InlineData('2', 'R')] // '2' -> "RZ"
    [InlineData('3', 'B')] // '3' -> "BE"
    [InlineData('C', 'C')] // not in dictionary, should return original
    [InlineData('0', 'O')]
    public void GetTranslation_ShouldTranslateNumbersToLetters(char input, char expected)
    {
        // Act
        var result = PlateHelper.GetTranslation(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetCombinations_ShouldReturnExpectedPlates()
    {
        // Arrange
        string input = "GSMIT";
        List<string> expected = "GSMIT,6SMIT,9SMIT,G5MIT,65MIT,95MIT,GSM1T,6SM1T,9SM1T,G5M1T,65M1T,95M1T,GSMI7,6SMI7,9SMI7,G5MI7,65MI7,95MI7,GSM17,6SM17,9SM17,G5M17,65M17,95M17".Split(',').ToList();

        // Act
        var results = PlateHelper.GetCombinations(input);

        // Assert
        var isAllInExpected = expected.All(e => results.Contains(e));

        Assert.True(isAllInExpected);
        Assert.Equal(expected.Count, results.Count);
    }

    [Theory]
    //[InlineData("", "")]      //empty
    [InlineData("MNP", "MNP")]  //no combinations
    public void GetCombinations_ShouldReturnExpected(string reg, string expected)
    {
        // Arrange

        // Act
        var results = PlateHelper.GetCombinations(reg);

        // Assert
        Assert.Single(results);
        Assert.Equal(results[0], expected);
    }
}
