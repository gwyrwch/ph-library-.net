using System.ComponentModel.DataAnnotations;

namespace ph.Models
{
    public enum PetType
    {
        [Display(Name = "Dog")]
        Dog,
        [Display(Name = "Cat")]
        Cat,
        [Display(Name = "Rabbit")]
        Rabbit,
        [Display(Name = "Rodent")]
        Rodent,
        [Display(Name = "Reptile")]
        Reptile,
        [Display(Name = "Bird")]
        Bird,
        [Display(Name = "WildAnimal")]
        WildAnimal
    }
}