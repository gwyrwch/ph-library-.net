using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ph.Data;
using ph.Models;

namespace ph.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private ApplicationDbContext db;
        private UserManager<User> _userManager;
        
        public ProfileController(ApplicationDbContext _context,
            UserManager<User> userManager, 
            SignInManager<User> signInManager)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder
                .UseSqlite("Data Source=app.db");
            db = _context;
            db.Pets.Load();
            db.Posts.Load();
            db.PetsToPosts.Load();
            db.Likes.Load();
            
                
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile(string petId, string username)
        {
            User user;
            if (username == "null") //todo: refactor params to be to not strings
                user = await _userManager.GetUserAsync(HttpContext.User);
            else
            {
                user =  _userManager.Users.First(user1 => user1.UserName == username);
            }
            List<Post> posts;
            
            if (petId == "null")
            {
                posts = new List<Post>(db.Posts
                    .Where(post => post.UserId == user.Id)
                    .OrderByDescending(post => post.PublicationTime));
            }
            else
            {
                posts = new List<Post>(db.PetsToPosts
                    .Where(pp => pp.PetId == petId)
                    .Select(pp => pp.Post)
                    .OrderByDescending(post => post.PublicationTime));
            }
            
            foreach (var post in posts)
            {
                if (!String.IsNullOrEmpty(post.ImagePath))
                {
                    post.ImagePath = post.ImagePath.Remove(0, 37);
                }
            }

            return Ok(posts.ToList());
        }
        
    }
}