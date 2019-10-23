using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ph.Data;
using ph.Models;

namespace ph.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<User> _userManager = null;
        private SignInManager<User> _signInManager = null;

        public HomeController(ApplicationDbContext _context,
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

        public IActionResult Index()
        {
            return Redirect("Home/Profile");
        }

        public async Task<IActionResult> Feed(uint? type = null, uint? petId = null)
        {
            var l = Enum.GetNames(typeof(PostType)).ToList();
            
            ViewBag.Types = l;

            int model = type != null ? (int)type : -1; 
            return View(model);
        }
        public async Task<IActionResult> CreatePost()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var post = new CreatePostViewModel {Username = currentUser.UserName};
            
            var l = Enum.GetNames(typeof(PostType)).ToList();
            ViewBag.Types = l;
            
            post.Pets = db.Pets.Where(pet => pet.UserId == currentUser.Id).ToImmutableList();
            
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel newPost)
        {
            var path = "";
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            newPost.Post.User = currentUser;
            // todo : right image path of the post
            
            var postsAmount = db.Posts.Count(post => post.UserId == currentUser.Id);
            if (newPost.PostImage != null)
            {
                var ext = newPost.PostImage.FileName.Split('.').Last();
                //todo: generate path))0)
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + 
                       "/images/posts/" + newPost.Username + postsAmount + "." + ext;
                
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await newPost.PostImage.CopyToAsync(fs);
                }
            }

            newPost.Post.ImagePath = path;
            var petsOnPostIds = newPost.SelectedPets.Split(',');
            for (int i = 0; i < petsOnPostIds.Length; i++)
            {
                newPost.Post.PetsToPosts.Add(new PetToPost()
                {
                    Post = newPost.Post, 
                    PostId = newPost.Post.Id,
                    // fixme: maybe bug 
                    Pet = db.Pets.First(pet => pet.Id == petsOnPostIds[i]),
                    PetId = petsOnPostIds[i]
                });
            }
            newPost.Post.PublicationTime  = DateTime.Now;

            db.Posts.Add(newPost.Post);
            db.SaveChanges();

            return Redirect("CreatePost");
        }

        public async Task<IActionResult> Profile(string petId = null)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var pets = db.Pets.Where(pet => pet.User.Id == currentUser.Id);
            if (!string.IsNullOrEmpty(currentUser.ProfileImagePath))
                currentUser.ProfileImagePath = currentUser.ProfileImagePath.Remove(0, 37);
            foreach (var pet in pets)
            {
                if (!string.IsNullOrEmpty(pet.ProfileImagePath))
                    pet.ProfileImagePath = pet.ProfileImagePath.Remove(0, 37);
            }
            
            var profile = new ProfileViewModel {Pets = pets, User = currentUser, PetIdToShow = petId};
            
            return View(profile);
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Settings()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            return View(currentUser);
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
            return Json(new{});
        }
        
  
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
        
        
    }
}
