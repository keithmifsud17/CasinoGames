using CasinoGames.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CasinoGames.Api.HttpClients
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
        Task<IEnumerable<Game>> ListGamesAsync();

        Task<IEnumerable<Jackpot>> ListJackpotsAsync();
    }
}