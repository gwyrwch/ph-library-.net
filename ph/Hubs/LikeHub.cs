using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ph.Data;
using Microsoft.AspNetCore.Identity;
using ph.Models;

namespace ph.Hubs
{
    public class LikeHub : Hub
    {
        private ApplicationDbContext db;
        private UserManager<User> userManager;

        public LikeHub(ApplicationDbContext _context, UserManager<User> _userManager)
        {
            db = _context;
            userManager = _userManager;
            
            db.Pets.Load();
            db.Posts.Load();
            db.PetsToPosts.Load();
            db.Likes.Load();
        }
        private string GetUserName()
        {
            return Context.User?.Identity.Name;
        }
        
        private string GetPostUserId(string id)
        {
            return db.Posts.First(post => post.Id == id).UserId;
        }

        public async Task LikePost(string liked, string message)
        {
            var userName = GetUserName();
            var id = GetPostUserId(message);
            message = liked;
            if (userManager.Users.First(user => user.Id == id).UserName == userName)
                userName = "you";
            await Clients.User(id).SendAsync("PostLiked", userName, message);
        }
        
    }
}