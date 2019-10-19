using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ph.Data;
using ph.Models;

namespace ph.Components
{
    public class PostsToProfileViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public PostsToProfileViewComponent(ApplicationDbContext context)
        {
            db = context;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(
            string petId, string userId)
        {
            var items = await GetItemsAsync(petId, userId);
            
            //todo: again problems with paths
            foreach (var post in items)
            {
                post.ImagePath = post.ImagePath.Remove(0, 37);
            }
            
            return View(items);
        }
        private Task<List<Post>> GetItemsAsync(string petId, string userId)
        {
            Task<List<Post>> posts;
            if (petId != null)
            {
                posts = db.PetsToPosts
                    .Where(pp => pp.PetId == petId)
                    .Select(pp => pp.Post)
                    .OrderByDescending(post => post.PublicationTime)
                    .ToListAsync();   
            }
            else 
            {
                posts = db.Posts
                    .Where(post => post.User.Id == userId)
                    .Where(post => petId == null)
                    .OrderByDescending(post => post.PublicationTime)
                    .ToListAsync();
            }
            
            return posts;
        }
    }
}