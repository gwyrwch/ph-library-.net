using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ph.Data;
using ph.Models;

namespace ph.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<User> _userManager = null;

        public HomeController(ApplicationDbContext _context,
            UserManager<User> userManager)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder
                .UseSqlite("Data Source=app.db");
            db = _context;
            db.Pets.Load();
            db.Posts.Load();
            db.PetsToPosts.Load();
                
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return Redirect("Home/Profile");
        }

        public async Task<IActionResult> Feed(uint? type = null, uint? petId = null)
        {
            var l = Enum.GetNames(typeof(PostType)).ToList();
            
            ViewBag.Types = l;

            int model;
            if (type != null)
                model = (int) type;
            else model = -1;
                
            return View(model);
        }
        public async Task<IActionResult> CreatePost()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var post = new CreatePostViewModel {Username = currentUser.UserName};
            
            var l = Enum.GetNames(typeof(PostType)).ToList();
            ViewBag.Types = l;
            
            // todo: hold pets in viewbag??) because it maybe doesn't work (pass null to post request)
            post.Pets = db.Pets.Where(pet => pet.UserId == currentUser.Id).ToImmutableList();
            
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel newPost)
        {
            var path = "";
            newPost.Post.User = _userManager.Users.First(user => user.UserName == newPost.Username);
            // todo : right image path of the post
            if (newPost.PostImage != null)
            {
                var ext = newPost.PostImage.FileName.Split('.').Last();
                //todo: generate path))0)
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + "/images/posts/" + newPost.Post.User.UserName + "3" + "." + ext;
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
            if (currentUser.ProfileImagePath != "")
                currentUser.ProfileImagePath = currentUser.ProfileImagePath.Remove(0, 37);
            foreach (var pet in pets)
            {
                if (pet.ProfileImagePath != "")
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
    }
}
