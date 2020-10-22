using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebApp.Hubs
{
    public class ChatHub:Hub
    {
        // https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-3.1&tabs=visual-studio
        // Remember: Open WebSocket in IIS: https://docs.microsoft.com/en-us/iis/configuration/system.webserver/websocket
        /*public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }*/
    }
}
