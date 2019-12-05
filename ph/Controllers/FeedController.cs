using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class FeedController : ControllerBase
    {
        private ApplicationDbContext db;
        private UserManager<User> _userManager;
        
        public FeedController(ApplicationDbContext _context,
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

        public async Task<IActionResult> Feed(int type)
        {
            var posts = db.Posts.ToImmutableList();
            var postsToFeed = new List<PostToFeed>(posts.Count);
            var users = _userManager.Users.ToList();
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            
            // todo: show amount of likes
            // todo: save images somewhere else not local

            foreach (var post in posts)
            {
                var author = users.First(user => user.Id == post.User.Id);
                //todo: again problems with paths
                var newUserImgPath = "";
                if (!String.IsNullOrEmpty(author.ProfileImagePath))
                    newUserImgPath = author.ProfileImagePath.Remove(0, 37);
                postsToFeed.Add(new PostToFeed
                {
                    Post = post, 
                    UserName = author.UserName,
                    UserProfileImage = newUserImgPath,
                    Liked = db.Likes.Count(like => like.UserId == currentUser.Id && like.PostId == post.Id) > 0
                });
            }

            foreach (var post in postsToFeed)
            {
                if (!String.IsNullOrEmpty(post.Post.ImagePath))
                    post.Post.ImagePath = post.Post.ImagePath.Remove(0, 37);
            }

            var filteredPosts = postsToFeed
                .Where(post => type == -1 || (uint) post.Post.Type == type)
                .OrderByDescending(post => post.Post.PublicationTime);

            return Ok(filteredPosts.ToList());
        }
        
    }
}