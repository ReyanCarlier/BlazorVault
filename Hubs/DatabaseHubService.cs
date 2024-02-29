using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace BlazorVault.Hubs
{
    public class DatabaseHubService(NavigationManager navigationManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        public HubConnection HubConnection { get; private set; }

        public async Task InitializeConnectionAsync(Dictionary<string, string> cookies)
        {
            HubConnection = new HubConnectionBuilder()
             .WithUrl(_navigationManager.ToAbsoluteUri("/databasehub"), options =>
             {
                 options.UseDefaultCredentials = true;
                 var cookieCount = cookies.Count;
                 var cookieContainer = new CookieContainer(cookieCount);
                 foreach (var cookie in cookies)
                     cookieContainer.Add(new Cookie(
                         cookie.Key,
                         WebUtility.UrlEncode(cookie.Value),
                         path: "/",
                         domain: _navigationManager.ToAbsoluteUri("/").Host));
                 options.Cookies = cookieContainer;

                 foreach (var header in cookies)
                     options.Headers.Add(header.Key, header.Value);

                 options.HttpMessageHandlerFactory = (input) =>
                 {
                     var clientHandler = new HttpClientHandler
                     {
                         PreAuthenticate = true,
                         CookieContainer = cookieContainer,
                         UseCookies = true,
                         UseDefaultCredentials = true,
                     };
                     return clientHandler;
                 };
             })
             .WithAutomaticReconnect()
             .Build();

            await HubConnection.StartAsync();
        }
    }
}
