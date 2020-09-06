using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoGames.Website.HttpClients
{
    public class CookieHandler : HttpClientHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CookieHandler(IHttpContextAccessor httpContextAccessor)
        {
            UseCookies = false;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            // Copy all cookies to send to request
            var requestCookies = context.Request.Cookies;
            if (requestCookies.Any())
            {
                request.Headers.Add("Cookie", string.Join("; ", requestCookies.Select(c => $"{c.Key}={c.Value}")));
            }

            var message = await base.SendAsync(request, cancellationToken);

            // Store all cookies from request
            if (message.Headers.TryGetValues(HeaderNames.SetCookie, out var responseCookies))
            {
                var container = new CookieContainer();
                var uri = message.RequestMessage.RequestUri;
                foreach (var cookieHeader in responseCookies)
                {
                    container.SetCookies(uri, cookieHeader);
                }

                foreach (var cookie in container.GetCookies(uri).OfType<Cookie>())
                {
                    var options = new CookieOptions
                    {
                        HttpOnly = cookie.HttpOnly,
                        Path = cookie.Path,
                        Secure = cookie.Secure
                    };

                    if (!DateTime.MinValue.Equals(cookie.Expires))
                    {
                        options.Expires = cookie.Expires;
                    }
                    context.Response.Cookies.Append(cookie.Name, cookie.Value, options);
                }
            }

            return message;
        }
    }
}