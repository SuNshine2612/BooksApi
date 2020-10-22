using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebApp.Hubs
{
    public class DrawDotHub: Hub
    {
        // http://blog.medhat.ca/2020/02/build-simple-sketchpad-app-with-signalr.html
        // Không xài 2 SignalR đc, nên mang hàm SendMessage sang đây
        public async Task UpdateCanvas(int x, int y)
        {
            await Clients.All.SendAsync("updateDot", x, y);
        }

        public async Task ClearCanvas()
        {
            await Clients.All.SendAsync("clearCanvas");
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
