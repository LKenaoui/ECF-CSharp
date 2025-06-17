using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ECF.Models
{
    public class Animal
    {
        [Key]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [MaxLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
        [Display(Name = "Nom")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La description est obligatoire")]
        [MaxLength(2000, ErrorMessage = "La description ne peut pas dépasser 2000 caractères")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "La race est obligatoire")]
        [Display(Name = "Race")]
        public int BreedId { get; set; }

        public virtual Breed? Breed { get; set; }
    }
}
