using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ph.Hubs
{
    public class LikeHub : Hub
    {
        private string GetUserName()
        {
            return Context.User?.Identity.Name;
        }

        public async Task LikePost(string liked, string message)
        {
            var userName = GetUserName();
            message += " " + liked;
            await this.Clients.All.SendAsync("PostLiked", userName, message);
        }
        
    }
}