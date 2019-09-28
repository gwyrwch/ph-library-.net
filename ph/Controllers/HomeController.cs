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
            ICollection<Post> posts = TmpRAMDB.Posts();
            
            var filteredPosts = posts
                .Where(post => type == null || (uint)post.Type == type)
                .Where(post => petId == null || post.IncludedPetId == petId)
                .OrderBy(post => post.PublicationTime);
            
            if (petId != null)
            {
                 var foundPet = TmpRAMDB.Pets().FirstOrDefault(pet => pet.Id == petId);
                 if (foundPet != null)
                     ViewData["PetName"] = foundPet.Name;
            }
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
        
        public async Task<IActionResult> CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([Bind("UserName, Name, Surname, Birth, Email")]User user)
        {
            //todo: add animals when creating
            user.Id = (uint) (user.UserName.GetHashCode() + user.Birth.GetHashCode() + user.Surname.GetHashCode());
            
            if (user.UserName != String.Empty && user.Name != string.Empty && user.Surname != string.Empty)
            {
                TmpRAMDB.Users().Add(user);
                return RedirectToAction(nameof(Index));
            }
            
            
            
            return View(user);
        }
        
        public async Task<IActionResult> Profile()
        {
            var uid = 1;
            var posts = TmpRAMDB.Posts().Where(post => post.AuthorId == uid);
            var pets = TmpRAMDB.Pets().Where(pet => pet.OwnerId == uid);
            var currentUser = TmpRAMDB.Users().First(user => user.Id == uid);
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
