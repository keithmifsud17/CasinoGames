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

        public Task<IEnumerable<Game>> ListGamesAsync() => GetAsync<IEnumerable<Game>>("api/game");

        public Task<Game> GetGameAsync(int id) => GetAsync<Game>($"api/game/{id}");

        public Task<IEnumerable<Jackpot>> ListJackpotsAsync() => GetAsync<IEnumerable<Jackpot>>("api/game/jackpots");

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

        public async Task DeleteGameAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/game/{id}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        private async Task<T> GetAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(
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

        Task DeleteGameAsync(int id);
        Task<Game> GetGameAsync(int id);
        Task<IEnumerable<Game>> ListGamesAsync();

        Task<IEnumerable<Jackpot>> ListJackpotsAsync();
    }
}