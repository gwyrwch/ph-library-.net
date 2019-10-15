using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ph.Models;

namespace ph.Controllers
{
    public class HomeController : Controller
    {
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
                var usr = users.First(user => user.Id == post.AuthorId.ToString());
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
            // todo create view
            // придумать айдишник
            ViewBag.AvailableAuthorIds = new List<SelectListItem>(TmpRAMDB.Users().Select(user => new SelectListItem
               {Text = user.UserName, Value = user.Id.ToString()}));

            var post = new Post();
            
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([Bind("Description, PostType, AuthorId")]Post post)
        {
            post.ImagePath = "/idk.png";
            post.PublicationTime = DateTime.Now;
            post.IncludedPetId = null;

            post.Id = (uint) (post.Description.GetHashCode() + post.AuthorId.GetHashCode() + post.PublicationTime.GetHashCode());

            if (post.Description != String.Empty)
            {
                TmpRAMDB.Posts().Add(post);
                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }

        public async Task<IActionResult> Profile(uint? petId = null)
        {
            var uid = 1;
            var posts = TmpRAMDB.Posts()
                .Where(post => post.AuthorId == uid)
                .Where(post => petId == null || post.IncludedPetId == petId)
                .OrderByDescending(post => post.PublicationTime);
            var pets = TmpRAMDB.Pets().Where(pet => pet.OwnerId == uid);
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
