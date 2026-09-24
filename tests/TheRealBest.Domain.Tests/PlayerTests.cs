namespace TheRealBest.Domain.Tests;

using FluentAssertions;
using TheRealBest.Domain.Entities;
using TheRealBest.Domain.Enums;
using Xunit;

public class PlayerTests
{
    [Fact]
    public void CreatePlayer_WithValidData_ShouldInitializeCorrectly()
    {
        // Arrange
        var extId = "api-123";
        var name = "Rodri";
        var nationality = "Spain";
        var dob = new DateOnly(1996, 6, 22);
        var photo = "https://example.com/rodri.png";
        var position = PlayerPosition.CDM;

        // Act
        var player = new Player(extId, name, nationality, dob, photo, position);

        // Assert
        player.Id.Should().NotBeEmpty();
        player.ExternalApiId.Should().Be(extId);
        player.Name.Should().Be(name);
        player.Nationality.Should().Be(nationality);
        player.DateOfBirth.Should().Be(dob);
        player.PhotoUrl.Should().Be(photo);
        player.PrimaryPosition.Should().Be(position);
        player.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        player.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateFieldsAndSetUpdatedAt()
    {
        // Arrange
        var player = new Player("1", "Vini", "Brazil", new DateOnly(2000, 7, 12), "url1", PlayerPosition.W);

        // Act
        player.UpdateProfile("Vinicius Junior", "Brazil", "url2", PlayerPosition.W);

        // Assert
        player.Name.Should().Be("Vinicius Junior");
        player.PhotoUrl.Should().Be("url2");
        player.UpdatedAt.Should().NotBeNull();
        player.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}