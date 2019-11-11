using System.ComponentModel.DataAnnotations;

namespace ph.Models
{
    public enum PetType
    {
        [Display(Name = "Пёся")]
        Dog,
        [Display(Name = "Кот")]
        Cat,
        [Display(Name = "Кролик")]
        Rabbit,
        [Display(Name = "Грызун")]
        Rodent,
        [Display(Name = "Ящерица")]
        Reptile,
        [Display(Name = "Птица")]
        Bird,
        [Display(Name = "Дикое животное")]
        WildAnimal
    }
}