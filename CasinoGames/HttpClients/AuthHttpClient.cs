using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace CasinoGames.Website.HttpClients
{
    public class AuthHttpClient : IAuthHttpClient
    {
        private readonly HttpClient client;

        public AuthHttpClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<bool> Login()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/admin/login");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<string> Info()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/admin");

            var response = await client.SendAsync(request);

            if (StatusCodes.Status401Unauthorized.Equals(response.StatusCode))
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }
    }

    public interface IAuthHttpClient
    {
        Task<bool> Login();
        Task<string> Info();
    }
}
