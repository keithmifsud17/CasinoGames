using CasinoGames.Api.Controllers;
using CasinoGames.Api.Logic;
using CasinoGames.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            mockProvider.Setup(provider => provider.GetGamesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);
            mockProvider.Verify();

            var result = await controller.GetGamesAsync(mockProvider.Object, CancellationToken.None);
            result.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task TestJackpots_Returns_Jackpots()
        {
            var controller = new GameController();

            var list = new[] { new Jackpot { Game = new Game { GameId = 1, Image = "Image", Name = "Name", Thumbnail = "Thumbnail", Url = "Url" }, JackpotId = 1, Value = 1 } };

            var mockProvider = new Mock<IJackpotProvider>();
            mockProvider.Setup(provider => provider.GetJackpotsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);
            mockProvider.Verify();

            var result = await controller.GetJackpotsAsync(mockProvider.Object, CancellationToken.None);
            result.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task TestPlayGame_Returns_Ok()
        {
            var controller = new GameController();
            var mockProvider = new Mock<IJackpotProvider>();

            mockProvider
                .Setup(provider => provider.GetGameAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int gameId, CancellationToken token) => new Game { GameId = gameId, Image = "Image", Name = "Name", Thumbnail = "Thumbnail", Url = "Url" });

            mockProvider
                .Setup(provider => provider.AddStatisticAsync(It.IsAny<Game>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            mockProvider.Verify();

            var result = await controller.PlayGameAsync(mockProvider.Object, 1, CancellationToken.None);
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
                .Setup(provider => provider.GetGameAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: null);

            mockProvider
                .Setup(provider => provider.AddStatisticAsync(It.IsAny<Game>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            mockProvider.Verify();

            var result = await controller.PlayGameAsync(mockProvider.Object, 1, CancellationToken.None);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task TestAddGame_Returns_Game()
        {
            var controller = new GameController();
            var mockAdminProvider = new Mock<IAdminJackpotProvider>();

            mockAdminProvider
                .Setup(provider => provider.AddGameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string name, string image, string thumbnail, CancellationToken token) => new Game
                {
                    DateCreated = DateTime.UtcNow,
                    GameId = 1,
                    Image = image,
                    Name = name,
                    Thumbnail = thumbnail,
                    TotalPlays = 0,
                    Url = default
                });

            mockAdminProvider.Verify();

            var mockGame = new Models.GameApiModel { Name = "name", Image = "image", Thumbnail = "thumbnail" };
            var result = await controller.AddGameAsync(mockAdminProvider.Object, mockGame, CancellationToken.None);

            result.Name.Should().Be(mockGame.Name);
            result.Image.Should().Be(mockGame.Image);
            result.Thumbnail.Should().Be(mockGame.Thumbnail);
        }

        [Fact]
        public void TestAddGame_ValidateModel_ValidModelState()
        {
            var result = new List<ValidationResult>();

            var mockGame = new Models.GameApiModel { Name = "name", Image = "https://www.google.com", Thumbnail = "https://www.google.com" };

            var isValid = Validator.TryValidateObject(mockGame, new ValidationContext(mockGame), result);

            isValid.Should().BeTrue();
        }

        [Fact]
        public void TestAddGame_EmptyName_InvalidModelState()
        {
            var result = new List<ValidationResult>();

            var mockGame = new Models.GameApiModel { Image = "https://www.google.com", Thumbnail = "https://www.google.com" };

            var isValid = Validator.TryValidateProperty(mockGame.Name, new ValidationContext(mockGame) { MemberName = nameof(mockGame.Name) }, result);

            isValid.Should().BeFalse();

            result.Should().HaveCount(1);
            result.First().MemberNames.First().Should().Be(nameof(mockGame.Name));
            result.First().ErrorMessage.Should().Be($"The {nameof(mockGame.Name)} field is required.");
        }

        [Fact]
        public void TestAddGame_EmptyImage_InvalidModelState()
        {
            var result = new List<ValidationResult>();

            var mockGame = new Models.GameApiModel { Name = "name", Thumbnail = "https://www.google.com" };

            var isValid = Validator.TryValidateProperty(mockGame.Image, new ValidationContext(mockGame) { MemberName = nameof(mockGame.Image) }, result);

            isValid.Should().BeFalse();

            result.Should().HaveCount(1);
            result.First().MemberNames.First().Should().Be(nameof(mockGame.Image));
            result.First().ErrorMessage.Should().Be($"The {nameof(mockGame.Image)} field is required.");
        }

        [Fact]
        public void TestAddGame_InvalidUrlImage_InvalidModelState()
        {
            var result = new List<ValidationResult>();

            var mockGame = new Models.GameApiModel { Name = "name", Image = "Hello World", Thumbnail = "https://www.google.com" };

            var isValid = Validator.TryValidateProperty(mockGame.Image, new ValidationContext(mockGame) { MemberName = nameof(mockGame.Image) }, result);

            isValid.Should().BeFalse();

            result.Should().HaveCount(1);
            result.First().MemberNames.First().Should().Be(nameof(mockGame.Image));
            result.First().ErrorMessage.Should().Be($"The {nameof(mockGame.Image)} field is not a valid fully-qualified http, https, or ftp URL.");
        }

        [Fact]
        public void TestAddGame_EmptyThumbnail_InvalidModelState()
        {
            var result = new List<ValidationResult>();

            var mockGame = new Models.GameApiModel { Name = "name", Image = "https://www.google.com" };

            var isValid = Validator.TryValidateProperty(mockGame.Thumbnail, new ValidationContext(mockGame) { MemberName = nameof(mockGame.Thumbnail) }, result);

            isValid.Should().BeFalse();

            result.Should().HaveCount(1);
            result.First().MemberNames.First().Should().Be(nameof(mockGame.Thumbnail));
            result.First().ErrorMessage.Should().Be($"The {nameof(mockGame.Thumbnail)} field is required.");
        }

        [Fact]
        public void TestAddGame_InvalidUrlThumbnail_InvalidModelState()
        {
            var result = new List<ValidationResult>();

            var mockGame = new Models.GameApiModel { Name = "name", Image = "https://www.google.com", Thumbnail = "Hello World" };

            var isValid = Validator.TryValidateProperty(mockGame.Thumbnail, new ValidationContext(mockGame) { MemberName = nameof(mockGame.Thumbnail) }, result);

            isValid.Should().BeFalse();

            result.Should().HaveCount(1);
            result.First().MemberNames.First().Should().Be(nameof(mockGame.Thumbnail));
            result.First().ErrorMessage.Should().Be($"The {nameof(mockGame.Thumbnail)} field is not a valid fully-qualified http, https, or ftp URL.");
        }
    }
}