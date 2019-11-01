using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ph.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Posts = new List<Post>();
            Pets = new List<Pet>();
            Likes = new List<Like>();
        }
        
        
        [RegularExpression(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$", 
            ErrorMessage = "Invalid name.")]
        [PersonalData]
        public string Name { get; set; }

        [RegularExpression(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$", 
            ErrorMessage = "Invalid surname.")]
        [PersonalData]
        public string Surname { get; set; }

        private DateTime _birth;
        
        [DataType(DataType.Date)]
        [PersonalData]
        public DateTime Birth {
            get => _birth;
            set {
                DateTime today = DateTime.Today;
                DateTime tenYearsAgo = today.AddYears(-10);
                if (value > tenYearsAgo)
                    throw new Exception("You are too young to use this app");
                _birth = value;
            }
        }
   
        [PersonalData]
        public string ProfileImagePath { get; set; }
        
        [PersonalData]
        public int GetUserAge()
        {
            var years = DateTime.Now.Year - Birth.Year;
            var birthdayThisYearPassed = Birth.AddYears(years) <= DateTime.Now;

            return birthdayThisYearPassed ? years : years - 1;
        }


        [JsonIgnore]
        public ICollection<Pet> Pets { get; set; }
        [JsonIgnore]
        public ICollection<Post> Posts { get; set; }
        [JsonIgnore]
        public ICollection<Like> Likes { get; set; }
    }
}