using CasinoGames.Api.Controllers;
using CasinoGames.Api.Logic;
using CasinoGames.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CasinoGames.Api.Tests
{
    public class GameControllerTests
    {
        [Fact]
        public async Task TestIndex_Returns_Games()
        {
            var controller = new GameController();

            var list = new[] { new Game { GameId = 1, Image = "Image", Name = "Name", Thumbnail = "Thumbnail", Url = "Url" } };

            var mockProvider = new Mock<IJackpotProvider>();
            mockProvider.Setup(provider => provider.GetGames(It.IsAny<CancellationToken>())).ReturnsAsync(list);
            mockProvider.Verify();

            var result = await controller.Index(mockProvider.Object, CancellationToken.None);
            result.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task TestJackpots_Returns_Jackpots()
        {
            var controller = new GameController();

            var list = new[] { new Jackpot { Game = new Game { GameId = 1, Image = "Image", Name = "Name", Thumbnail = "Thumbnail", Url = "Url" }, JackpotId = 1, Value = 1 } };

            var mockProvider = new Mock<IJackpotProvider>();
            mockProvider.Setup(provider => provider.GetJackpots(It.IsAny<CancellationToken>())).ReturnsAsync(list);
            mockProvider.Verify();

            var result = await controller.Jackpots(mockProvider.Object, CancellationToken.None);
            result.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task TestPlayGame_Returns_Ok()
        {
            var controller = new GameController();
            var mockProvider = new Mock<IJackpotProvider>();

            mockProvider
                .Setup(provider => provider.GetGame(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int gameId, CancellationToken token) => new Game { GameId = gameId, Image = "Image", Name = "Name", Thumbnail = "Thumbnail", Url = "Url" });

            mockProvider
                .Setup(provider => provider.AddStatistic(It.IsAny<Game>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            mockProvider.Verify();

            var result = await controller.PlayGame(mockProvider.Object, 1, CancellationToken.None);
            var okResult = result.Should().BeOfType<OkObjectResult>();

            okResult.Which.StatusCode.Should().Be(200);
            okResult.Which.Value.Should().Be("Now playing Name");
        }

        [Fact]
        public async Task TestPlayGame_InvalidId_Returns_NotFound()
        {
            var controller = new GameController();
            var mockProvider = new Mock<IJackpotProvider>();

            mockProvider
                .Setup(provider => provider.GetGame(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            mockProvider
                .Setup(provider => provider.AddStatistic(It.IsAny<Game>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            mockProvider.Verify();

            var result = await controller.PlayGame(mockProvider.Object, 1, CancellationToken.None);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}