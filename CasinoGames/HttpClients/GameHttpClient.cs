using CasinoGames.Shared.Models;
using CasinoGames.Website.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CasinoGames.Website.HttpClients
{
    public class GameHttpClient : IGameHttpClient
    {
        private readonly HttpClient client;

        public GameHttpClient(HttpClient client)
        {
            this.client = client;
        }

        public Task<IEnumerable<Game>> ListGamesAsync() => SimpleListAsync<Game>("api/game");

        public Task<IEnumerable<Jackpot>> ListJackpotsAsync() => SimpleListAsync<Jackpot>("api/game/jackpots");

        public async Task<Game> AddGameAsync(GameViewModel game)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/game")
            {
                Content = new StringContent(JsonSerializer.Serialize(game), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Game>(
                responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        private async Task<IEnumerable<T>> SimpleListAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<T>>(
                responseStream, 
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

    }

    public interface IGameHttpClient
    {
        Task<Game> AddGameAsync(GameViewModel game);
        Task<IEnumerable<Game>> ListGamesAsync();

        Task<IEnumerable<Jackpot>> ListJackpotsAsync();
    }
}