using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ph.Data;
using ph.Models;

namespace ph.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private ApplicationDbContext db;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        
        
        public LikesController(ApplicationDbContext _context,
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
            _signInManager = signInManager;
        }
        
        public async Task<IActionResult> LikeEvent(string postId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var oldLike = db.Likes.FirstOr(l => l.PostId == postId && l.UserId == currentUser.Id, null);
            if (oldLike != null)
            {
                db.Likes.Remove(oldLike);
            }
            else
            {
                var like = new Like
                {
                    Post = db.Posts.First(post => post.Id == postId),
                    PostId = postId,
                    User = currentUser,
                    UserId = currentUser.Id
                };               
                db.Likes.Add(like);    
            }
            
            db.SaveChanges();
            
            return Ok();
        }
    }
}