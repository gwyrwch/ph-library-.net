using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ph.Models;

namespace ph
{
    public class Program
    {
        public static void Main(string[] args)
        {
//            User user = new User
//            {
//                Birth = new DateTime(2000, 06, 07),
//                UserName = "hloya_ygrt",
//                Email = "feelthepowerofsda@yandex.ru",
//                Id = 1,
//                Name = "Yury",
//                ProfileImagePath = "/dev/null",
//                Surname = "Shilyaev"
//            };
//            
//            TmpRAMDB.Users().Add(user);
//            return;

//            Pet pet = new Pet
//            {
//                Id = 11,
//                OwnerId = 1,
//                Name = "Tasya",
//                PetType = PetType.Cat,
//                Breed = "idk"
//            };
//            TmpRAMDB.Pets().Add(pet);
//            return;

//            Post post = new Post
//            {
//                AuthorId = 1,
//                Description = "Hi, I'm Yury.",
//                Id = 1,
//                ImagePath = "./none",
//                PublicationTime = DateTime.Now,
//                IncludedPetId = null,
//                Type = PostType.Other
//            };
//            TmpRAMDB.Posts().Add(post);
//            return;

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
