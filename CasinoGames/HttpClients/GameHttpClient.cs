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

        public async Task<IEnumerable<Game>> ListGamesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/game");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<Game>>(
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
    }
}