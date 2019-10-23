using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ph.Data;
using ph.Models;

namespace ph.Components
{
    public class PostsToFeedViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;
        private UserManager<User> _userManager = null;

        public PostsToFeedViewComponent(ApplicationDbContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(
            uint? type = null)
        {
            var items = await GetItemsAsync(type);

            return View(items);
        }
        private async Task<List<PostToFeed>> GetItemsAsync(uint? type)
        {
            var posts = db.Posts.ToImmutableList();
            var postsToFeed = new List<PostToFeed>(posts.Count);
            var users = _userManager.Users.ToList();
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            
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
                .Where(post => type == null || (uint) post.Post.Type == type)
                .OrderByDescending(post => post.Post.PublicationTime);

            return filteredPosts.ToList();
        }
    }
}