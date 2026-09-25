namespace TheRealBest.API.IntegrationTests;

using FluentAssertions;
using TheRealBest.API.Commands;
using TheRealBest.Infrastructure.Ingestion;

public class ImportSeasonCommandTests
{
    [Fact]
    public void Parse_SeasonOnly()
    {
        var request = ImportSeasonCommand.Parse(["--import-season", "2023"], out var error);

        error.Should().BeNull();
        request.Should().Be(new SeasonImportRequest(2023));
    }

    [Fact]
    public void Parse_WithCompetitionAndLimit()
    {
        var request = ImportSeasonCommand.Parse(["--import-season", "2023", "--competition", "9", "--limit", "5"], out var error);

        error.Should().BeNull();
        request.Should().Be(new SeasonImportRequest(2023, 9, 5));
    }

    [Theory]
    [InlineData(new[] { "--import-season" }, "expects a number")]
    [InlineData(new[] { "--import-season", "abc" }, "expects a number")]
    [InlineData(new[] { "--import-season", "2019" }, "outside the supported range")]
    [InlineData(new[] { "--import-season", "2024", "--competition", "4" }, "not in the scope")]
    [InlineData(new[] { "--import-season", "2023", "--limit", "0" }, "greater than zero")]
    public void Parse_InvalidArguments_ReturnsError(string[] args, string expected)
    {
        var request = ImportSeasonCommand.Parse(args, out var error);

        request.Should().BeNull();
        error.Should().Contain(expected);
    }
}
