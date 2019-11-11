using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Localization;
using ph.Data;
using ph.Models;

namespace ph.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly IStringLocalizer<AuthController> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;


        public HomeController(ApplicationDbContext _context,
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            IStringLocalizer<AuthController> localizer,
            IStringLocalizer<SharedResource> sharedLocalizer)
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
            
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
        }

        public IActionResult Index()
        {
            return Redirect("Profile");
        }

        public  IActionResult Feed(int type)
        {
            var types = Enum.GetNames(typeof(PostType)).ToList();

            for (var i = 0; i < types.Count; i++)
            {
                types[i] = _localizer[types[i]];
            }

            ViewBag.Types = types;
            
            return View(type);
        }
        public async Task<IActionResult> CreatePost()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var post = new CreatePostViewModel {Username = currentUser.UserName};
            
            var types = Enum.GetNames(typeof(PostType)).ToList();

            for (var i = 0; i < types.Count; i++)
            {
                types[i] = _localizer[types[i]];
            }

            ViewBag.Types = types;
            
            post.Pets = db.Pets.Where(pet => pet.UserId == currentUser.Id).ToImmutableList();
            
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel newPost)
        {
            var path = "";
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            newPost.Post.User = currentUser;
            
            var postsAmount = db.Posts.Count(post => post.UserId == currentUser.Id);
            if (newPost.PostImage != null)
            {
                var ext = newPost.PostImage.FileName.Split('.').Last();
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

        public async Task<IActionResult> Settings()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            return View(new SignUpViewModel() {User = currentUser});
        }
        
        [HttpPost]
        public async Task<IActionResult> Settings(SignUpViewModel userEdit)
        {
//            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
//            
//            if (!string.IsNullOrEmpty(userEdit.User.Name))
//            {
//                currentUser.Name = userEdit.User.Name;
//            }
//            if (!string.IsNullOrEmpty(userEdit.User.Surname))
//            {
//                currentUser.Surname = userEdit.User.Surname;
//            }
//            if (!string.IsNullOrEmpty(userEdit.User.Email))
//            {
//                currentUser.Email = userEdit.User.Email;
//            }
//            if (!string.IsNullOrEmpty(userEdit.User.UserName))
//            {
//                // todo: when change username you need to rename all post image paths 
//                if (!(_userManager.Users.Count(u => u.UserName == userEdit.User.UserName) > 0))
//                    currentUser.UserName = userEdit.User.UserName;
//            }
//
//            if (userEdit.ProfileImage != null)
//            {
//                var ext = userEdit.ProfileImage.FileName.Split('.').Last();
//                // bug: delete old images (because can be different extensions) and there will be images like gwyrwch.jpg gwyrwch.png and so on
//                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + "/images/users/" + currentUser.UserName + "." + ext;
//                using (var fs = new FileStream(path, FileMode.Create))
//                {
//                    await userEdit.ProfileImage.CopyToAsync(fs);
//                }
//                currentUser.ProfileImagePath = path;
//            }

//            var result = await _userManager.UpdateAsync(currentUser);

//            if (result.Succeeded)
//                return Redirect("Profile");

            return View(userEdit);
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
        [Route("Home/SetLanguage")]

        public IActionResult SetLanguage(string culture)
        {
            Console.WriteLine(culture);
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction("Profile", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}
