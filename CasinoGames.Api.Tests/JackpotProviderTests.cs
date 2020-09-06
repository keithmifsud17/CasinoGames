using CasinoGames.Api.Data;
using CasinoGames.Api.Logic;
using CasinoGames.Shared.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CasinoGames.Api.Tests
{
    public class JackpotProviderTests
    {
        private readonly DbContextOptions<UserGameContext> options;
        private readonly DbContextOptions<AdminGameContext> adminOptions;

        public JackpotProviderTests()
        {
            var InMemoryDatabaseRoot = new InMemoryDatabaseRoot();

            options = new DbContextOptionsBuilder<UserGameContext>()
                .UseInMemoryDatabase(databaseName: "CasinoGames", InMemoryDatabaseRoot)
                .Options;

            adminOptions = new DbContextOptionsBuilder<AdminGameContext>()
                .UseInMemoryDatabase(databaseName: "CasinoGames", InMemoryDatabaseRoot)
                .Options;

            SetupDatabase();
        }

        private void SetupDatabase()
        {
            using var context = new UserGameContext(options);
            if (!context.Games.Any())
            {
                for (int i = 1; i <= 10; i++)
                {
                    var game = context.Games.Attach(new Shared.Models.Game
                    {
                        Name = $"Game{i}",
                        Image = $"Game{i}Image",
                        Thumbnail = $"Game{i}Thumbnail",
                        Url = $"Game{i}Url",
                        DateCreated = DateTime.UtcNow,
                        Enabled = true
                    });

                    context.Jackpots.Add(new Jackpot
                    {
                        Game = game.Entity,
                        Value = i * 100
                    });
                }
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task TestJackpotProvider_GetGames()
        {
            using var context = new UserGameContext(options);
            var provider = new JackpotProviderA(context, null);
            var result = await provider.GetGamesAsync(CancellationToken.None);

            result.Should()
                .HaveCount(10)
                .And.SatisfyRespectively(Enumerable.Range(1, 10).Select<int, Action<Game>>(i => game => TestGame(game, i)));
        }

        [Fact]
        public async Task TestJackpotProvider_GetJackpots()
        {
            using var context = new UserGameContext(options);
            var provider = new JackpotProviderA(context, null);
            var result = await provider.GetJackpotsAsync(CancellationToken.None);

            result.Should()
                .HaveCount(10)
                .And.SatisfyRespectively(Enumerable.Range(1, 10).Select<int, Action<Jackpot>>(i => jackpot =>
                {
                    jackpot.Value.Should().Be(i * 100);

                    TestGame(jackpot.Game, i);
                }));
        }

        [Fact]
        public async Task TestJackpotProvider_GetGame()
        {
            using var context = new UserGameContext(options);
            var provider = new JackpotProviderA(context, null);
            var game = await provider.GetGameAsync(2);
            TestGame(game, 2);
        }

        [Fact]
        public async Task TestJackpotProvider_GetGame_InvalidId_NullResult()
        {
            using var context = new UserGameContext(options);
            var provider = new JackpotProviderA(context, null);
            var game = await provider.GetGameAsync(50);
            game.Should().BeNull();
        }

        [Fact]
        public async Task TestJackpotProvider_AddStatistic()
        {
            using var context = new UserGameContext(options);
            var provider = new JackpotProviderA(context, null);

            var game = await provider.GetGameAsync(1);
            await provider.AddStatisticAsync(game, "testSession");

            var statistic = await context.Statistics.FirstAsync();

            statistic.SessionId.Should().Be("testSession");
            statistic.Game.Should().BeEquivalentTo(game);
            statistic.DateTime.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
        }

        [Fact]
        public void TestJackpotProvider_AddStatistic_NullGame_ThrowException()
        {
            using var context = new UserGameContext(options);
            var provider = new JackpotProviderA(context, null);

            Func<Task> act = async () => await provider.AddStatisticAsync(null, "testSession");
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("game");
        }

        [Fact]
        public void TestJackpotProvider_AddStatistic_NullSession_ThrowException()
        {
            using var context = new UserGameContext(options);
            var provider = new JackpotProviderA(context, null);

            Func<Task> act = async () => await provider.AddStatisticAsync(new Game(), null);
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("sessionId");
        }

        [Fact]
        public void TestJackpotProvider_AddStatistic_NonExistantGame_ThrowException()
        {
            using var context = new UserGameContext(options);
            var provider = new JackpotProviderA(context, null);

            Func<Task> act = async () => await provider.AddStatisticAsync(new Game(), "sessionId");
            act.Should().Throw<InvalidOperationException>().WithInnerException<ArgumentException>();
        }

        [Fact]
        public async Task TestAdminJackpotProvider_AddGame()
        {
            using var context = new UserGameContext(options);
            using var adminContext = new AdminGameContext(adminOptions);
            var provider = new JackpotProviderA(context, adminContext);

            var game = await provider.AddGameAsync("GameName", "GameImage", "GameThumbnail");

            game.GameId.Should().BeGreaterThan(0);
            game.Name.Should().Be($"GameName");
            game.Image.Should().Be($"GameImage");
            game.Thumbnail.Should().Be($"GameThumbnail");
            game.DateCreated.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
        }

        [Fact]
        public void TestAdminJackpotProvider_AddGame_EmptyName_Throw()
        {
            using var context = new UserGameContext(options);
            using var adminContext = new AdminGameContext(adminOptions);
            var provider = new JackpotProviderA(context, adminContext);

            Func<Task> act = async () => await provider.AddGameAsync(null, "GameImage", "GameThumbnail");
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("name");
        }

        [Fact]
        public void TestAdminJackpotProvider_AddGame_EmptyImage_Throw()
        {
            using var context = new UserGameContext(options);
            using var adminContext = new AdminGameContext(adminOptions);
            var provider = new JackpotProviderA(context, adminContext);

            Func<Task> act = async () => await provider.AddGameAsync("GameName", null, "GameThumbnail");
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("image");
        }

        [Fact]
        public void TestAdminJackpotProvider_AddGame_EmptyThumbnail_Throw()
        {
            using var context = new UserGameContext(options);
            using var adminContext = new AdminGameContext(adminOptions);
            var provider = new JackpotProviderA(context, adminContext);

            Func<Task> act = async () => await provider.AddGameAsync("GameName", "GameImage", null);
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("thumbnail");
        }

        [Fact]
        public async Task TestAdminJackpotProvider_DeleteGame_CannotFetchIt()
        {
            using var context = new UserGameContext(options);
            using var adminContext = new AdminGameContext(adminOptions);
            var provider = new JackpotProviderA(context, adminContext);

            var game = (await provider.GetGamesAsync()).First();
            game.Enabled.Should().BeTrue();

            await provider.DeleteGameAsync(game.GameId);

            //raw value
            var deletedGame = await adminContext.Games.FirstAsync(g => g.GameId.Equals(game.GameId));
            deletedGame.Enabled.Should().BeFalse();
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