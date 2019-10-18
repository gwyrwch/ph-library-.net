using System;
using System.Collections.Generic;
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
        private User tempUser;
        
        
        public HomeController(ApplicationDbContext _context,
            UserManager<User> userManager)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder
                .UseSqlite("Data Source=app.db");
            db = _context;
            db.Pets.Load();
                
            _userManager = userManager;
            tempUser = _userManager.Users.First(user => user.UserName == "gwyrwch");

            
        }

        public IActionResult Index()
        {
            return Redirect("Home/Profile");
        }

        public IActionResult Feed(uint? type = null, uint? petId = null)
        {
            var posts = TmpRAMDB.Posts();
//            var pets = TmpRAMDB.Pets();
            var users = TmpRAMDB.Users();
            var postsToFeed = new List<PostToFeed>(TmpRAMDB.Posts().Count);


            foreach (var post in posts)
            {
                var usr = users.First(user => user.Id == post.User.Id);
                postsToFeed.Add(new PostToFeed
                {
                    Post = post, 
                    UserName = usr.UserName,
                    UserProfileImage = usr.ProfileImagePath
                });
            }
            
            var filteredPosts = postsToFeed
                .Where(post => type == null || (uint)post.Post.Type == type)
                .OrderBy(post => post.Post.PublicationTime);

            
//            if (petId != null)
//            {
//                 var foundPet = TmpRAMDB.Pets().FirstOrDefault(pet => pet.Id == petId);
//                 if (foundPet != null)
//                     ViewData["PetName"] = foundPet.Name;
//            }
            var l = Enum.GetNames(typeof(PostType)).ToList();
            ViewBag.Types = l;
            return View(filteredPosts);
        }
        public async Task<IActionResult> CreatePost()
        {
            var post = new CreatePostViewModel {Username = tempUser.UserName};
            
            var l = Enum.GetNames(typeof(PostType)).ToList();
            ViewBag.Types = l;
            
            // todo: hold pets in viewbag??) because it maybe doesn't work (pass null to post request)
            post.Pets = db.Pets.Where(pet => pet.UserId == tempUser.Id).ToImmutableList();
            
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel newPost)
        {
            var path = "";
            if (newPost.PostImage != null)
            {
                var ext = newPost.PostImage.FileName.Split('.').Last();
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + "/images/posts/" + newPost.Post.User.UserName + "." + ext;
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await newPost.PostImage.CopyToAsync(fs);
                }
            }

            newPost.Post.ImagePath = path;
            newPost.Post.User = _userManager.Users.First(user => user.UserName == newPost.Username);
            
            for (int i = 0; i < newPost.Pets.Count; i++)
            {
                newPost.Post.PetsToPosts.Add(new PetToPost()
                {
                    Post = newPost.Post, 
                    PostId = newPost.Post.Id,
                    // fixme: maybe bug 
                    Pet = newPost.Pets.ElementAt(i),
                    PetId = newPost.Pets.ElementAt(i).Id
                });
            }
            

            return View();
        }

        public async Task<IActionResult> Profile(uint? petId = null)
        {
            var uid = 1;
            var posts = TmpRAMDB.Posts()
                .Where(post => post.User.Id == uid.ToString())
                .Where(post => petId == null || post.PetsToPosts.First(pp => pp.PetId == petId.ToString()).PetId.ToString()== petId.ToString())
                .OrderByDescending(post => post.PublicationTime);
            var pets = TmpRAMDB.Pets().Where(pet => pet.User.Id == uid.ToString());
            var currentUser = TmpRAMDB.Users().First(user => user.Id == uid.ToString());
            var profile = new ProfileViewModel {Posts = posts, Pets = pets, User = currentUser};
            
            return View(profile);
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
