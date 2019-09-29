using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ph.Models;

namespace ph.Controllers
{
    public class AuthController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return Redirect("Auth/Login");
        }

        public async Task<IActionResult> Registration()
        {
            return View();
        }

        [HttpPost]
        // todo: add checking password
        public async Task<IActionResult> Registration([Bind("UserName, Email")]User user)
        {
            var alreadyExists = TmpRAMDB.Users().Count(u => u.Email == user.Email || u.UserName == user.UserName);
            if (alreadyExists != 0)
            {
                return Redirect("/Home/Feed");
            }
            

            if (user.Email != string.Empty && user.UserName != String.Empty)
            {
                user.Id = (uint) (user.UserName + user.Email).GetHashCode();
                TmpRAMDB.Users().Add(user);
                return Redirect("/Home/Feed");
            }

            return Redirect("/Home/Feed");
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }
        
        [HttpPost]
        // todo: add checking password
        public async Task<IActionResult> Login([Bind("UserName")] User user)
        {
            return Redirect("/Home/Feed");
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
    }
}