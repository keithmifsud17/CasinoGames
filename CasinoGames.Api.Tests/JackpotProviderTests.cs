using CasinoGames.Api.Data;
using CasinoGames.Api.Logic;
using CasinoGames.Shared.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CasinoGames.Api.Tests
{
    public class JackpotProviderTests
    {
        private readonly DbContextOptions<GameContext> options;

        public JackpotProviderTests()
        {
            options = new DbContextOptionsBuilder<GameContext>()
                .UseInMemoryDatabase(databaseName: "CasinoGames")
                .Options;

            SetupDatabase();
        }

        private void SetupDatabase()
        {
            using var context = new GameContext(options);
            if (!context.Games.Any())
            {
                for (int i = 1; i <= 10; i++)
                {
                    context.Games.Add(new Shared.Models.Game
                    {
                        Name = $"Game{i}",
                        Image = $"Game{i}Image",
                        Thumbnail = $"Game{i}Thumbnail",
                        Url = $"Game{i}Url"
                    });
                }
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task TestJackpotProvider_GetGames()
        {
            using var context = new GameContext(options);
            var provider = new JackpotProviderA(context);
            var result = await provider.GetGames(CancellationToken.None);

            result.Should()
                .HaveCount(10)
                .And.SatisfyRespectively(Enumerable.Range(1, 10).Select<int, Action<Game>>(i => game => TestGame(game, i)));
        }

        [Fact]
        public async Task TestJackpotProvider_GetGame()
        {
            using var context = new GameContext(options);
            var provider = new JackpotProviderA(context);
            var game = await provider.GetGame(2);
            TestGame(game, 2);
        }

        [Fact]
        public async Task TestJackpotProvider_GetGame_InvalidId_NullResult()
        {
            using var context = new GameContext(options);
            var provider = new JackpotProviderA(context);
            var game = await provider.GetGame(50);
            game.Should().BeNull();
        }

        [Fact]
        public async Task TestJackpotProvider_AddStatistic()
        {
            using var context = new GameContext(options);
            var provider = new JackpotProviderA(context);

            var game = await provider.GetGame(1);
            await provider.AddStatistic(game, "testSession");

            var statistic = await context.Statistics.FirstAsync();

            statistic.SessionId.Should().Be("testSession");
            statistic.Game.Should().BeEquivalentTo(game);
            statistic.DateTime.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
        }

        [Fact]
        public void TestJackpotProvider_AddStatistic_NullGame_ThrowException()
        {
            using var context = new GameContext(options);
            var provider = new JackpotProviderA(context);

            Func<Task> act = async () => await provider.AddStatistic(null, "testSession");
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("game");
        }

        [Fact]
        public void TestJackpotProvider_AddStatistic_NullSession_ThrowException()
        {
            using var context = new GameContext(options);
            var provider = new JackpotProviderA(context);

            Func<Task> act = async () => await provider.AddStatistic(new Game(), null);
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("sessionId");
        }

        [Fact]
        public void TestJackpotProvider_AddStatistic_NonExistantGame_ThrowException()
        {
            using var context = new GameContext(options);
            var provider = new JackpotProviderA(context);

            Func<Task> act = async () => await provider.AddStatistic(new Game(), "sessionId");
            act.Should().Throw<InvalidOperationException>().WithInnerException<ArgumentException>();
        }

        private void TestGame(Game game, int i)
        {
            game.Name.Should().Be($"Game{i}");
            game.Image.Should().Be($"Game{i}Image");
            game.Thumbnail.Should().Be($"Game{i}Thumbnail");
            game.Url.Should().Be($"Game{i}Url");
        }
    }
}