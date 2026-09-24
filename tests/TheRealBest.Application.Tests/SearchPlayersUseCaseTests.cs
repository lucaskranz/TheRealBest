namespace TheRealBest.Application.Tests;

using FluentAssertions;
using Moq;
using TheRealBest.Application.UseCases;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;

public class SearchPlayersUseCaseTests
{
    private readonly Mock<IPlayerRepository> _playerRepositoryMock = new();
    private readonly SearchPlayersUseCase _useCase;

    public SearchPlayersUseCaseTests()
    {
        _useCase = new SearchPlayersUseCase(_playerRepositoryMock.Object, TestLabels.Create());
    }

    [Fact]
    public async Task ExecuteAsync_WithQuery_ReturnsMatchingPlayers()
    {
        // Arrange
        var player = new Player("p-haaland", "Erling Haaland", "Norway", new DateOnly(2000, 7, 21), "https://img.com/haaland.png", PlayerPosition.ST);
        _playerRepositoryMock
            .Setup(r => r.SearchAsync("Haaland", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([player]);

        // Act
        var results = await _useCase.ExecuteAsync("Haaland", null);

        // Assert
        results.Should().HaveCount(1);
        results[0].Name.Should().Be("Erling Haaland");
        results[0].PrimaryPosition.Should().Be("ST");
    }

    [Fact]
    public async Task ExecuteAsync_WithPositionFilterOnly_ReturnsFilteredPlayers()
    {
        // Arrange
        var player = new Player("p-carvajal", "Dani Carvajal", "Spain", new DateOnly(1992, 1, 11), "https://img.com/carvajal.png", PlayerPosition.FB);
        _playerRepositoryMock
            .Setup(r => r.GetByPositionAsync(PlayerPosition.FB, It.IsAny<CancellationToken>()))
            .ReturnsAsync([player]);

        // Act
        var results = await _useCase.ExecuteAsync(null, PlayerPosition.FB);

        // Assert
        results.Should().HaveCount(1);
        results[0].Name.Should().Be("Dani Carvajal");
    }
}