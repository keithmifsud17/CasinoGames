using CasinoGames.Api.HttpClients;
using CasinoGames.Shared.Models;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CasinoGames.Website.Tests
{
    public class GameHttpClientTests
    {
        [Fact]
        public async Task TestGetGames_Returns_Serialized_List()
        {
            var list = new[] { new Game { GameId = 1, Image = "Image", Name = "Name", Thumbnail = "Thumbnail", Url = "Url" } };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(list)),
                });

            var mockClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new System.Uri("https://www.google.com") // Sorry google
            };

            var gameHttpClient = new GameHttpClient(mockClient);

            var result = await gameHttpClient.ListGamesAsync();

            result.Should().BeEquivalentTo(list);
        }

        [Fact]
        public void TestGetGames_NotFound_Throws_Exception()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var mockClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new System.Uri("https://www.google.com") // Sorry google
            };

            var gameHttpClient = new GameHttpClient(mockClient);

            Func<Task> act = async () => await gameHttpClient.ListGamesAsync();

            act.Should().Throw<HttpRequestException>();
        }
    }
}