using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ph.Models
{
    public class User
    {
        [Required]
        public uint Id { get; set; }
        
        [RegularExpression(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$", 
            ErrorMessage = "Invalid name.")]
        [Required]
        public string Name { get; set; }

        [RegularExpression(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$", 
            ErrorMessage = "Invalid name.")]
        [Required]
        public string Surname { get; set; }

        private DateTime _birth;
        
        [DataType(DataType.Date)]
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

        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Invalid characters in username")]
        [Required]
        public string UserName { get; set; }

        [EmailAddress] 
        [Required]
        public string Email { get; set; }     
        
        public string ProfileImagePath { get; set; }

        public int GetUserAge()
        {
            var years = DateTime.Now.Year - this.Birth.Year;
            var birthdayThisYearPassed = this.Birth.AddYears(years) <= DateTime.Now;

            return birthdayThisYearPassed ? years : years - 1;
        }
    }
}