using Microsoft.AspNetCore.SignalR;

namespace BlazorVault.Hubs
{
    /*
     * DatabaseHub is a SignalR hub that is used to send messages to clients when there is a change in the database.
     * This is used to notify clients when a password is added, updated, or deleted, so that the update can be reflected in the UI.
     * This also applies to categories, groups, and roles.
     */
    public class DatabaseHub : Hub
    {
        private static readonly Dictionary<string, HashSet<string>> _connections = [];

        public override async Task OnConnectedAsync()
        {
            var email = Context?.User?.Identity?.Name;
            var connectionId = Context?.ConnectionId;

            if (email != null && connectionId != null)
            {
                lock (_connections)
                {
                    if (!_connections.TryGetValue(email, out var connections))
                    {
                        connections = [];
                        _connections[email] = connections;
                    }

                    connections.Add(connectionId);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                Console.WriteLine(exception.Message);
            }
            var email = Context?.User?.Identity?.Name;

            if (email != null)
            {
                lock (_connections)
                {
                    if (_connections.TryGetValue(email, out var connections))
                    {
                        if (Context?.ConnectionId == null)
                        {
                            connections.Clear();
                        }
                        else
                        {
                            connections.Remove(Context.ConnectionId);
                        }

                        if (connections.Count == 0)
                        {
                            _connections.Remove(email);
                        }
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToSpecificClient(string mail, string action, string message)
        {
            if (_connections.TryGetValue(mail, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync(action, mail, message);
                }
            }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task UpdateDatabase()
        {
            await Clients.All.SendAsync("UpdateDatabase", "", "");
        }

    }
}
