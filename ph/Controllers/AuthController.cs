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
            Console.WriteLine("kek");
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
    }
}